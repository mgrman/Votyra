using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UniRx.Async;
using UnityEngine;
using UnityEngine.Assertions;
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
        private readonly ConcurrentDictionary<Vector2i, ITerrainGroupGeneratorManager2i> _activeGroups = new ConcurrentDictionary<Vector2i, ITerrainGroupGeneratorManager2i>();

        private readonly Vector2i _cellInGroupCount;
        private readonly IFrameDataProvider2i _frameDataProvider;
        private readonly Func<GameObject> _gameObjectFactory;

        private readonly HashSet<Vector2i> _groupsToRecompute = new HashSet<Vector2i>();
        private readonly IInterpolationConfig _interpolationConfig;
        private readonly IThreadSafeLogger _logger;

        private readonly int _meshTopologyDistance;

        private readonly CancellationTokenSource _onDestroyCts = new CancellationTokenSource();
        private readonly IProfiler _profiler;
        private readonly ITerrainConfig _terrainConfig;
        private readonly ITerrainUVPostProcessor _uvPostProcessor;

        private readonly ITerrainVertexPostProcessor _vertexPostProcessor;

        private readonly TaskFactory _taskFactory = new TaskFactory();
        
        private Task _waitForTask = Task.CompletedTask;

        public TerrainGeneratorManager2i(Func<GameObject> gameObjectFactory, IThreadSafeLogger logger, ITerrainConfig terrainConfig, IProfiler profiler, IFrameDataProvider2i frameDataProvider, [InjectOptional] ITerrainVertexPostProcessor vertexPostProcessor, [InjectOptional] ITerrainUVPostProcessor uvPostProcessor, IInterpolationConfig interpolationConfig)
        {
            _gameObjectFactory = gameObjectFactory;
            _logger = logger;
            _terrainConfig = terrainConfig;
            _cellInGroupCount = _terrainConfig.CellInGroupCount.XY;
            _profiler = profiler;
            _frameDataProvider = frameDataProvider;
            _vertexPostProcessor = vertexPostProcessor;
            _uvPostProcessor = uvPostProcessor;
            _interpolationConfig = interpolationConfig;

            if (_terrainConfig.Async)
            {
                _frameDataProvider.FrameData += UpdateTerrainInBackground;
            }
            else
            {
                _frameDataProvider.FrameData += UpdateTerrainInForeground;
            }
        }

        public void Dispose()
        {
            _onDestroyCts.Cancel();

            if (_terrainConfig.Async)
            {
                _frameDataProvider.FrameData -= UpdateTerrainInBackground;
            }
            else
            {
                _frameDataProvider.FrameData -= UpdateTerrainInForeground;
            }
        }

        private void UpdateTerrainInForeground(IFrameData2i context)
        {
            context?.Activate();
            UpdateTerrain(context);
        }


        private void UpdateTerrainInBackground(IFrameData2i context)
        {
            if (!_waitForTask.IsCompleted)
            {
                return;
            }

            context?.Activate();
            _waitForTask = _taskFactory.StartNew(UpdateTerrain, context);
        }

        private void UpdateTerrain(object context)
        {
            UpdateTerrain(context as IFrameData2i);
        }

        private void UpdateTerrain(IFrameData2i context)
        {
            HandleVisibilityUpdates(context);
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

        private void HandleVisibilityUpdates(IFrameData2i context)
        {
            context.UpdateGroupsVisibility(_cellInGroupCount,
                _groupsToRecompute,
                newGroup =>
                {
                    var success = _activeGroups.TryAdd(newGroup, CreateGroupManager(newGroup));
                    Assert.IsTrue(success, "Problem adding new group to the dictionary of active groups, seems the dictionary already contains it.");
                },
                removedGroup =>
                {
                    ITerrainGroupGeneratorManager2i value;
                    var success = _activeGroups.TryRemove(removedGroup, out value);
                    Assert.IsTrue(success, "Problem removing existing group from the dictionary of active groups, seems the dictionary does not contain it.");
                    value.Dispose();
                });
        }

        private ITerrainGroupGeneratorManager2i CreateGroupManager(Vector2i newGroup)
        {
            if (_terrainConfig.Async)
                return new AsyncTerrainGroupGeneratorManager2i(_cellInGroupCount, _gameObjectFactory, newGroup, _onDestroyCts.Token, CreatePooledTerrainMesh(), GetMeshStrategy());
            return new SyncTerrainGroupGeneratorManager2i(_cellInGroupCount, _gameObjectFactory, newGroup, _onDestroyCts.Token, CreatePooledTerrainMesh(), GetMeshStrategy());
        }

        private IPooledTerrainMesh CreatePooledTerrainMesh()
        {
            IPooledTerrainMesh pooledMesh;
            if (_interpolationConfig.DynamicMeshes)
            {
                pooledMesh = PooledTerrainMeshContainer<ExpandingUnityTerrainMesh>.CreateDirty();
            }
            else
            {
                var triangleCount = _cellInGroupCount.AreaSum * 2 * _interpolationConfig.MeshSubdivision * _interpolationConfig.MeshSubdivision;
                pooledMesh = PooledTerrainMeshWithFixedCapacityContainer<FixedUnityTerrainMesh2i>.CreateDirty(triangleCount);
            }

            pooledMesh.Mesh.Initialize(_vertexPostProcessor == null ? (Func<Vector3f, Vector3f>) null : _vertexPostProcessor.PostProcessVertex, _uvPostProcessor == null ? (Func<Vector2f, Vector2f>) null : _uvPostProcessor.ProcessUV);
            return pooledMesh;
        }

        private Action<IFrameData2i, Vector2i, ITerrainMesh> GetMeshStrategy()
        {
            if (_interpolationConfig.MeshSubdivision > 1 && _interpolationConfig.ActiveAlgorithm == IntepolationAlgorithm.Cubic)
                return GenerateBicubicMesh;
            if (_interpolationConfig.ImageSubdivision > 1 && _interpolationConfig.MeshSubdivision == 1)
                return GenerateMeshByAreas;
            if (_interpolationConfig.MeshSubdivision == 1)
                return GenerateSimpleMesh;
            return null;
        }

        private static void GenerateBicubicMesh(IFrameData2i context, Vector2i group, ITerrainMesh mesh)
        {
            var image = context.Image;
            var mask = context.Mask;
            var cellInGroupCount = context.CellInGroupCount;

            BicubicTerrainMesher2f.GetResultingMesh(mesh, group, cellInGroupCount, image, mask, context.MeshSubdivision);
        }

        private static void GenerateMeshByAreas(IFrameData2i context, Vector2i group, ITerrainMesh mesh)
        {
            var image = context.Image;
            var mask = context.Mask;
            var cellInGroupCount = context.CellInGroupCount;

            DynamicTerrainMesher2f.GetResultingMesh(mesh, group, cellInGroupCount, image, mask);
        }

        private static void GenerateSimpleMesh(IFrameData2i context, Vector2i group, ITerrainMesh mesh)
        {
            var image = context.Image;
            var mask = context.Mask;
            var cellInGroupCount = context.CellInGroupCount;

            TerrainMesher2f.GetResultingMesh(mesh, group, cellInGroupCount, image, mask);
        }
    }

    public interface ITerrainGroupGeneratorManager2i : IDisposable
    {
        void Update(IFrameData2i context);
    }

    public class SyncTerrainGroupGeneratorManager2i : TerrainGroupGeneratorManager2i
    {
        public SyncTerrainGroupGeneratorManager2i(Vector2i cellInGroupCount, Func<GameObject> unityDataFactory, Vector2i group, CancellationToken token, IPooledTerrainMesh pooledMesh, Action<IFrameData2i, Vector2i, ITerrainMesh> generateUnityMesh)
            : base(cellInGroupCount, unityDataFactory, group, token, pooledMesh, generateUnityMesh)
        {
        }

        public override void Dispose()
        {
            base.Dispose();
            _unityData.DestroyWithMeshes();
        }

        protected override void UpdateGroup()
        {
            if (_token.IsCancellationRequested)
                return;

            if (ContextToProcess != null)
            {
                UpdateGroup(ContextToProcess, _token);
                ContextToProcess = null;
            }
        }

        private void UpdateGroup(IFrameData2i context, CancellationToken token)
        {
            if (context == null)
                return;
            UpdateTerrainMesh(context);
            if (_token.IsCancellationRequested)
                return;
            UpdateUnityMesh(_pooledMesh.Mesh);
        }
    }

    public class AsyncTerrainGroupGeneratorManager2i : TerrainGroupGeneratorManager2i
    {
        private Task _activeTask = Task.CompletedTask;

        public AsyncTerrainGroupGeneratorManager2i(Vector2i cellInGroupCount, Func<GameObject> unityDataFactory, Vector2i group, CancellationToken token, IPooledTerrainMesh pooledMesh, Action<IFrameData2i, Vector2i, ITerrainMesh> generateUnityMesh)
            : base(cellInGroupCount, unityDataFactory, group, token, pooledMesh, generateUnityMesh)
        {
        }

        protected override void UpdateGroup()
        {
            if (_token.IsCancellationRequested)
                return;

            if (!_activeTask.IsCompleted || ContextToProcess == null)
                return;

            var context = GetFrameDataWithOwnership();

            _activeTask = Task.Run(async () =>
            {
                try
                {
                    await UpdateGroupAsync(context, _token);
                }
                finally
                {
                    context?.Deactivate();
                }
            });
            _activeTask.ConfigureAwait(false);

            _activeTask.ContinueWith(t => UpdateGroup());
        }

        private async Task UpdateGroupAsync(IFrameData2i context, CancellationToken token)
        {
            if (context == null)
                return;
            UpdateTerrainMesh(context);
            if (_token.IsCancellationRequested)
                return;
            await UniTask.SwitchToMainThread();
            if (_token.IsCancellationRequested)
                return;
            UpdateUnityMesh(_pooledMesh.Mesh);
        }

        public override void Dispose()
        {
            base.Dispose();
            MainThreadUtils.RunOnMainThreadAsync(() =>
            {
                _unityData.DestroyWithMeshes();
            });
        }
    }

    public abstract class TerrainGroupGeneratorManager2i : ITerrainGroupGeneratorManager2i
    {
        protected readonly CancellationTokenSource _cts;
        protected readonly Action<IFrameData2i, Vector2i, ITerrainMesh> _generateUnityMesh;
        protected readonly Vector2i _group;
        protected readonly IPooledTerrainMesh _pooledMesh;
        protected readonly Range2i _range;
        protected readonly CancellationToken _token;
        protected readonly Func<GameObject> _unityDataFactory;

        private IFrameData2i _contextToProcess;

        protected GameObject _unityData;

        private bool _updatedOnce;

        public TerrainGroupGeneratorManager2i(Vector2i cellInGroupCount, Func<GameObject> unityDataFactory, Vector2i group, CancellationToken token, IPooledTerrainMesh pooledMesh, Action<IFrameData2i, Vector2i, ITerrainMesh> generateUnityMesh)
        {
            _unityDataFactory = unityDataFactory;
            _group = group;
            _range = Range2i.FromMinAndSize(group * cellInGroupCount, cellInGroupCount);
            _cts = CancellationTokenSource.CreateLinkedTokenSource(token);
            _token = _cts.Token;

            _pooledMesh = pooledMesh;
            _generateUnityMesh = generateUnityMesh;
        }

        protected IFrameData2i ContextToProcess
        {
            get => _contextToProcess;
            set
            {
                _contextToProcess?.Deactivate();
                _contextToProcess = value;
                _contextToProcess?.Activate();
            }
        }

        public void Update(IFrameData2i context)
        {
            if (_token.IsCancellationRequested)
                return;

            if (_updatedOnce && !context.InvalidatedArea.Overlaps(_range))
                return;
            _updatedOnce = true;

            ContextToProcess = context;

            UpdateGroup();
        }

        public virtual void Dispose()
        {
            ContextToProcess = null;
            _cts.Cancel();
            _pooledMesh.Dispose();
        }

        protected IFrameData2i GetFrameDataWithOwnership()
        {
            var contextToProcess = _contextToProcess;
            _contextToProcess = null;
            return contextToProcess;
        }

        protected abstract void UpdateGroup();

        protected void UpdateTerrainMesh(IFrameData2i context)
        {
            _pooledMesh.Mesh.Reset(Area3f.FromMinAndSize((_group * context.CellInGroupCount).ToVector3f(context.RangeZ.Min), context.CellInGroupCount.ToVector3f(context.RangeZ.Size)));
            _generateUnityMesh(context, _group, _pooledMesh.Mesh);
            _pooledMesh.Mesh.FinalizeMesh();
        }

        protected void UpdateUnityMesh(ITerrainMesh unityMesh)
        {
            if (_unityData == null)
                _unityData = _unityDataFactory();

            unityMesh.SetUnityMesh(_unityData);
        }
    }
}