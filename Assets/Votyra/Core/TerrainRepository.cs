using System;
using System.Collections.Generic;

namespace Votyra.Core
{
    public enum RepositorActionType
    {
        New,
        Changed,
        Removed,
    }

    public struct RepositoryChange<TKey, TValue>
    {
        public RepositoryChange(RepositorActionType action, TKey group, TValue mesh)
        {
            this.Action = action;
            this.Group = group;
            this.Mesh = mesh;
        }

        public readonly RepositorActionType Action;
        public readonly TKey Group;
        public readonly TValue Mesh;

        public static RepositoryChange<TKey, TValue> New(TKey group, TValue mesh) => new RepositoryChange<TKey, TValue>(RepositorActionType.New, group, mesh);

        public static RepositoryChange<TKey, TValue> Changed(TKey group, TValue mesh) => new RepositoryChange<TKey, TValue>(RepositorActionType.Changed, group, mesh);

        public static RepositoryChange<TKey, TValue> Removed(TKey group, TValue mesh) => new RepositoryChange<TKey, TValue>(RepositorActionType.Removed, group, mesh);
    }

    public class TerrainRepository<TKey, TValue> : ITerrainRepository<TKey, TValue> where TKey : struct
    {
        private readonly Dictionary<TKey, TValue> _activeGroups = new Dictionary<TKey, TValue>();
        private readonly object _activeGroupsLock = new object();
        private readonly Dictionary<TKey, TValue> _lockedGroups = new Dictionary<TKey, TValue>();

        public TerrainRepository()
        {
            this.ContainsKeyFunc = this.Contains;
        }

        private event Action<RepositoryChange<TKey, TValue>> _terrainChange;

        public event Action<RepositoryChange<TKey, TValue>> TerrainChange
        {
            add
            {
                this._terrainChange += value;
                foreach (var activeGroup in this._activeGroups)
                {
                    value?.Invoke(RepositoryChange<TKey, TValue>.New(activeGroup.Key, activeGroup.Value));
                }
            }
            remove => this._terrainChange -= value;
        }

        public bool Contains(TKey key)
        {
            lock (this._activeGroupsLock)
            {
                return this._activeGroups.ContainsKey(key);
            }
        }

        public bool Lock(TKey key)
        {
            lock (this._activeGroupsLock)
            {
                if (this._activeGroups.TryGetValue(key, out var value))
                {
                    this._lockedGroups.Add(key, value);
                    return true;
                }
            }

            return false;
        }

        public Func<TKey, bool> ContainsKeyFunc { get; }

        public void Add(TKey key, TValue value)
        {
            lock (this._activeGroupsLock)
            {
                this._activeGroups.Add(key, value);
            }

            this._terrainChange?.Invoke(RepositoryChange<TKey, TValue>.New(key, value));
        }

        public void Remove(TKey key)
        {
            TValue value;
            lock (this._activeGroupsLock)
            {
                if (!this._activeGroups.TryGetValue(key, out value))
                {
                    return;
                }

                this._activeGroups.Remove(key);

                if (this._lockedGroups.ContainsKey(key))
                {
                    return;
                }
            }

            this._terrainChange?.Invoke(RepositoryChange<TKey, TValue>.Removed(key, value));
        }

        public void Unlock(TKey key)
        {
            RepositoryChange<TKey, TValue>? toInvoke = null;
            lock (this._activeGroupsLock)
            {
                if (this._activeGroups.TryGetValue(key, out var activeValue))
                {
                    toInvoke = RepositoryChange<TKey, TValue>.Changed(key, activeValue);
                }
                else if (this._lockedGroups.TryGetValue(key, out var removedLockedValue))
                {
                    toInvoke = RepositoryChange<TKey, TValue>.Removed(key, removedLockedValue);
                }

                this._lockedGroups.Remove(key);
            }

            if (toInvoke.HasValue)
            {
                this._terrainChange?.Invoke(toInvoke.Value);
            }
        }

        public TValue TryGetValue(TKey key)
        {
            TValue value;
            lock (this._activeGroupsLock)
            {
                if (!this._activeGroups.TryGetValue(key, out value))
                {
                    return default;
                }
            }

            return value;
        }

        public void Select<TResult>(Func<TKey, TValue, TResult> func, List<TResult> cache)
        {
            lock (this._activeGroupsLock)
            {
                if (cache.Capacity < this._activeGroups.Count)
                {
                    cache.Capacity = this._activeGroups.Count;
                }

                cache.Clear();

                foreach (var activeGroup in this._activeGroups)
                {
                    cache.Add(func(activeGroup.Key, activeGroup.Value));
                }
            }
        }
    }
}
