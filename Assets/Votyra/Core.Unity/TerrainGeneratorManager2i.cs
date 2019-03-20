using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UniRx.Async;
using UnityEngine;
using UnityEngine.Assertions;
using Votyra.Core.GroupSelectors;
using Votyra.Core.Images;
using Votyra.Core.ImageSamplers;
using Votyra.Core.Logging;
using Votyra.Core.MeshUpdaters;
using Votyra.Core.Models;
using Votyra.Core.Pooling;
using Votyra.Core.Profiling;
using Votyra.Core.TerrainGenerators.TerrainMeshers;
using Votyra.Core.TerrainMeshes;
using Votyra.Core.Unity;
using Votyra.Core.Utils;
using Zenject;

namespace Votyra.Core
{
    //TODO: move to floats
    public class TerrainGeneratorManager2i : IDisposable
    {
        public readonly ConcurrentDictionary<Vector2i, ITerrainGroupGeneratorManager2i> _activeGroups = new ConcurrentDictionary<Vector2i, ITerrainGroupGeneratorManager2i>();

        private readonly Vector2i _cellInGroupCount;
        private readonly IFrameDataProvider2i _frameDataProvider;
        private readonly ITerrainGameObjectPool _gameObjectPool;

        private readonly HashSet<Vector2i> _groupsToRecompute = new HashSet<Vector2i>();
        private readonly IInterpolationConfig _interpolationConfig;
        private readonly IThreadSafeLogger _logger;

        private readonly int _meshTopologyDistance;

        private readonly CancellationTokenSource _onDestroyCts = new CancellationTokenSource();
        private readonly IProfiler _profiler;
        private readonly TaskFactory _taskFactory = new TaskFactory();
        private readonly ITerrainConfig _terrainConfig;
        private readonly ITerrainMesher2f _terrainMesher;
        private readonly ITerrainMeshPool _terrainMeshPool;

        private Task _waitForTask = Task.CompletedTask;

        public TerrainGeneratorManager2i(ITerrainGameObjectPool gameObjectPool, IThreadSafeLogger logger, ITerrainConfig terrainConfig, IProfiler profiler, IFrameDataProvider2i frameDataProvider, IInterpolationConfig interpolationConfig, ITerrainMesher2f terrainMesher, ITerrainMeshPool terrainMeshPool)
        {
            _gameObjectPool = gameObjectPool;
            _logger = logger;
            _terrainConfig = terrainConfig;
            _cellInGroupCount = _terrainConfig.CellInGroupCount.XY();
            _profiler = profiler;
            _frameDataProvider = frameDataProvider;
            _interpolationConfig = interpolationConfig;
            _terrainMesher = terrainMesher;
            _terrainMeshPool = terrainMeshPool;

            onAdd = OnTerrainGroupAdd;

            onRemove = OnTerrainGroupRemove;

            if (_terrainConfig.Async)
                _frameDataProvider.FrameData += UpdateTerrainInBackground;
            else
                _frameDataProvider.FrameData += UpdateTerrainInForeground;
        }

        public void Dispose()
        {
            _onDestroyCts.Cancel();

            if (_terrainConfig.Async)
                _frameDataProvider.FrameData -= UpdateTerrainInBackground;
            else
                _frameDataProvider.FrameData -= UpdateTerrainInForeground;
        }

        public ITerrainMesh GetMeshForGroup(Vector2i group) =>
            _activeGroups.TryGetValue(group)
                ?.Mesh;

        private void UpdateTerrainInForeground(ArcResource<IFrameData2i> context)
        {
            UpdateTerrain(context);
        }

        private void UpdateTerrainInBackground(ArcResource<IFrameData2i> context)
        {
            if (!_waitForTask.IsCompleted)
            {
                context.Dispose();
                return;
            }

            _waitForTask = _taskFactory.StartNew(UpdateTerrain, context);
        }

        private void UpdateTerrain(object context)
        {
            UpdateTerrain(context as ArcResource<IFrameData2i>);
        }

