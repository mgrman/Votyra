using System;
using System.Collections.Generic;
using Votyra.Core.Models;
using Votyra.Core.TerrainGenerators.TerrainMeshers;
using Votyra.Core.TerrainMeshes;

namespace Votyra.Core
{
    public class TerrainRepository<TKey, TValue> : ITerrainRepository<TKey, TValue> where TKey : struct
    {
        private readonly Dictionary<TKey, TValue> _activeGroups = new Dictionary<TKey, TValue>();
        private readonly object _activeGroupsLock = new object();

        public TerrainRepository()
        {
            ContainsKeyFunc = Contains;
        }

        private event Action<TKey, TValue> _newTerrain;
        private event Action<TKey> _changedTerrain;

        public event Action<TKey, TValue> NewTerrain
        {
            add
            {
                _newTerrain += value;
                foreach (var activeGroup in _activeGroups)
                {
                    value?.Invoke(activeGroup.Key, activeGroup.Value);
                }
            }
            remove
            {
                _newTerrain -= value;
            }
        }

        public event Action<TKey> ChangedTerrain
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

        public event Action<TKey> RemovedTerrain;

        public bool Contains(TKey key)
        {
            lock (_activeGroupsLock)
            {
                return _activeGroups.ContainsKey(key);
            }
        }

        public Func<TKey, bool> ContainsKeyFunc { get; }

        public void Add(TKey key, TValue value)
        {
            lock (_activeGroupsLock)
            {
                _activeGroups.Add(key, value);
            }

            _newTerrain?.Invoke(key, value);
        }

        public TValue Remove(TKey key)
        {
            TValue value;
            lock (_activeGroupsLock)
            {
                if (!_activeGroups.TryGetValue(key, out value))
                {
                    return default;
                }

                _activeGroups.Remove(key);
            }

            RemovedTerrain?.Invoke(key);
            return value;
        }

        public void TriggerUpdate(TKey key)
        {
            _changedTerrain?.Invoke(key);
        }

        public TValue TryGetValue(TKey key)
        {
            TValue value;
            lock (_activeGroupsLock)
            {
                if (!_activeGroups.TryGetValue(key, out value))
                {
                    return default;
                }
            }

            return value;
        }

        public void Select<TResult>(Func<TKey, TValue, TResult> func, List<TResult> cache)
        {
            lock (_activeGroupsLock)
            {
                if (cache.Capacity < _activeGroups.Count)
                {
                    cache.Capacity = _activeGroups.Count;
                }

                cache.Clear();

                foreach (var activeGroup in _activeGroups)
                {
                    cache.Add(func(activeGroup.Key, activeGroup.Value));
                }
            }
        }
    }
}