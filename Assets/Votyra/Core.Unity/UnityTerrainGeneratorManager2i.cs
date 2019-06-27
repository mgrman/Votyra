using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UniRx.Async;
using Votyra.Core.GroupSelectors;
using Votyra.Core.Models;
using Votyra.Core.Pooling;
using Votyra.Core.Queueing;
using Votyra.Core.TerrainMeshes;
using Votyra.Core.Utils;
using Zenject;

namespace Votyra.Core
{
    public class UnityTerrainGeneratorManager2i : ITickable, IUnityTerrainGeneratorManager2i
    {
        private Queue<RepositoryChange<Vector2i, ITerrainMesh2f>> _queue = new Queue<RepositoryChange<Vector2i, ITerrainMesh2f>>(10);
        private object _queueLock = new object();
        private readonly ITerrainRepository2i _manager;

        private Dictionary<Vector2i, ITerrainMesh2f> _activeTerrains = new Dictionary<Vector2i, ITerrainMesh2f>();

        private event Action<Vector2i, ITerrainMesh2f> _newTerrain;
        private Action<Vector2i> _changedTerrain;

        private object _activeTerrainsLock => (_activeTerrains as ICollection).SyncRoot;

        public UnityTerrainGeneratorManager2i(ITerrainRepository2i manager)
        {
            _manager = manager;
            _manager.TerrainChange += OnTerrainChange;
        }

        private void OnTerrainChange(RepositoryChange<Vector2i, ITerrainMesh2f> arg)
        {
            lock (_queueLock)
            {
                switch (arg.Action)
                {
                    case RepositorActionType.New:
                        _queue.Enqueue(arg);
                        lock (_activeTerrainsLock)
                        {
                            _activeTerrains.Add(arg.Group, arg.Mesh);
                        }

                        break;
                    case RepositorActionType.Changed:
                        _queue.Enqueue(arg);
                        break;
                    case RepositorActionType.Removed:
                        _queue.Enqueue(arg);
                        lock (_activeTerrainsLock)
                        {
                            _activeTerrains.Remove(arg.Group);
                        }

                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
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
                        _changedTerrain?.Invoke(valueTuple.Group);
                        break;
                    case RepositorActionType.Removed:
                        RemovedTerrain?.Invoke(valueTuple.Group);
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
                foreach (var activeGroup in _activeTerrains)
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
    }
}