        private void UpdateTerrain(ArcResource<IFrameData2i> context)
        {
            HandleVisibilityUpdates(context.Value);
            foreach (var activeGroup in _activeGroups)
            {
                activeGroup.Value.Update(context.Activate());
            }

            context.Dispose();
            context = null;
        }

        private void HandleVisibilityUpdates(IFrameData2i context)
        {
            context.UpdateGroupsVisibility(_cellInGroupCount, _groupsToRecompute, onAdd, onRemove);
        }

        private Action<Vector2i> onAdd;
        private Action<Vector2i> onRemove;

        private void OnTerrainGroupRemove(Vector2i removedGroup)
        {
            var success = _activeGroups.TryRemove(removedGroup, out var value);
            Assert.IsTrue(success, "Problem removing existing group from the dictionary of active groups, seems the dictionary does not contain it.");
            var mesh = value.Mesh;
            var gameObject = value.TerrainGameObject;
            value.Dispose();
            _terrainMeshPool.ReturnRaw(mesh);
            if (gameObject.GameObject != null)
                gameObject.GameObject.SetActive(false);
            _gameObjectPool.ReturnRaw(gameObject);
        }

        private void OnTerrainGroupAdd(Vector2i newGroup)
        {
            var success = _activeGroups.TryAdd(newGroup, CreateGroupManager(newGroup));
            Assert.IsTrue(success, "Problem adding new group to the dictionary of active groups, seems the dictionary already contains it.");
        }

        private ITerrainGroupGeneratorManager2i CreateGroupManager(Vector2i newGroup)
        {
            if (_terrainConfig.Async)
                return new AsyncTerrainGroupGeneratorManager2i(_cellInGroupCount, CreatePooledGameObject(), newGroup, _onDestroyCts.Token, CreatePooledTerrainMesh(), _terrainMesher.GetResultingMesh);
            return new SyncTerrainGroupGeneratorManager2i(_cellInGroupCount, CreatePooledGameObject(), newGroup, _onDestroyCts.Token, CreatePooledTerrainMesh(), _terrainMesher.GetResultingMesh);
        }

        private ITerrainMesh CreatePooledTerrainMesh()
        {
            var triangleCount = (uint) (_cellInGroupCount.AreaSum() * 2 * _interpolationConfig.MeshSubdivision * _interpolationConfig.MeshSubdivision);
            return _terrainMeshPool.GetRaw(triangleCount);
        }

        private ITerrainGameObject CreatePooledGameObject()
        {
            return _gameObjectPool.GetRaw();
        }
    }

    public interface ITerrainGroupGeneratorManager2i : IDisposable
    {
        ITerrainGameObject TerrainGameObject { get; }
        ITerrainMesh Mesh { get; }
        void Update(ArcResource<IFrameData2i> context);
    }

    public class SyncTerrainGroupGeneratorManager2i : TerrainGroupGeneratorManager2i
    {
        public SyncTerrainGroupGeneratorManager2i(Vector2i cellInGroupCount, ITerrainGameObject gameObjectPool, Vector2i group, CancellationToken token, ITerrainMesh pooledMesh, Action<ITerrainMesh, Vector2i, IImage2f, IMask2e> generateUnityMesh)
            : base(cellInGroupCount, gameObjectPool, group, token, pooledMesh, generateUnityMesh)
        {
        }

        protected override void UpdateGroup(ArcResource<IFrameData2i> context)
        {
            if (_token.IsCancellationRequested)
            {
                context.Dispose();
                return;
            }

            if (context.Value != null)
            {
                UpdateGroup(context, _token);
            }

            context.Dispose();
        }

        private void UpdateGroup(ArcResource<IFrameData2i> context, CancellationToken token)
        {
            if (context == null)
                return;
            UpdateTerrainMesh(context.Value);
            if (_token.IsCancellationRequested)
                return;
            UpdateUnityMesh();
        }
    }

    public class AsyncTerrainGroupGeneratorManager2i : TerrainGroupGeneratorManager2i
    {
        private Task _activeTask = Task.CompletedTask;

        private ArcResource<IFrameData2i> _queuedUpdate = null;

