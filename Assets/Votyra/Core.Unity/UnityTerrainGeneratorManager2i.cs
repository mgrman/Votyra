using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Votyra.Core.Models;
using Votyra.Core.TerrainMeshes;
using Zenject;

namespace Votyra.Core
{
    public class UnityTerrainGeneratorManager2i : ITickable, IUnityTerrainGeneratorManager2i
    {
        private readonly ConcurrentDictionary<Vector2i, ITerrainMesh2f> activeTerrains = new ConcurrentDictionary<Vector2i, ITerrainMesh2f>();
        private readonly ITerrainRepository2i manager;
        private readonly Queue<RepositoryChange<Vector2i, ITerrainMesh2f>> queue = new Queue<RepositoryChange<Vector2i, ITerrainMesh2f>>(10);
        private readonly object queueLock = new object();
        private Action<Vector2i, ITerrainMesh2f> changedTerrain;

        public UnityTerrainGeneratorManager2i(ITerrainRepository2i manager)
        {
            this.manager = manager;
            this.manager.TerrainChange += this.OnTerrainChange;
        }

        private event Action<Vector2i, ITerrainMesh2f> RawNewTerrain;

        public void Tick()
        {
            while (this.queue.Count > 0)
            {
                var valueTuple = this.queue.Dequeue();
                switch (valueTuple.Action)
                {
                    case RepositorActionType.New:
                        this.activeTerrains.TryAdd(valueTuple.Group, valueTuple.Mesh);
                        this.RawNewTerrain?.Invoke(valueTuple.Group, valueTuple.Mesh);
                        break;
                    case RepositorActionType.Changed:
                        this.changedTerrain?.Invoke(valueTuple.Group, valueTuple.Mesh);
                        break;
                    case RepositorActionType.Removed:
                        this.RemovedTerrain?.Invoke(valueTuple.Group, valueTuple.Mesh);
                        this.activeTerrains.TryRemove(valueTuple.Group, out var _);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            this.queue.Clear();
        }

        public event Action<Vector2i, ITerrainMesh2f> NewTerrain
        {
            add
            {
                this.RawNewTerrain += value;
                foreach (var activeGroup in this.activeTerrains)
                {
                    value?.Invoke(activeGroup.Key, activeGroup.Value);
                }
            }
            remove => this.RawNewTerrain -= value;
        }

        public event Action<Vector2i, ITerrainMesh2f> ChangedTerrain
        {
            add
            {
                this.changedTerrain += value;
                foreach (var activeGroup in this.activeTerrains)
                {
                    value?.Invoke(activeGroup.Key, activeGroup.Value);
                }
            }
            remove => this.changedTerrain -= value;
        }

        public event Action<Vector2i, ITerrainMesh2f> RemovedTerrain;

        private void OnTerrainChange(RepositoryChange<Vector2i, ITerrainMesh2f> arg)
        {
            lock (this.queueLock)
            {
                switch (arg.Action)
                {
                    case RepositorActionType.New:
                        this.queue.Enqueue(arg);

                        break;
                    case RepositorActionType.Changed:
                        this.queue.Enqueue(arg);
                        break;
                    case RepositorActionType.Removed:
                        this.queue.Enqueue(arg);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}
