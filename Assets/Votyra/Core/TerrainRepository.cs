using System;
using System.Collections.Generic;
using Votyra.Core.Models;
using Votyra.Core.TerrainGenerators.TerrainMeshers;
using Votyra.Core.TerrainMeshes;

namespace Votyra.Core
{
    public enum RepositorActionType
    {
        New,
        Changed,
        Removed
    }
    public struct RepositoryChange<TKey, TValue>
    {
        public RepositoryChange(RepositorActionType action, TKey @group, TValue newMesh)
            Action = action;
            Group = @group;
            NewMesh = newMesh;
        }

        public readonly RepositorActionType Action;
        public readonly TKey Group;
        public readonly TValue NewMesh;

        public static RepositoryChange<TKey, TValue> New(TKey group, TValue newMesh)
        {
            return new RepositoryChange<TKey, TValue>(RepositorActionType.New, group, newMesh);
        }

        public static RepositoryChange<TKey, TValue> Changed(TKey group)
        {
            return new RepositoryChange<TKey, TValue>(RepositorActionType.Changed, group, default);
        }

        public static RepositoryChange<TKey, TValue> Removed(TKey group)
        {
            return new RepositoryChange<TKey, TValue>(RepositorActionType.Removed, group, default);
        }
    }
    
    public class TerrainRepository<TKey, TValue> : ITerrainRepository<TKey, TValue> where TKey : struct
    {
        private readonly Dictionary<TKey, TValue> _activeGroups = new Dictionary<TKey, TValue>();
        private readonly object _activeGroupsLock = new object();

        public TerrainRepository()
        {
            ContainsKeyFunc = Contains;
        }

        private event Action<RepositoryChange<TKey, TValue>> _terrainChange;

        public event Action<RepositoryChange<TKey, TValue>> TerrainChange
        {
            add
            {
                _terrainChange += value;
                foreach (var activeGroup in _activeGroups)
                {
                    value?.Invoke(RepositoryChange<TKey, TValue>.New(activeGroup.Key, activeGroup.Value));
                }
            }
            remove
            {
                _terrainChange -= value;
            }
        }

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

            _terrainChange?.Invoke(RepositoryChange<TKey, TValue>.New(key, value));
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

            _terrainChange?.Invoke(RepositoryChange<TKey, TValue>.Removed(key));
            return value;
        }

        public void TriggerUpdate(TKey key)
        {
            _terrainChange?.Invoke(RepositoryChange<TKey, TValue>.Changed(key));
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