using System;
using System.Collections.Generic;

namespace Votyra.Core.Models.ObjectPool
{
    public class ObjectListPool<T, TKey> : BaseKeyObjectPool<T, TKey>
        where TKey : struct
    {
        private readonly Func<TKey, TKey, bool> _comparer;
        private readonly List<Container> _containers = new List<Container>();

        public ObjectListPool(int limit, Func<TKey, T> objectGenerator, Func<TKey, TKey, bool> comparer)
            : base(limit, objectGenerator)
        {
            _comparer = comparer;
        }

        protected override List<T> GetPool(TKey key)
        {
            List<T> objectPool = null;
            foreach (var container in _containers)
            {
                if (_comparer(key, container.key))
                {
                    objectPool = container.list;
                    break;
                }
            }
            if (objectPool == null)
            {
                var container = new Container(key);
                objectPool = container.list;
                _containers.Add(container);
            }
            return objectPool;
        }

        private struct Container
        {
            public readonly TKey key;
            public readonly List<T> list;

            public Container(TKey key)
            {
                this.key = key;
                this.list = new List<T>();
            }
        }
    }
}