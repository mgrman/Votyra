using System;
using System.Collections.Generic;
using Votyra.Core.Logging;

namespace Votyra.Core.Pooling
{
    public class ArcPool<TValue> : IArcPool<TValue>
    {
        private readonly object _lock = new object();
        private readonly List<ArcResource<TValue>> _list = new List<ArcResource<TValue>>();
        private readonly Func<TValue> _factory;

        public int PoolCount { get; private set; }
        public int ActiveCount { get; private set; }

        public ArcPool(Func<TValue> factory)
        {
            _factory = factory;
        }

        public ArcResource<TValue> Get()
        {
            lock (_lock)
            {
                ActiveCount++;
                ArcResource<TValue> value;
                if (_list.Count == 0)
                {
                    value = new ArcResource<TValue>(_factory(), ReturnRaw);
                }
                else
                {
                    PoolCount--;
                    value = _list[_list.Count - 1];
                    _list.RemoveAt(_list.Count - 1);
                }

                value.Activate();
                return value;
            }
        }

        public void ReturnRaw(ArcResource<TValue> value)
        {
            lock (_lock)
            {
                ActiveCount--;
                PoolCount++;

                _list.Add(value);
#if UNITY_EDITOR
                if (_list.Count != PoolCount)
                {
                    StaticLogger.LogError("ArcPool in inconsistent state!");
                }
#endif
            }
        }
    }
}