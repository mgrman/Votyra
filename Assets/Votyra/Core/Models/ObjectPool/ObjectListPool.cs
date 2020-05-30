using System;
using System.Collections.Generic;

namespace Votyra.Core.Models.ObjectPool
{
    public class ObjectListPool<T, TKey> : BaseKeyObjectPool<T, TKey> where TKey : struct
    {
        private readonly Func<TKey, TKey, bool> comparer;
        private readonly List<Container> containers = new List<Container>();

        public ObjectListPool(int limit, Func<TKey, T> objectGenerator, Func<TKey, TKey, bool> comparer)
            : base(limit, objectGenerator)
        {
            this.comparer = comparer;
        }

        protected override List<T> GetPool(TKey key)
        {
            List<T> objectPool = null;
            foreach (var container in this.containers)
            {
                if (this.comparer(key, container.Key))
                {
                    objectPool = container.List;
                    break;
                }
            }

            if (objectPool == null)
            {
                var container = new Container(key);
                objectPool = container.List;
                this.containers.Add(container);
            }

            return objectPool;
        }

        private struct Container
        {
            public readonly TKey Key;
            public readonly List<T> List;

            public Container(TKey key)
            {
                this.Key = key;
                this.List = new List<T>();
            }
        }
    }
}
