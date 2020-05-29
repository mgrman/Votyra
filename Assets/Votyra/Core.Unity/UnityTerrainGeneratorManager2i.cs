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
        private readonly ConcurrentDictionary<Vector2i, ITerrainMesh2f> _activeTerrains = new ConcurrentDictionary<Vector2i, ITerrainMesh2f>();
        private readonly ITerrainRepository2i _manager;
        private readonly Queue<RepositoryChange<Vector2i, ITerrainMesh2f>> _queue = new Queue<RepositoryChange<Vector2i, ITerrainMesh2f>>(10);
        private readonly object _queueLock = new object();
        private Action<Vector2i, ITerrainMesh2f> _changedTerrain;

        public UnityTerrainGeneratorManager2i(ITerrainRepository2i manager)
        {
            this._manager = manager;
            this._manager.TerrainChange += this.OnTerrainChange;
        }

        private event Action<Vector2i, ITerrainMesh2f> _newTerrain;

        public void Tick()
        {
            while (this._queue.Count > 0)
            {
                var valueTuple = this._queue.Dequeue();
                switch (valueTuple.Action)
                {
                    case RepositorActionType.New:
                        this._activeTerrains.TryAdd(valueTuple.Group, valueTuple.Mesh);
                        this._newTerrain?.Invoke(valueTuple.Group, valueTuple.Mesh);
                        break;
                    case RepositorActionType.Changed:
                        this._changedTerrain?.Invoke(valueTuple.Group, valueTuple.Mesh);
                        break;
                    case RepositorActionType.Removed:
                        this.RemovedTerrain?.Invoke(valueTuple.Group, valueTuple.Mesh);
                        this._activeTerrains.TryRemove(valueTuple.Group, out var _);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            this._queue.Clear();
        }

        public event Action<Vector2i, ITerrainMesh2f> NewTerrain
        {
            add
            {
                this._newTerrain += value;
                foreach (var activeGroup in this._activeTerrains)
                {
                    value?.Invoke(activeGroup.Key, activeGroup.Value);
                }
            }
            remove => this._newTerrain -= value;
        }

        public event Action<Vector2i, ITerrainMesh2f> ChangedTerrain
        {
            add
            {
                this._changedTerrain += value;
                foreach (var activeGroup in this._activeTerrains)
                {
                    value?.Invoke(activeGroup.Key, activeGroup.Value);
                }
            }
            remove => this._changedTerrain -= value;
        }

        public event Action<Vector2i, ITerrainMesh2f> RemovedTerrain;

        private void OnTerrainChange(RepositoryChange<Vector2i, ITerrainMesh2f> arg)
        {
            lock (this._queueLock)
            {
                switch (arg.Action)
                {
                    case RepositorActionType.New:
                        this._queue.Enqueue(arg);

                        break;
                    case RepositorActionType.Changed:
                        this._queue.Enqueue(arg);
                        break;
                    case RepositorActionType.Removed:
                        this._queue.Enqueue(arg);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}
