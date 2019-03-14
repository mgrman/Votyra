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

        public PoolWithKey(Func<TKey, TValue> factory)
        {
            _factory = factory;
        }

        public TValue Get(TKey key)
        {
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
                    value = list.First.Value;
                    list.RemoveFirst();
                }

                value.OnDispose += Return;
                return value;
            }
        }

        private void Return(TValue value)
        {
            var key = value.Key;
            value.OnDispose -= Return;
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