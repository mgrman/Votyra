using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UniRx.Async;
using UnityEditor;
using UnityEngine;
using Votyra.Core.GroupSelectors;
using Votyra.Core.Images;
using Votyra.Core.ImageSamplers;
using Votyra.Core.Logging;
using Votyra.Core.MeshUpdaters;
using Votyra.Core.Models;
using Votyra.Core.Pooling;
using Votyra.Core.Profiling;
using Votyra.Core.TerrainGenerators;
using Votyra.Core.TerrainGenerators.TerrainMeshers;
using Votyra.Core.TerrainMeshes;
using Votyra.Core.Utils;
using Zenject;

namespace Votyra.Core
{
    //TODO: move to floats
    public class TerrainGeneratorManager2i : IDisposable
    {
        private readonly IFrameDataProvider2i _frameDataProvider;
        private readonly IThreadSafeLogger _logger;
        private readonly IProfiler _profiler;
        private readonly IStateModel _stateModel;
        private readonly ITerrainConfig _terrainConfig;
        private readonly Vector2i _cellInGroupCount;

        private readonly ITerrainVertexPostProcessor _vertexPostProcessor;
        private readonly ITerrainUVPostProcessor _uvPostProcessor;
        private readonly IInterpolationConfig _interpolationConfig;
        private readonly Func<GameObject> _gameObjectFactory;

        private readonly CancellationTokenSource _onDestroyCts = new CancellationTokenSource();


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

            StartUpdateing();
        }

        public void Dispose()
        {
            _onDestroyCts.Cancel();
        }

        private void StartUpdateing()
        {
            Task.Run(async () =>
            {
                while (!_onDestroyCts.IsCancellationRequested)
                {
                    try
                    {
                        if (_stateModel.IsEnabled)
                        {
                            IFrameData2i context = null;
                            await Task.Run(async () =>
                            {
                                await UniTask.SwitchToMainThread();
                                var meshTopologyDistance = (_interpolationConfig.ActiveAlgorithm == IntepolationAlgorithm.Cubic && _interpolationConfig.MeshSubdivision != 1) ? 2 : 1;
                                context = _frameDataProvider.GetCurrentFrameData(meshTopologyDistance);
                            });
                            UpdateTerrain(context, _terrainConfig.Async, _onDestroyCts.Token);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogException(ex);
                    }

                    await Task.Delay(10);
                }
            }).ConfigureAwait(false);
        }

        private readonly Dictionary<Vector2i, TerrainGroupGeneratorManager2i> _activeGroups = new Dictionary<Vector2i, TerrainGroupGeneratorManager2i>();

        private readonly HashSet<Vector2i> _groupsToRecompute = new HashSet<Vector2i>();

        private void UpdateTerrain(IFrameData2i context, bool async, CancellationToken token)
        {
            try
            {
                if (!async)
                {
                    throw new NotImplementedException();
                }
                else
                {
                    context?.Activate();

                    context.UpdateGroupsVisibility(_cellInGroupCount, _groupsToRecompute, newGroup =>
                    {
                        var groupManager = CreateGroupManager(token, newGroup);
                        _activeGroups.Add(newGroup, groupManager);
                    }, removedGroup =>
                    {
                        if (!_activeGroups.TryGetValue(removedGroup, out var groupManager))
                            return;
                        _activeGroups.Remove(removedGroup);
                        groupManager?.Dispose();
                    });

                    foreach (var activeGroup in _activeGroups)
                    {
                        activeGroup.Value.Update(context);
                    }

                    context?.Deactivate();
                }
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
            }
        }

        private TerrainGroupGeneratorManager2i CreateGroupManager(CancellationToken token, Vector2i newGroup)
        {
            return new TerrainGroupGeneratorManager2i(_interpolationConfig, _vertexPostProcessor, _uvPostProcessor, _cellInGroupCount, _gameObjectFactory, newGroup, token);
        }
    }

