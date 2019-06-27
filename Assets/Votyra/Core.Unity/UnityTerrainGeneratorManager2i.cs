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
        private enum ActionType
        {
            New,
            Changed,
            Removed
        }

        private Queue<(ActionType, Vector2i, ITerrainMesh2f)> _queue = new Queue<(ActionType, Vector2i, ITerrainMesh2f)>(10);
        private readonly ITerrainRepository2i _manager;

        private Dictionary<Vector2i, ITerrainMesh2f> _activeTerrains = new Dictionary<Vector2i, ITerrainMesh2f>();

        private event Action<Vector2i, ITerrainMesh2f> _newTerrain;
        private Action<Vector2i> _changedTerrain;

        private object _activeTerrainsLock => (_activeTerrains as ICollection).SyncRoot;

        public UnityTerrainGeneratorManager2i(ITerrainRepository2i manager)
        {
            _manager = manager;
            _manager.NewTerrain += OnNewTerrain;
            _manager.ChangedTerrain += OnChangedTerrain;
            _manager.RemovedTerrain += OnRemovedTerrain;
        }

        private void OnRemovedTerrain(Vector2i arg1)
        {
            _queue.Enqueue((ActionType.Removed, arg1, null));
            lock (_activeTerrainsLock)
            {
                _activeTerrains.Remove(arg1);
            }
        }

        private void OnChangedTerrain(Vector2i arg1)
        {
            _queue.Enqueue((ActionType.Changed, arg1, null));
        }

        private void OnNewTerrain(Vector2i arg1, ITerrainMesh2f arg2)
        {
            _queue.Enqueue((ActionType.New, arg1, arg2));
            lock (_activeTerrainsLock)
            {
                _activeTerrains.Add(arg1, arg2);
            }
        }

        public void Tick()
        {
            while (_queue.Count > 0)
            {
                var valueTuple = _queue.Dequeue();
                switch (valueTuple.Item1)
                {
                    case ActionType.New:
                        _newTerrain?.Invoke(valueTuple.Item2, valueTuple.Item3);
                        break;
                    case ActionType.Changed:
                        _changedTerrain?.Invoke(valueTuple.Item2);
                        break;
                    case ActionType.Removed:
                        RemovedTerrain?.Invoke(valueTuple.Item2);
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