using System;

namespace Votyra.Core.Models.ObjectPool
{
    public class ConcurentObjectDictionaryPool<T, TKey> : ObjectDictionaryPool<T, TKey> where TKey : struct
    {
        private readonly object _accessLock = new object();

        public ConcurentObjectDictionaryPool(int limit, Func<TKey, T> objectGenerator)
            : base(limit, objectGenerator)
        {
        }

        public override T GetObject(TKey key)
        {
            lock (this._accessLock)
            {
                return base.GetObject(key);
            }
        }

        public override void ReturnObject(T obj, TKey key)
        {
            lock (this._accessLock)
            {
                base.ReturnObject(obj, key);
            }
        }
    }
}
