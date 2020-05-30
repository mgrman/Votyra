using System;
using System.Collections.Generic;

namespace Votyra.Core.Models.ObjectPool
{
    public class ObjectDictionaryPool<T, TKey> : BaseKeyObjectPool<T, TKey> where TKey : struct
    {
        private readonly Dictionary<TKey, List<T>> objects;

        public ObjectDictionaryPool(int limit, Func<TKey, T> objectGenerator)
            : base(limit, objectGenerator)
        {
            this.objects = new Dictionary<TKey, List<T>>();
        }

        protected override List<T> GetPool(TKey key)
        {
            List<T> objectPool;
            if (!this.objects.TryGetValue(key, out objectPool))
            {
                objectPool = new List<T>();
                this.objects[key] = objectPool;
            }

            return objectPool;
        }
    }
}
