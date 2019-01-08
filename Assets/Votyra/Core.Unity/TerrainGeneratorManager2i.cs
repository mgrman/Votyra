using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UniRx.Async;
using UnityEngine;
using Votyra.Core.GroupSelectors;
using Votyra.Core.ImageSamplers;
using Votyra.Core.Logging;
using Votyra.Core.MeshUpdaters;
using Votyra.Core.Models;
using Votyra.Core.Pooling;
using Votyra.Core.Profiling;
using Votyra.Core.TerrainGenerators.TerrainMeshers;
using Votyra.Core.TerrainMeshes;
using Votyra.Core.Utils;
using Zenject;

namespace Votyra.Core
{
    //TODO: move to floats
    public class TerrainGeneratorManager2i : IDisposable
    {
        private readonly Dictionary<Vector2i, TerrainGroupGeneratorManager2i> _activeGroups = new Dictionary<Vector2i, TerrainGroupGeneratorManager2i>();
        private readonly Vector2i _cellInGroupCount;
        private readonly IFrameDataProvider2i _frameDataProvider;
        private readonly Func<GameObject> _gameObjectFactory;

        private readonly HashSet<Vector2i> _groupsToRecompute = new HashSet<Vector2i>();
        private readonly IInterpolationConfig _interpolationConfig;
        private readonly IThreadSafeLogger _logger;

        private readonly CancellationTokenSource _onDestroyCts = new CancellationTokenSource();
        private readonly IProfiler _profiler;
        private readonly IStateModel _stateModel;
        private readonly ITerrainConfig _terrainConfig;
        private readonly ITerrainUVPostProcessor _uvPostProcessor;

        private readonly ITerrainVertexPostProcessor _vertexPostProcessor;

        private readonly int _meshTopologyDistance;


        public TerrainGeneratorManager2i(Func<GameObject> gameObjectFactory, IThreadSafeLogger logger, ITerrainConfig terrainConfig, IStateModel stateModel, IProfiler profiler, IFrameDataProvider2i frameDataProvider, [InjectOptional] ITerrainVertexPostProcessor vertexPostProcessor, [InjectOptional] ITerrainUVPostProcessor uvPostProcessor, IInterpolationConfig interpolationConfig)
        {
            _gameObjectFactory = gameObjectFactory;
            _logger = logger;
            _terrainConfig = terrainConfig;
            _cellInGroupCount = _terrainConfig.CellInGroupCount.XY;
            _stateModel = stateModel;
            _profiler = profiler;
            _frameDataProvider = frameDataProvider;
            _vertexPostProcessor = vertexPostProcessor;
            _uvPostProcessor = uvPostProcessor;
            _interpolationConfig = interpolationConfig;
            _meshTopologyDistance = _interpolationConfig.ActiveAlgorithm == IntepolationAlgorithm.Cubic && _interpolationConfig.MeshSubdivision != 1 ? 2 : 1;

            StartUpdateing();
        }

        public void Dispose()
        {
            _onDestroyCts.Cancel();
        }

