using System;
using System.Collections.Generic;

namespace Votyra.Core
{
    public interface ITerrainRepository<TKey, TValue> where TKey : struct
    {
        event Action<TKey, TValue> NewTerrain;
        event Action<TKey> ChangedTerrain;
        event Action<TKey> RemovedTerrain;
        void Add(TKey key, TValue value);
        TValue Remove(TKey key);
        void TriggerUpdate(TKey key);
        TValue TryGetValue(TKey key);
        void Select<TResult>(Func<TKey, TValue, TResult> func,List<TResult> cache);
        bool Contains(TKey key);
        Func<TKey, bool> ContainsKeyFunc { get; }
    }
}