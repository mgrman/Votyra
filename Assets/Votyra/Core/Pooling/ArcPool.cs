using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Votyra.Core.Models.ObjectPool;
using Votyra.Core.TerrainMeshes;

namespace Votyra.Core.Pooling
{
    public class ArcPool<TValue> : IArcPool<TValue>
    {
        private readonly object _lock=new object();
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
            ActiveCount++;
            lock (_lock)
            {
                ArcResource<TValue> value;
                if (_list.Count == 0)
                {
                    value = new ArcResource<TValue>( _factory(),ReturnRaw);
                }
                else
                {
                    PoolCount--;
                    value = _list[_list.Count-1];
                    _list.RemoveAt(_list.Count-1);
                }

                value.Activate();
                return value;
            }
        }

        public void ReturnRaw(ArcResource<TValue> value)
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