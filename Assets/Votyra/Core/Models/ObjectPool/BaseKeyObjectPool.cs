using System;
using System.Collections.Generic;
using UnityEngine.Profiling;

namespace Votyra.Core.Models.ObjectPool
{
    public abstract class BaseKeyObjectPool<T, TKey> : IObjectDictionaryPool<T, TKey> where TKey : struct
    {
        private readonly int _limit;
        private readonly Func<TKey, T> _objectGenerator;

        public BaseKeyObjectPool(int limit, Func<TKey, T> objectGenerator)
        {
            if (objectGenerator == null)
                throw new ArgumentNullException("objectGenerator");
            _objectGenerator = objectGenerator;

            _limit = Math.Max(limit, 1);
        }

        public virtual T GetObject(TKey key)
        {
            var objectPool = GetPool(key);
            T obj;
            if (objectPool.Count > 0)
            {
                Profiler.BeginSample("removing from pool");
                obj = objectPool[objectPool.Count - 1];
                objectPool.RemoveAt(objectPool.Count - 1);
                Profiler.EndSample();
            }
            else
            {
                Profiler.BeginSample("creating new");
                obj = _objectGenerator(key);
                Profiler.EndSample();
            }

            return obj;
        }

        public virtual void ReturnObject(T obj, TKey key)
        {
            var objectPool = GetPool(key);
            if (objectPool.Count < _limit)
                objectPool.Add(obj);
        }

        protected abstract List<T> GetPool(TKey key);
    }
}