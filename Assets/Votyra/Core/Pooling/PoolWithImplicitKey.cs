using System;
using System.Collections.Generic;

namespace Votyra.Core.Pooling
{
    public class PoolWithImplicitKey<TKey, TValue> : IRawPool<TKey, TValue>
    {
        private readonly Dictionary<TKey, List<TValue>> dictionary = new Dictionary<TKey, List<TValue>>();
        private readonly Func<TKey, TValue> factory;
        private readonly Func<TValue, TKey> getKey;
        private readonly object @lock = new object();

        public PoolWithImplicitKey(Func<TKey, TValue> factory, Func<TValue, TKey> getKey)
        {
            this.factory = factory;
            this.getKey = getKey;
        }

        public int PoolCount { get; private set; }

        public int ActiveCount { get; private set; }

        public TValue GetRaw(TKey key)
        {
            this.ActiveCount++;
            lock (this.@lock)
            {
                List<TValue> list;
                if (!this.dictionary.TryGetValue(key, out list))
                {
                    list = new List<TValue>();
                    this.dictionary[key] = list;
                }

                TValue value;
                if (list.Count == 0)
                {
                    value = this.factory(key);
                }
                else
                {
                    this.PoolCount--;
                    value = list[list.Count - 1];
                    list.RemoveAt(list.Count - 1);
                }

                return value;
            }
        }

        public void ReturnRaw(TValue value)
        {
            this.ActiveCount--;
            this.PoolCount++;
            var key = this.getKey(value);
            lock (this.@lock)
            {
                List<TValue> list;
                if (!this.dictionary.TryGetValue(key, out list))
                {
                    list = new List<TValue>();
                    this.dictionary[key] = list;
                }

                list.Add(value);
            }
        }
    }
}
