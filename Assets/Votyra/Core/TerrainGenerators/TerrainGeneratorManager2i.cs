using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Votyra.Core.GroupSelectors;
using Votyra.Core.Models;
using Votyra.Core.Pooling;
using Votyra.Core.Queueing;
using Votyra.Core.TerrainMeshes;
using Votyra.Core.Utils;

namespace Votyra.Core
{
    //TODO: move to floats
    public class TerrainGeneratorManager2i : IDisposable, ITerrainGeneratorManager2i
    {
        private readonly Dictionary<Vector2i, ITerrainGroupGeneratorManager2i> _activeGroups = new Dictionary<Vector2i, ITerrainGroupGeneratorManager2i>();
        private readonly object _activeGroupsLock = new object();

        private readonly Vector2i _cellInGroupCount;
        private readonly IFrameDataProvider2i _frameDataProvider;
        private readonly int _meshTopologyDistance;

        private readonly CancellationTokenSource _onDestroyCts = new CancellationTokenSource();
        private readonly ITerrainConfig _terrainConfig;

        private readonly IGroupsByCameraVisibilitySelector2i _groupsByCameraVisibilitySelector2I;

        private Task _waitForTask = Task.CompletedTask;

        private TaskQueue<ArcResource<IFrameData2i>> _backgroundUpdateQueue;

        private event Action<Vector2i, ITerrainMesh2f> _newTerrain;

        public event Action<Vector2i, ITerrainMesh2f> NewTerrain
        {
            add
            {
                _newTerrain += value;
                foreach (var activeGroup in _activeGroups)
                {
                    value?.Invoke(activeGroup.Key, activeGroup.Value.Mesh);
                }
            }
            remove
            {
                _newTerrain -= value;
            }
        }

        public event Action<Vector2i> ChangedTerrain;
        public event Action<Vector2i> RemovedTerrain;

        public TerrainGeneratorManager2i(ITerrainConfig terrainConfig, IFrameDataProvider2i frameDataProvider, ITerrainGroupGeneratorManagerPool managerPool, IGroupsByCameraVisibilitySelector2i groupsByCameraVisibilitySelector2I)
        {
            _terrainConfig = terrainConfig;
            _cellInGroupCount = _terrainConfig.CellInGroupCount.XY();
            _frameDataProvider = frameDataProvider;
            _groupsByCameraVisibilitySelector2I = groupsByCameraVisibilitySelector2I;

            create = (g) =>
            {
                var manager = managerPool.GetRaw();
                manager.Group = g;

                _newTerrain?.Invoke(g, manager.Mesh);
                return manager;
            };

            dispose = (manager) =>
            {
                var g = manager.Group;
                manager.Stop();
                managerPool.ReturnRaw(manager);
                RemovedTerrain?.Invoke(g);
            };

            if (_terrainConfig.Async)
            {
                _backgroundUpdateQueue = new TaskQueue<ArcResource<IFrameData2i>>("Main task queue", (Action<ArcResource<IFrameData2i>>) UpdateTerrain);
                _frameDataProvider.FrameData += UpdateTerrainInBackground;
            }
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

            lock (_activeGroupsLock)
            {
                foreach (var pair in _activeGroups)
                {
                    pair.Value.Stop();
                }
            }
        }

        public ITerrainMesh2f GetMeshForGroup(Vector2i group)
        {
            lock (_activeGroupsLock)
            {
                return _activeGroups.TryGetValue(group)
                    ?.Mesh;
            }
        }

        private void UpdateTerrainInForeground(ArcResource<IFrameData2i> context)
        {
            try
            {
                UpdateTerrain(context);
            }
            finally
            {
                context.Dispose();
            }
        }

        private void UpdateTerrainInBackground(ArcResource<IFrameData2i> context)
        {
            _backgroundUpdateQueue.QueueNew(context);
        }

        private void UpdateTerrain(ArcResource<IFrameData2i> context)
        {
            HandleVisibilityUpdates(context.Value);
            lock (_activeGroupsLock)
            {
                foreach (var activeGroup in _activeGroups)
                {
                    activeGroup.Value.Update(context.Activate(), OnChangedTerrain);
                }
            }
        }

        private void OnChangedTerrain(Vector2i group)
        {
            ChangedTerrain?.Invoke(group);
        }

        private void HandleVisibilityUpdates(IFrameData2i context)
        {
            _groupsByCameraVisibilitySelector2I.UpdateGroupsVisibility(context, _cellInGroupCount, _activeGroups, _activeGroupsLock, create, dispose);
        }

        private Func<Vector2i, ITerrainGroupGeneratorManager2i> create;
        private Action<ITerrainGroupGeneratorManager2i> dispose;
    }
}