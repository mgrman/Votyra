using System;

namespace Votyra.Core.Models.ObjectPool
{
    public class ConcurentObjectPool<T> : ObjectPool<T>
    {
        private readonly object accessLock = new object();

        public ConcurentObjectPool(int limit, Func<T> objectGenerator)
            : base(limit, objectGenerator)
        {
        }

        public override T GetObject()
        {
            lock (this.accessLock)
            {
                return base.GetObject();
            }
        }

        public override void ReturnObject(T obj)
        {
            lock (this.accessLock)
            {
                base.ReturnObject(obj);
            }
        }
    }
}
