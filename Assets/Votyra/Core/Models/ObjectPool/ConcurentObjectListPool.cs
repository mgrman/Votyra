using System;

namespace Votyra.Core.Models.ObjectPool
{
    public class ConcurentObjectListPool<T, TKey> : ObjectListPool<T, TKey>
        where TKey : struct
    {
        private readonly object accessLock = new object();

        public ConcurentObjectListPool(int limit, Func<TKey, T> objectGenerator, Func<TKey, TKey, bool> comparer)
            : base(limit, objectGenerator, comparer)
        {
        }

        public override T GetObject(TKey key)
        {
            lock (this.accessLock)
            {
                return base.GetObject(key);
            }
        }

        public override void ReturnObject(T obj, TKey key)
        {
            lock (this.accessLock)
            {
                base.ReturnObject(obj, key);
            }
        }
    }
}
