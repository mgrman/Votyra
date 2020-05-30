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
        private readonly Dictionary<TKey, TValue> activeGroups = new Dictionary<TKey, TValue>();
        private readonly object activeGroupsLock = new object();
        private readonly Dictionary<TKey, TValue> lockedGroups = new Dictionary<TKey, TValue>();

        public TerrainRepository()
        {
            this.ContainsKeyFunc = this.Contains;
        }

        private event Action<RepositoryChange<TKey, TValue>> RawTerrainChange;

        public event Action<RepositoryChange<TKey, TValue>> TerrainChange
        {
            add
            {
                this.RawTerrainChange += value;
                foreach (var activeGroup in this.activeGroups)
                {
                    value?.Invoke(RepositoryChange<TKey, TValue>.New(activeGroup.Key, activeGroup.Value));
                }
            }
            remove => this.RawTerrainChange -= value;
        }

        public bool Contains(TKey key)
        {
            lock (this.activeGroupsLock)
            {
                return this.activeGroups.ContainsKey(key);
            }
        }

        public bool Lock(TKey key)
        {
            lock (this.activeGroupsLock)
            {
                if (this.activeGroups.TryGetValue(key, out var value))
                {
                    this.lockedGroups.Add(key, value);
                    return true;
                }
            }

            return false;
        }

        public Func<TKey, bool> ContainsKeyFunc { get; }

        public void Add(TKey key, TValue value)
        {
            lock (this.activeGroupsLock)
            {
                this.activeGroups.Add(key, value);
            }

            this.RawTerrainChange?.Invoke(RepositoryChange<TKey, TValue>.New(key, value));
        }

        public void Remove(TKey key)
        {
            TValue value;
            lock (this.activeGroupsLock)
            {
                if (!this.activeGroups.TryGetValue(key, out value))
                {
                    return;
                }

                this.activeGroups.Remove(key);

                if (this.lockedGroups.ContainsKey(key))
                {
                    return;
                }
            }

            this.RawTerrainChange?.Invoke(RepositoryChange<TKey, TValue>.Removed(key, value));
        }

        public void Unlock(TKey key)
        {
            RepositoryChange<TKey, TValue>? toInvoke = null;
            lock (this.activeGroupsLock)
            {
                if (this.activeGroups.TryGetValue(key, out var activeValue))
                {
                    toInvoke = RepositoryChange<TKey, TValue>.Changed(key, activeValue);
                }
                else if (this.lockedGroups.TryGetValue(key, out var removedLockedValue))
                {
                    toInvoke = RepositoryChange<TKey, TValue>.Removed(key, removedLockedValue);
                }

                this.lockedGroups.Remove(key);
            }

            if (toInvoke.HasValue)
            {
                this.RawTerrainChange?.Invoke(toInvoke.Value);
            }
        }

        public TValue TryGetValue(TKey key)
        {
            TValue value;
            lock (this.activeGroupsLock)
            {
                if (!this.activeGroups.TryGetValue(key, out value))
                {
                    return default;
                }
            }

            return value;
        }

        public void Select<TResult>(Func<TKey, TValue, TResult> func, List<TResult> cache)
        {
            lock (this.activeGroupsLock)
            {
                if (cache.Capacity < this.activeGroups.Count)
                {
                    cache.Capacity = this.activeGroups.Count;
                }

                cache.Clear();

                foreach (var activeGroup in this.activeGroups)
                {
                    cache.Add(func(activeGroup.Key, activeGroup.Value));
                }
            }
        }
    }
}