    public class TerrainGroupGeneratorManager2i : IDisposable
    {
        public TerrainGroupGeneratorManager2i(IInterpolationConfig interpolationConfig, ITerrainVertexPostProcessor vertexPostProcessor, ITerrainUVPostProcessor uvPostProcessor, Vector2i cellInGroupCount, Func<GameObject> unityDataFactory, Vector2i group, CancellationToken token)
        {
            _interpolationConfig = interpolationConfig;
            _vertexPostProcessor = vertexPostProcessor;
            _uvPostProcessor = uvPostProcessor;
            _cellInGroupCount = cellInGroupCount;
            _unityDataFactory = unityDataFactory;
            _group = group;
            _range = Range2i.FromMinAndSize((group * cellInGroupCount), cellInGroupCount);
            _cts = CancellationTokenSource.CreateLinkedTokenSource(token);
            _token = _cts.Token;
        }

        private readonly IInterpolationConfig _interpolationConfig;
        private readonly ITerrainVertexPostProcessor _vertexPostProcessor;
        private readonly ITerrainUVPostProcessor _uvPostProcessor;
        private readonly Vector2i _cellInGroupCount;
        private readonly Func<GameObject> _unityDataFactory;
        private readonly Vector2i _group;
        private readonly Range2i _range;
        private readonly CancellationTokenSource _cts;
        private readonly CancellationToken _token;
        private GameObject _unityData;
        private Task _activeTask = Task.CompletedTask;

        private bool _updatedOnce;
        private IFrameData2i _contextToProcess;

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

            UpdateGroupInBackground();
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
                    await UpdateGroup(context, _token);
                    context.Deactivate();
                });
                _activeTask.ConfigureAwait(false);

                _activeTask.ContinueWith((t) => UpdateGroupInBackground());
            }
        }

        private async Task UpdateGroup(IFrameData2i context, CancellationToken token)
        {
            if (context == null)
            {
                return;
            }

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
            {
                BicubicTerrainMesher2f.GetResultingMesh(pooledMesh.Mesh, _group, _cellInGroupCount, image, mask, _interpolationConfig.MeshSubdivision);
            }
            else if (_interpolationConfig.ImageSubdivision > 1 && _interpolationConfig.MeshSubdivision == 1)
            {
                DynamicTerrainMesher2f.GetResultingMesh(pooledMesh.Mesh, _group, _cellInGroupCount, image, mask);
            }
            else if (_interpolationConfig.MeshSubdivision == 1)
            {
                TerrainMesher2f.GetResultingMesh(pooledMesh.Mesh, _group, _cellInGroupCount, image, mask);
            }

            pooledMesh.FinalizeMesh();

            var unityMesh = pooledMesh.Mesh.GetUnityMesh(null);

            if (_token.IsCancellationRequested)
                return;

            await UniTask.SwitchToMainThread();

            if (_token.IsCancellationRequested)
                return;
            if (_unityData == null)
            {
                _unityData = _unityDataFactory();
            }

            unityMesh.SetUnityMesh(_unityData);
        }

        public void Dispose()
        {
            _cts.Cancel();

            DestroyOnMainThreadAsync();
        }

        private async Task DestroyOnMainThreadAsync()
        {
            await UniTask.SwitchToMainThread();
            _unityData.Destroy();
        }

        public static IEqualityComparer<TerrainGroupGeneratorManager2i> GroupEqualityComparer = new TerrainGroupGeneratorManager2iGroupEquality();

        private class TerrainGroupGeneratorManager2iGroupEquality : IEqualityComparer<TerrainGroupGeneratorManager2i>
        {
            public bool Equals(TerrainGroupGeneratorManager2i x, TerrainGroupGeneratorManager2i y)
            {
                return x?._group == y?._group;
            }

            public int GetHashCode(TerrainGroupGeneratorManager2i obj)
            {
                return obj?._group.GetHashCode() ?? 0;
            }
        }
    }
}