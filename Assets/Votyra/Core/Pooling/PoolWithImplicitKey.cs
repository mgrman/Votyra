using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Votyra.Core.Models.ObjectPool;
using Votyra.Core.TerrainMeshes;

namespace Votyra.Core.Pooling
{
    public class PoolWithImplicitKey<TKey, TValue> : IRawPool<TKey, TValue>
    {
        private readonly object _lock=new object();
        private readonly Dictionary<TKey, List<TValue>> _dictionary = new Dictionary<TKey, List<TValue>>();
        private readonly Func<TKey, TValue> _factory;
        private readonly Func<TValue, TKey> _getKey;


        public int PoolCount { get; private set; }
        public int ActiveCount { get; private set; }
        
        public PoolWithImplicitKey(Func<TKey, TValue> factory,Func<TValue,TKey> getKey)
        {
            _factory = factory;
            _getKey = getKey;
        }
        
        public TValue GetRaw(TKey key)
        {
            ActiveCount++;
            lock (_lock)
            {
                List<TValue> list;
                if (!_dictionary.TryGetValue(key, out list))
                {
                    list = new List<TValue>();
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
                    value = list[list.Count - 1];
                    list.RemoveAt(list.Count - 1);
                }

                return value;
            }
        }

        public void ReturnRaw(TValue value)
        {
            ActiveCount--;
            PoolCount++;
            var key = _getKey(value);
            lock (_lock)
            {
                List<TValue> list;
                if (!_dictionary.TryGetValue(key, out list))
                {
                    list = new List<TValue>();
                    _dictionary[key] = list;
                }

                list.Add(value);
            }
        }

    }
}