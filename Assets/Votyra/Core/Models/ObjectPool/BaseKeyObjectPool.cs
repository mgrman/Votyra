using System;
using System.Collections.Generic;

namespace Votyra.Core.Models.ObjectPool
{
    public abstract class BaseKeyObjectPool<T, TKey> : IObjectDictionaryPool<T, TKey> where TKey : struct
    {
        private readonly int _limit;
        private readonly Func<TKey, T> _objectGenerator;

        public BaseKeyObjectPool(int limit, Func<TKey, T> objectGenerator)
        {
            if (objectGenerator == null)
            {
                throw new ArgumentNullException("objectGenerator");
            }

            this._objectGenerator = objectGenerator;

            this._limit = Math.Max(limit, 1);
        }

        public virtual T GetObject(TKey key)
        {
            var objectPool = this.GetPool(key);
            T obj;
            if (objectPool.Count > 0)
            {
                obj = objectPool[objectPool.Count - 1];
                objectPool.RemoveAt(objectPool.Count - 1);
            }
            else
            {
                obj = this._objectGenerator(key);
            }

            return obj;
        }

        public virtual void ReturnObject(T obj, TKey key)
        {
            var objectPool = this.GetPool(key);
            if (objectPool.Count < this._limit)
            {
                objectPool.Add(obj);
            }
        }

        protected abstract List<T> GetPool(TKey key);
    }
}
