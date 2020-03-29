using System;

namespace Votyra.Core.Models.ObjectPool
{
    public class ConcurentObjectPool<T> : ObjectPool<T>
    {
        private readonly object _accessLock = new object();

        public ConcurentObjectPool(int limit, Func<T> objectGenerator)
            : base(limit, objectGenerator)
        {
        }

        public override T GetObject()
        {
            lock (_accessLock)
            {
                return base.GetObject();
            }
        }

        public override void ReturnObject(T obj)
        {
            lock (_accessLock)
            {
                base.ReturnObject(obj);
            }
        }
    }
}
