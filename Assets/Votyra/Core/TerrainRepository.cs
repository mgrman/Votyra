using System;
using System.Collections.Generic;

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
        public RepositoryChange(RepositorActionType action, TKey @group, TValue mesh)
        {
            Action = action;
            Group = @group;
            Mesh = mesh;
        }

        public readonly RepositorActionType Action;
        public readonly TKey Group;
        public readonly TValue Mesh;

        public static RepositoryChange<TKey, TValue> New(TKey group, TValue mesh)
        {
            return new RepositoryChange<TKey, TValue>(RepositorActionType.New, group, mesh);
        }

        public static RepositoryChange<TKey, TValue> Changed(TKey group, TValue mesh)
        {
            return new RepositoryChange<TKey, TValue>(RepositorActionType.Changed, group, mesh);
        }

        public static RepositoryChange<TKey, TValue> Removed(TKey group, TValue mesh)
        {
            return new RepositoryChange<TKey, TValue>(RepositorActionType.Removed, group, mesh);
        }
    }

    public class TerrainRepository<TKey, TValue> : ITerrainRepository<TKey, TValue> where TKey : struct
    {
        private readonly Dictionary<TKey, TValue> _activeGroups = new Dictionary<TKey, TValue>();
        private readonly Dictionary<TKey, TValue> _lockedGroups = new Dictionary<TKey, TValue>();
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

        public bool Lock(TKey key)
        {
            lock (_activeGroupsLock)
            {
                if (_activeGroups.TryGetValue(key, out var value))
                {
                    _lockedGroups.Add(key, value);
                    return true;
                }
            }

            return false;
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

        public void Remove(TKey key)
        {
            TValue value;
            lock (_activeGroupsLock)
            {
                if (!_activeGroups.TryGetValue(key, out value))
                {
                    return;
                }

                _activeGroups.Remove(key);

                if (_lockedGroups.ContainsKey(key))
                {
                    return;
                }
            }

            _terrainChange?.Invoke(RepositoryChange<TKey, TValue>.Removed(key, value));
        }

        public void Unlock(TKey key)
        {
            RepositoryChange<TKey, TValue>? toInvoke = null;
            lock (_activeGroupsLock)
            {
                if (_activeGroups.TryGetValue(key, out var activeValue))
                {
                    toInvoke = RepositoryChange<TKey, TValue>.Changed(key, activeValue);
                }
                else if (_lockedGroups.TryGetValue(key, out var removedLockedValue))
                {
                    toInvoke = RepositoryChange<TKey, TValue>.Removed(key, removedLockedValue);
                }

                _lockedGroups.Remove(key);
            }

            if (toInvoke.HasValue)
            {
                _terrainChange?.Invoke(toInvoke.Value);
            }
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