        public AsyncTerrainGroupGeneratorManager2i(Vector2i cellInGroupCount, ITerrainGameObject gameObjectPool, Vector2i group, CancellationToken token, ITerrainMesh pooledMesh, Action<ITerrainMesh, Vector2i, IImage2f, IMask2e> generateUnityMesh)
            : base(cellInGroupCount, gameObjectPool, group, token, pooledMesh, generateUnityMesh)
        {
        }

        protected override void UpdateGroup(ArcResource<IFrameData2i> context)
        {
            if (_token.IsCancellationRequested)
            {
                context.Dispose();
                return;
            }

            if (!_activeTask.IsCompleted)
            {
                if (_queuedUpdate != null)
                {
                    _queuedUpdate.Dispose();
                }

                _queuedUpdate = context;
                return;
            }

            _activeTask = Task.Run(async () =>
            {
                try
                {
                    await UpdateGroupAsync(context, _token);
                }
                finally
                {
                    context.Dispose();
                }
            });

            if (_queuedUpdate != null)
            {
                UpdateGroup(_queuedUpdate);
            }
        }

        private async Task UpdateGroupAsync(ArcResource<IFrameData2i> context, CancellationToken token)
        {
            if (context == null)
                return;
            UpdateTerrainMesh(context.Value);
            if (_token.IsCancellationRequested)
                return;
            await UniTask.SwitchToMainThread();
            if (_token.IsCancellationRequested)
                return;
            UpdateUnityMesh();
        }
    }

    public abstract class TerrainGroupGeneratorManager2i : ITerrainGroupGeneratorManager2i
    {
        protected readonly CancellationTokenSource _cts;
        protected readonly Action<ITerrainMesh, Vector2i, IImage2f, IMask2e> _generateUnityMesh;
        protected readonly Vector2i _group;
        protected readonly ITerrainMesh _pooledMesh;
        protected readonly Range2i _range;
        protected readonly CancellationToken _token;
        protected readonly ITerrainGameObject _gameObjectPool;

        private bool _updatedOnce;

        public TerrainGroupGeneratorManager2i(Vector2i cellInGroupCount, ITerrainGameObject gameObject, Vector2i group, CancellationToken token, ITerrainMesh pooledMesh, Action<ITerrainMesh, Vector2i, IImage2f, IMask2e> generateUnityMesh)
        {
            _gameObjectPool = gameObject;
            _group = group;
            _range = Range2i.FromMinAndSize(group * cellInGroupCount, cellInGroupCount);
            _cts = CancellationTokenSource.CreateLinkedTokenSource(token);
            _token = _cts.Token;

            _pooledMesh = pooledMesh;
            _generateUnityMesh = generateUnityMesh;
        }

        public ITerrainGameObject TerrainGameObject => _gameObjectPool;
        public ITerrainMesh Mesh => _pooledMesh;

        public void Update(ArcResource<IFrameData2i> context)
        {
            if (_token.IsCancellationRequested)
            {
                context.Dispose();
                return;
            }

            if (_updatedOnce && !context.Value.InvalidatedArea.Overlaps(_range))
            {
                context.Dispose();
                return;
            }

            _updatedOnce = true;

            UpdateGroup(context);
        }

        public virtual void Dispose()
        {
            _cts.Cancel();
        }

        protected abstract void UpdateGroup(ArcResource<IFrameData2i> context);

        protected void UpdateTerrainMesh(IFrameData2i context)
        {
            _pooledMesh.Reset(Area3f.FromMinAndSize((_group * context.CellInGroupCount).ToVector3f(context.RangeZ.Min), context.CellInGroupCount.ToVector3f(context.RangeZ.Size)));
            _generateUnityMesh(_pooledMesh, _group, context.Image, context.Mask);
            _pooledMesh.FinalizeMesh();
        }

        protected void UpdateUnityMesh()
        {
            if (_gameObjectPool.GameObject == null)
            {
                _gameObjectPool.InitializeOnMainThread();
            }

            _gameObjectPool.GameObject.SetActive(true);

            _pooledMesh.SetUnityMesh(_gameObjectPool);
        }
    }
}