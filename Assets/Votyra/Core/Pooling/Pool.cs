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

        public int PoolCount { get; private set; }
        public int ActiveCount { get; private set; }
        
        public Pool(Func<TValue> factory)
        {
            _factory = factory;
        }

        public TValue Get()
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
                    value = _list.First.Value;
                    _list.RemoveFirst();
                }

                value.OnReturn += Return;
                return value;
            }
        }

        private void Return(TValue value)
        {
            ActiveCount--;
            PoolCount++;
            value.OnReturn -= Return;
            lock (_lock)
            {
                _list.AddFirst(value);
            }
        }
    }
}