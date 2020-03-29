using System;
using System.Collections.Generic;

namespace Votyra.Core.Pooling
{
    public class Pool<TValue> : IRawPool<TValue>
    {
        private readonly Func<TValue> _factory;
        private readonly List<TValue> _list = new List<TValue>();
        private readonly object _lock = new object();

        public Pool(Func<TValue> factory)
        {
            _factory = factory;
        }

        public int PoolCount { get; private set; }

        public int ActiveCount { get; private set; }

        public TValue GetRaw()
        {
            ActiveCount++;
            lock (_lock)
            {
                TValue value;
                if (_list.Count == 0)
                {
                    value = _factory();
                }
                else
                {
                    PoolCount--;
                    value = _list[_list.Count - 1];
                    _list.RemoveAt(_list.Count - 1);
                }

                return value;
            }
        }

        public void ReturnRaw(TValue value)
        {
            ActiveCount--;
            PoolCount++;
            lock (_lock)
            {
                _list.Add(value);
            }
        }
    }
}
