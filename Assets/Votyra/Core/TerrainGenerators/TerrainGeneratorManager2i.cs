using System;
using System.Collections.Generic;
using System.Threading;
using Votyra.Core.GroupSelectors;
using Votyra.Core.Logging;
using Votyra.Core.Models;
using Votyra.Core.Pooling;
using Votyra.Core.Queueing;
using Votyra.Core.TerrainGenerators.TerrainMeshers;
using Votyra.Core.TerrainMeshes;

namespace Votyra.Core
{
    public class TerrainGeneratorManager2i : IDisposable
    {
        private readonly Vector2i _cellInGroupCount;

        private readonly IFrameDataProvider2i _frameDataProvider;

        private readonly IWorkQueue<ArcResource<IFrameData2i>> _frameWorkQueue;

        private readonly IGroupsByCameraVisibilitySelector2i _groupsByCameraVisibilitySelector2I;
        private readonly IWorkQueue<GroupUpdateData> _groupWorkQueue;
        private readonly ITerrainRepository2i _meshRepository;
        private readonly int _meshTopologyDistance;

        private readonly CancellationTokenSource _onDestroyCts = new CancellationTokenSource();

        private readonly Action<Vector2i, ArcResource<IFrameData2i>> _onGroupBecameVisibleDelegate;
        private readonly Action<Vector2i> _onGroupStoppedBeingVisibleDelegate;
        private readonly ITerrainMesher2f _terrainMesher;
        private readonly ITerrainMesh2iPool _terrainMeshPool;
        private readonly List<GroupUpdateData> _updateDateCache = new List<GroupUpdateData>();

        public TerrainGeneratorManager2i(ITerrainConfig terrainConfig, IFrameDataProvider2i frameDataProvider, IGroupsByCameraVisibilitySelector2i groupsByCameraVisibilitySelector2I, ITerrainMesh2iPool terrainMeshPool, ITerrainMesher2f terrainMesher, ITerrainRepository2i repository, IWorkQueue<ArcResource<IFrameData2i>> frameWorkQueue, IWorkQueue<GroupUpdateData> groupWorkQueue)
        {
            this._cellInGroupCount = terrainConfig.CellInGroupCount.XY();
            this._frameDataProvider = frameDataProvider;
            this._groupsByCameraVisibilitySelector2I = groupsByCameraVisibilitySelector2I;
            this._terrainMeshPool = terrainMeshPool;
            this._terrainMesher = terrainMesher;
            this._meshRepository = repository;
            this._meshRepository.TerrainChange += this.MeshRepositoryOnTerrainChange;

            this._onGroupBecameVisibleDelegate = this.OnGroupBecameVisible;
            this._onGroupStoppedBeingVisibleDelegate = this.OnGroupStoppedBeingVisible;

            this._frameWorkQueue = frameWorkQueue;
            this._frameWorkQueue.DoWork += this.EnqueueTerrainUpdates;
            this._groupWorkQueue = groupWorkQueue;
            this._groupWorkQueue.DoWork += this.UpdateGroup;

            this._frameDataProvider.FrameData += this._frameWorkQueue.QueueNew;
        }

        public void Dispose()
        {
            this._onDestroyCts.Cancel();
            this._frameWorkQueue.DoWork -= this.EnqueueTerrainUpdates;
            this._groupWorkQueue.DoWork -= this.UpdateGroup;

            this._frameDataProvider.FrameData -= this._frameWorkQueue.QueueNew;
        }

        private void MeshRepositoryOnTerrainChange(RepositoryChange<Vector2i, ITerrainMesh2f> obj)
        {
            switch (obj.Action)
            {
                case RepositorActionType.Removed:
                    this._terrainMeshPool.ReturnRaw(obj.Mesh);
                    break;
            }
        }

        private void EnqueueTerrainUpdates(ArcResource<IFrameData2i> context)
        {
            this._groupsByCameraVisibilitySelector2I.UpdateGroupsVisibility(context, this._meshRepository.ContainsKeyFunc, this._onGroupBecameVisibleDelegate, this._onGroupStoppedBeingVisibleDelegate);

            this._meshRepository.Select((group, mesh) => new GroupUpdateData(group, context, mesh, false), this._updateDateCache);
            foreach (var activeGroup in this._updateDateCache)
            {
                context.Activate();
                this._groupWorkQueue.QueueNew(activeGroup);
            }
        }

        private void OnGroupBecameVisible(Vector2i group, ArcResource<IFrameData2i> data)
        {
            var terrainMesh = this._terrainMeshPool.GetRaw();
            this._meshRepository.Add(group, terrainMesh);
            this._groupWorkQueue.QueueNew(new GroupUpdateData(group, data, terrainMesh, true));
        }

        private void UpdateGroup(GroupUpdateData data)
        {
            var context = data.Context;
            var locked = false;
            try
            {
                // TODO problem here, since by the time the method finishes the group can go out of sight.
                // maybe need to be able to "lock" the group while updating, but also the terrainMesh should not be returned to pool until this method finishes.
                var terrainMesh = data.Mesh;
                var forceUpdate = data.ForceUpdate;
                var rangeXY = Range2i.FromMinAndSize(data.Group * this._cellInGroupCount, this._cellInGroupCount);
                if (!forceUpdate && !context.Value.InvalidatedArea.Overlaps(rangeXY))
                {
                    return;
                }

                locked = this._meshRepository.Lock(data.Group);
                if (!locked)
                {
                    return;
                }

                this._terrainMesher.GetResultingMesh(terrainMesh, data.Group, context.Value.Image);
                terrainMesh.FinalizeMesh();
            }
            catch (Exception ex)
            {
                StaticLogger.LogException(ex);
                throw;
            }
            finally
            {
                context.Dispose();
                if (locked)
                {
                    this._meshRepository.Unlock(data.Group);
                }
            }
        }

        private void OnGroupStoppedBeingVisible(Vector2i group)
        {
            this._meshRepository.Remove(group);
        }
    }
}
