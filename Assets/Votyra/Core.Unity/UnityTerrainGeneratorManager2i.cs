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
        private readonly ITerrainRepository2i _manager;

        private readonly ConcurrentDictionary<Vector2i, ITerrainMesh2f> _activeTerrains = new ConcurrentDictionary<Vector2i, ITerrainMesh2f>();
        private Action<Vector2i, ITerrainMesh2f> _changedTerrain;
        private readonly Queue<RepositoryChange<Vector2i, ITerrainMesh2f>> _queue = new Queue<RepositoryChange<Vector2i, ITerrainMesh2f>>(10);
        private readonly object _queueLock = new object();

        public UnityTerrainGeneratorManager2i(ITerrainRepository2i manager)
        {
            _manager = manager;
            _manager.TerrainChange += OnTerrainChange;
        }

        public void Tick()
        {
            while (_queue.Count > 0)
            {
                var valueTuple = _queue.Dequeue();
                switch (valueTuple.Action)
                {
                    case RepositorActionType.New:
                        _newTerrain?.Invoke(valueTuple.Group, valueTuple.Mesh);
                        break;
                    case RepositorActionType.Changed:
                        _changedTerrain?.Invoke(valueTuple.Group, valueTuple.Mesh);
                        break;
                    case RepositorActionType.Removed:
                        RemovedTerrain?.Invoke(valueTuple.Group, valueTuple.Mesh);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            _queue.Clear();
        }

        public event Action<Vector2i, ITerrainMesh2f> NewTerrain
        {
            add
            {
                _newTerrain += value;
                foreach (var activeGroup in _activeTerrains)
                {
                    value?.Invoke(activeGroup.Key, activeGroup.Value);
                }
            }
            remove => _newTerrain -= value;
        }

        public event Action<Vector2i, ITerrainMesh2f> ChangedTerrain
        {
            add
            {
                _changedTerrain += value;
                foreach (var activeGroup in _activeTerrains)
                {
                    value?.Invoke(activeGroup.Key, activeGroup.Value);
                }
            }
            remove => _changedTerrain -= value;
        }

        public event Action<Vector2i, ITerrainMesh2f> RemovedTerrain;

        private event Action<Vector2i, ITerrainMesh2f> _newTerrain;

        private void OnTerrainChange(RepositoryChange<Vector2i, ITerrainMesh2f> arg)
        {
            lock (_queueLock)
            {
                switch (arg.Action)
                {
                    case RepositorActionType.New:
                        _queue.Enqueue(arg);
                        _activeTerrains.TryAdd(arg.Group, arg.Mesh);

                        break;
                    case RepositorActionType.Changed:
                        _queue.Enqueue(arg);
                        break;
                    case RepositorActionType.Removed:
                        _queue.Enqueue(arg);
                        _activeTerrains.TryRemove(arg.Group, out var _);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}
