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
    public class TerrainGeneratorManager2I : IDisposable
    {
        private readonly Vector2i cellInGroupCount;

        private readonly IFrameDataProvider2I frameDataProvider;

        private readonly IWorkQueue<ArcResource<IFrameData2I>> frameWorkQueue;

        private readonly IGroupsByCameraVisibilitySelector2I groupsByCameraVisibilitySelector2I;
        private readonly IWorkQueue<GroupUpdateData> groupWorkQueue;
        private readonly ITerrainRepository2I meshRepository;
        private readonly CancellationTokenSource onDestroyCts = new CancellationTokenSource();

        private readonly Action<Vector2i, ArcResource<IFrameData2I>> onGroupBecameVisibleDelegate;
        private readonly Action<Vector2i> onGroupStoppedBeingVisibleDelegate;
        private readonly ITerrainMesher2F terrainMesher;
        private readonly ITerrainMesh2IPool terrainMeshPool;
        private readonly List<GroupUpdateData> updateDateCache = new List<GroupUpdateData>();

        public TerrainGeneratorManager2I(ITerrainConfig terrainConfig, IFrameDataProvider2I frameDataProvider, IGroupsByCameraVisibilitySelector2I groupsByCameraVisibilitySelector2I, ITerrainMesh2IPool terrainMeshPool, ITerrainMesher2F terrainMesher, ITerrainRepository2I repository, IWorkQueue<ArcResource<IFrameData2I>> frameWorkQueue, IWorkQueue<GroupUpdateData> groupWorkQueue)
        {
            this.cellInGroupCount = terrainConfig.CellInGroupCount.XY();
            this.frameDataProvider = frameDataProvider;
            this.groupsByCameraVisibilitySelector2I = groupsByCameraVisibilitySelector2I;
            this.terrainMeshPool = terrainMeshPool;
            this.terrainMesher = terrainMesher;
            this.meshRepository = repository;
            this.meshRepository.TerrainChange += this.MeshRepositoryOnTerrainChange;

            this.onGroupBecameVisibleDelegate = this.OnGroupBecameVisible;
            this.onGroupStoppedBeingVisibleDelegate = this.OnGroupStoppedBeingVisible;

            this.frameWorkQueue = frameWorkQueue;
            this.frameWorkQueue.DoWork += this.EnqueueTerrainUpdates;
            this.groupWorkQueue = groupWorkQueue;
            this.groupWorkQueue.DoWork += this.UpdateGroup;

            this.frameDataProvider.FrameData += this.frameWorkQueue.QueueNew;
        }

        public void Dispose()
        {
            this.onDestroyCts.Cancel();
            this.frameWorkQueue.DoWork -= this.EnqueueTerrainUpdates;
            this.groupWorkQueue.DoWork -= this.UpdateGroup;

            this.frameDataProvider.FrameData -= this.frameWorkQueue.QueueNew;
        }

        private void MeshRepositoryOnTerrainChange(RepositoryChange<Vector2i, ITerrainMesh2F> obj)
        {
            switch (obj.Action)
            {
                case RepositorActionType.Removed:
                    this.terrainMeshPool.ReturnRaw(obj.Mesh);
                    break;
            }
        }

        private void EnqueueTerrainUpdates(ArcResource<IFrameData2I> context)
        {
            this.groupsByCameraVisibilitySelector2I.UpdateGroupsVisibility(context, this.meshRepository.ContainsKeyFunc, this.onGroupBecameVisibleDelegate, this.onGroupStoppedBeingVisibleDelegate);

            this.meshRepository.Select((group, mesh) => new GroupUpdateData(group, context, mesh, false), this.updateDateCache);
            foreach (var activeGroup in this.updateDateCache)
            {
                context.Activate();
                this.groupWorkQueue.QueueNew(activeGroup);
            }
        }

        private void OnGroupBecameVisible(Vector2i group, ArcResource<IFrameData2I> data)
        {
            var terrainMesh = this.terrainMeshPool.GetRaw();
            this.meshRepository.Add(group, terrainMesh);
            this.groupWorkQueue.QueueNew(new GroupUpdateData(group, data, terrainMesh, true));
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
                var rangeXy = Range2i.FromMinAndSize(data.Group * this.cellInGroupCount, this.cellInGroupCount);
                if (!forceUpdate && !context.Value.InvalidatedArea.Overlaps(rangeXy))
                {
                    return;
                }

                locked = this.meshRepository.Lock(data.Group);
                if (!locked)
                {
                    return;
                }

                this.terrainMesher.GetResultingMesh(terrainMesh, data.Group, context.Value.Image);
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
                    this.meshRepository.Unlock(data.Group);
                }
            }
        }

        private void OnGroupStoppedBeingVisible(Vector2i group)
        {
            this.meshRepository.Remove(group);
        }
    }
}