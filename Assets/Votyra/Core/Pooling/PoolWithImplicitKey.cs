using System;
using System.Collections.Generic;

namespace Votyra.Core.Pooling
{
    public class PoolWithImplicitKey<TKey, TValue> : IRawPool<TKey, TValue>
    {
        private readonly Dictionary<TKey, List<TValue>> _dictionary = new Dictionary<TKey, List<TValue>>();
        private readonly Func<TKey, TValue> _factory;
        private readonly Func<TValue, TKey> _getKey;
        private readonly object _lock = new object();

        public PoolWithImplicitKey(Func<TKey, TValue> factory, Func<TValue, TKey> getKey)
        {
            _factory = factory;
            _getKey = getKey;
        }

        public int PoolCount { get; private set; }

        public int ActiveCount { get; private set; }

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