        private async void StartUpdateing()
        {
            if (_terrainConfig.Async)
            {
                await Task.Run(async () =>
                    {
                        while (!_onDestroyCts.IsCancellationRequested)
                        {
                            try
                            {
                                if (_stateModel.IsEnabled)
                                {
                                    var context = await TaskUtils.RunOnMainThread(() => _frameDataProvider.GetCurrentFrameData(_meshTopologyDistance));
                                    UpdateTerrain(context, _onDestroyCts.Token);
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.LogException(ex);
                            }
                        }
                    })
                    .ConfigureAwait(false);
            }
            else
            {
                while (!_onDestroyCts.IsCancellationRequested)
                {
                    try
                    {
                        if (_stateModel.IsEnabled)
                        {
                            var context = _frameDataProvider.GetCurrentFrameData(_meshTopologyDistance);
                            UpdateTerrain(context, _onDestroyCts.Token);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogException(ex);
                    }

                    await UniTask.Yield();
                }
            }
        }

        private void UpdateTerrain(IFrameData2i context, CancellationToken token)
        {
            context?.Activate();

            HandleVisibilityUpdates(context, token);
            UpdateGroupManagers(context);

            context?.Deactivate();
        }

        private void UpdateGroupManagers(IFrameData2i context)
        {
            foreach (var activeGroup in _activeGroups)
            {
                activeGroup.Value.Update(context);
            }
        }

        private void HandleVisibilityUpdates(IFrameData2i context, CancellationToken token)
        {
            context.UpdateGroupsVisibility(_cellInGroupCount, _groupsToRecompute, newGroup =>
            {
                _activeGroups.Add(newGroup, CreateGroupManager(token, newGroup));
            }, removedGroup =>
            {
                if (!_activeGroups.TryGetValue(removedGroup, out var groupManager))
                    return;
                _activeGroups.Remove(removedGroup);
                groupManager?.Dispose();
            });
        }

        private TerrainGroupGeneratorManager2i CreateGroupManager(CancellationToken token, Vector2i newGroup) => new TerrainGroupGeneratorManager2i(_interpolationConfig, _vertexPostProcessor, _uvPostProcessor, _cellInGroupCount, _gameObjectFactory, newGroup, token, _terrainConfig.Async);
    }

    public class TerrainGroupGeneratorManager2i : IDisposable
    {
        public static IEqualityComparer<TerrainGroupGeneratorManager2i> GroupEqualityComparer = new TerrainGroupGeneratorManager2iGroupEquality();
        private readonly Vector2i _cellInGroupCount;
        private readonly CancellationTokenSource _cts;
        private readonly Vector2i _group;

        private readonly IInterpolationConfig _interpolationConfig;
        private readonly Range2i _range;
        private readonly CancellationToken _token;
        private readonly Func<GameObject> _unityDataFactory;
        private readonly ITerrainUVPostProcessor _uvPostProcessor;
        private readonly ITerrainVertexPostProcessor _vertexPostProcessor;
        private readonly bool _async;
        private Task _activeTask = Task.CompletedTask;
        private IFrameData2i _contextToProcess;
        private GameObject _unityData;

        private bool _updatedOnce;

        public TerrainGroupGeneratorManager2i(IInterpolationConfig interpolationConfig, ITerrainVertexPostProcessor vertexPostProcessor, ITerrainUVPostProcessor uvPostProcessor, Vector2i cellInGroupCount, Func<GameObject> unityDataFactory, Vector2i group, CancellationToken token, bool async)
        {
            _interpolationConfig = interpolationConfig;
            _vertexPostProcessor = vertexPostProcessor;
            _uvPostProcessor = uvPostProcessor;
            _cellInGroupCount = cellInGroupCount;
            _unityDataFactory = unityDataFactory;
            _group = group;
            _range = Range2i.FromMinAndSize(group * cellInGroupCount, cellInGroupCount);
            _cts = CancellationTokenSource.CreateLinkedTokenSource(token);
            _token = _cts.Token;
            _async = async;
        }

        public void Dispose()
        {
            _cts.Cancel();

            if (_async)
            {
                DestroyOnMainThreadAsync();
            }
            else
            {
                _unityData.Destroy();
            }
        }

        public void Update(IFrameData2i context)
        {
            if (_token.IsCancellationRequested)
                return;

            if (_updatedOnce && !context.InvalidatedArea.Overlaps(_range))
                return;
            _updatedOnce = true;

            _contextToProcess?.Deactivate();

            context.Activate();
            _contextToProcess = context;

            if (_async)
            {
                UpdateGroupInBackground();
            }
            else
            {
                UpdateGroupInForeground();
            }
        }

        private void UpdateGroupInForeground()
        {
            if (_token.IsCancellationRequested)
                return;

            if (_contextToProcess != null)
            {
                var context = _contextToProcess;
                _contextToProcess = null;

                UpdateGroup(context, _token);
                context.Deactivate();
            }
        }

        private void UpdateGroupInBackground()
        {
            if (_token.IsCancellationRequested)
                return;

            if (_activeTask.IsCompleted && _contextToProcess != null)
            {
                var context = _contextToProcess;
                _contextToProcess = null;

                _activeTask = Task.Run(async () =>
                {
                    await UpdateGroupAsync(context, _token);
                    context.Deactivate();
                });
                _activeTask.ConfigureAwait(false);

                _activeTask.ContinueWith(t => UpdateGroupInBackground());
            }
        }

        private void UpdateGroup(IFrameData2i context, CancellationToken token)
        {
            if (context == null)
                return;
            var unityMesh = GenerateUnityMesh(context);
            if (_token.IsCancellationRequested)
                return;
            UpdateUnityMesh(unityMesh);
        }

        private async Task UpdateGroupAsync(IFrameData2i context, CancellationToken token)
        {
            if (context == null)
                return;
            var unityMesh = GenerateUnityMesh(context);
            if (_token.IsCancellationRequested)
                return;
            await UniTask.SwitchToMainThread();
            if (_token.IsCancellationRequested)
                return;
            UpdateUnityMesh(unityMesh);
        }

        private void UpdateUnityMesh(UnityMesh unityMesh)
        {
            if (_unityData == null)
                _unityData = _unityDataFactory();

            unityMesh.SetUnityMesh(_unityData);
        }

        private UnityMesh GenerateUnityMesh(IFrameData2i context)
        {
            var image = context.Image;
            var mask = context.Mask;

            var bounds = _range.ToArea3f(image.RangeZ);

            IPooledTerrainMesh pooledMesh;
            if (_interpolationConfig.DynamicMeshes)
            {
                pooledMesh = PooledTerrainMeshContainer<ExpandingUnityTerrainMesh>.CreateDirty();
                pooledMesh.Mesh.Clear(bounds, _vertexPostProcessor == null ? (Func<Vector3f, Vector3f>) null : _vertexPostProcessor.PostProcessVertex, _uvPostProcessor == null ? (Func<Vector2f, Vector2f>) null : _uvPostProcessor.ProcessUV);
            }
            else
            {
                var triangleCount = _cellInGroupCount.AreaSum * 2 * _interpolationConfig.MeshSubdivision * _interpolationConfig.MeshSubdivision;
                pooledMesh = PooledTerrainMeshWithFixedCapacityContainer<FixedTerrainMesh2i>.CreateDirty(triangleCount);
                pooledMesh.Mesh.Clear(bounds, _vertexPostProcessor == null ? (Func<Vector3f, Vector3f>) null : _vertexPostProcessor.PostProcessVertex, _uvPostProcessor == null ? (Func<Vector2f, Vector2f>) null : _uvPostProcessor.ProcessUV);
            }

            if (_interpolationConfig.MeshSubdivision > 1 && _interpolationConfig.ActiveAlgorithm == IntepolationAlgorithm.Cubic && !_interpolationConfig.DynamicMeshes)
                BicubicTerrainMesher2f.GetResultingMesh(pooledMesh.Mesh, _group, _cellInGroupCount, image, mask, _interpolationConfig.MeshSubdivision);
            else if (_interpolationConfig.ImageSubdivision > 1 && _interpolationConfig.MeshSubdivision == 1)
                DynamicTerrainMesher2f.GetResultingMesh(pooledMesh.Mesh, _group, _cellInGroupCount, image, mask);
            else if (_interpolationConfig.MeshSubdivision == 1)
                TerrainMesher2f.GetResultingMesh(pooledMesh.Mesh, _group, _cellInGroupCount, image, mask);

            pooledMesh.FinalizeMesh();

            var unityMesh = pooledMesh.Mesh.GetUnityMesh(null);
            return unityMesh;
        }

        private async Task DestroyOnMainThreadAsync()
        {
            await UniTask.SwitchToMainThread();
            _unityData.Destroy();
        }

        private class TerrainGroupGeneratorManager2iGroupEquality : IEqualityComparer<TerrainGroupGeneratorManager2i>
        {
            public bool Equals(TerrainGroupGeneratorManager2i x, TerrainGroupGeneratorManager2i y) => x?._group == y?._group;

            public int GetHashCode(TerrainGroupGeneratorManager2i obj) => obj?._group.GetHashCode() ?? 0;
        }
    }
}