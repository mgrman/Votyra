using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Votyra.Core.Models;
using Votyra.Core.TerrainMeshes;
using Zenject;

namespace Votyra.Core
{
    public class UnityTerrainGeneratorManager2I : ITickable, IUnityTerrainGeneratorManager2I
    {
        private readonly ConcurrentDictionary<Vector2i, ITerrainMesh2F> activeTerrains = new ConcurrentDictionary<Vector2i, ITerrainMesh2F>();
        private readonly ITerrainRepository2I manager;
        private readonly Queue<RepositoryChange<Vector2i, ITerrainMesh2F>> queue = new Queue<RepositoryChange<Vector2i, ITerrainMesh2F>>(10);
        private readonly object queueLock = new object();
        private Action<Vector2i, ITerrainMesh2F> changedTerrain;

        public UnityTerrainGeneratorManager2I(ITerrainRepository2I manager)
        {
            this.manager = manager;
            this.manager.TerrainChange += this.OnTerrainChange;
        }

        private event Action<Vector2i, ITerrainMesh2F> RawNewTerrain;

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

        public event Action<Vector2i, ITerrainMesh2F> NewTerrain
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

        public event Action<Vector2i, ITerrainMesh2F> ChangedTerrain
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

        public event Action<Vector2i, ITerrainMesh2F> RemovedTerrain;

        private void OnTerrainChange(RepositoryChange<Vector2i, ITerrainMesh2F> arg)
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
