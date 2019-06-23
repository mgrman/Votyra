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

        private IWorkQueue<ArcResource<IFrameData2i>> _frameWorkQueue;
        private IWorkQueue<Vector2i, GroupUpdateData> _groupWorkQueue;

        private event Action<Vector2i, ITerrainMesh2f> _newTerrain;
        private event Action<Vector2i> _changedTerrain;

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

        public event Action<Vector2i> ChangedTerrain
        {
            add
            {
                _changedTerrain += value;
                foreach (var activeGroup in _activeGroups)
                {
                    value?.Invoke(activeGroup.Key);
                }
            }
            remove
            {
                _changedTerrain -= value;
            }
        }

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
                managerPool.ReturnRaw(manager);
                RemovedTerrain?.Invoke(g);
            };

            if (_terrainConfig.AsyncTerrainGeneration)
            {
                _frameWorkQueue = new LastValueTaskQueue<ArcResource<IFrameData2i>>("Main task queue", UpdateTerrain);

                // TODO still problem with several updates on same group being called at the same time
                _groupWorkQueue = new ParalelTaskQueue<Vector2i,GroupUpdateData>("Main group queue", GroupUpdate);
            }
            else
            {
                _frameWorkQueue = new ImmediateQueue<ArcResource<IFrameData2i>>(UpdateTerrain);
                _groupWorkQueue = new ImmediateQueue<Vector2i, GroupUpdateData>(GroupUpdate);
            }

            _frameDataProvider.FrameData += QueueUpdateTerrain;
        }

        public void Dispose()
        {
            _onDestroyCts.Cancel();

            _frameDataProvider.FrameData -= QueueUpdateTerrain;
        }

        public ITerrainMesh2f GetMeshForGroup(Vector2i group)
        {
            lock (_activeGroupsLock)
            {
                return _activeGroups.TryGetValue(group)
                    ?.Mesh;
            }
        }

        private void QueueUpdateTerrain(ArcResource<IFrameData2i> context)
        {
            _frameWorkQueue.QueueNew(context);
        }

        private void UpdateTerrain(ArcResource<IFrameData2i> context)
        {
            HandleVisibilityUpdates(context.Value);
            lock (_activeGroupsLock)
            {
                foreach (var activeGroup in _activeGroups)
                {
                    context.Activate();
                    _groupWorkQueue.QueueNew(activeGroup.Key, new GroupUpdateData(context, activeGroup.Value));
                }
            }
        }

        private void GroupUpdate(GroupUpdateData data)
        {
            data.Manager.Update(data.Context, OnChangedTerrain);
        }

        private void OnChangedTerrain(Vector2i group)
        {
            _changedTerrain?.Invoke(group);
        }

        private void HandleVisibilityUpdates(IFrameData2i context)
        {
            _groupsByCameraVisibilitySelector2I.UpdateGroupsVisibility(context, _cellInGroupCount, _activeGroups, _activeGroupsLock, create, dispose);
        }

        private Func<Vector2i, ITerrainGroupGeneratorManager2i> create;
        private Action<ITerrainGroupGeneratorManager2i> dispose;

        private struct GroupUpdateData : IDisposable
        {
            public GroupUpdateData(ArcResource<IFrameData2i> context, ITerrainGroupGeneratorManager2i manager)
            {
                this.Context = context;
                this.Manager = manager;
            }

            public ArcResource<IFrameData2i> Context { get; }
            public ITerrainGroupGeneratorManager2i Manager { get; }

            public void Dispose()
            {
                Context?.Dispose();
            }
        }
    }
}