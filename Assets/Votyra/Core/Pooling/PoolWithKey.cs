using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Votyra.Core.Models.ObjectPool;
using Votyra.Core.TerrainMeshes;

namespace Votyra.Core.Pooling
{
    public class PoolWithKey<TValue, TKey> : IPool<TValue, TKey> where TKey : struct
        where TValue : IPoolable<TValue, TKey>
    {
        private readonly object _lock=new object();
        private readonly Dictionary<TKey, LinkedList<TValue>> _dictionary = new Dictionary<TKey, LinkedList<TValue>>();
        private readonly Func<TKey, TValue> _factory;

        public int PoolCount { get; private set; }
        public int ActiveCount { get; private set; }
        
        public PoolWithKey(Func<TKey, TValue> factory)
        {
            _factory = factory;
        }

        public TValue Get(TKey key)
        {
            ActiveCount++;
            lock (_lock)
            {
                LinkedList<TValue> list;
                if (!_dictionary.TryGetValue(key, out list))
                {
                    list = new LinkedList<TValue>();
                    _dictionary[key] = list;
                }

                TValue value;
                if (list.Count == 0)
                {
                    value = _factory(key);
                }
                else
                {
                    PoolCount--;
                    value = list.First.Value;
                    list.RemoveFirst();
                }

                value.OnReturn += Return;
                return value;
            }
        }

        private void Return(TValue value)
        {
            ActiveCount--;
            PoolCount++;
            var key = value.Key;
            value.OnReturn -= Return;
            lock (_lock)
            {
                LinkedList<TValue> list;
                if (!_dictionary.TryGetValue(key, out list))
                {
                    list = new LinkedList<TValue>();
                    _dictionary[key] = list;
                }

                list.AddFirst(value);
            }
        }

    }
}