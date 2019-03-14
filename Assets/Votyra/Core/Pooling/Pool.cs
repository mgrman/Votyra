using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Votyra.Core.Models.ObjectPool;
using Votyra.Core.TerrainMeshes;

namespace Votyra.Core.Pooling
{
    public class Pool<TValue> : IPool<TValue> 
        where TValue : IPoolable<TValue>
    {
        private readonly object _lock=new object();
        private readonly LinkedList<TValue> _list = new LinkedList<TValue>();

        private readonly Func<TValue> _factory;

        public Pool(Func<TValue> factory)
        {
            _factory = factory;
        }

        public TValue Get()
        {
            lock (_lock)
            {
                TValue value;
                if (_list.Count == 0)
                {
                    value = _factory();
                }
                else
                {
                    value = _list.First.Value;
                    _list.RemoveFirst();
                }

                value.OnDispose += Return;
                return value;
            }
        }

        private void Return(TValue value)
        {
            value.OnDispose -= Return;
            lock (_lock)
            {
                _list.AddFirst(value);
            }
        }
    }
}