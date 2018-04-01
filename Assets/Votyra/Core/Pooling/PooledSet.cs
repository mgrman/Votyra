using System;
using System.Collections.Generic;
using Votyra.Core.Models.ObjectPool;

namespace Votyra.Core.Pooling
{
    public class PooledSet<T> : List<T>, IReadOnlyPooledSet<T>
    {
        private static readonly bool IsDisposable = typeof(IDisposable).IsAssignableFrom(typeof(T));

        private static readonly ConcurentObjectPool<PooledSet<T>> Pool = new ConcurentObjectPool<PooledSet<T>>(5, () => new PooledSet<T>());

        private PooledSet()
        {
        }

        public static PooledSet<T> Create()
        {
            var obj = Pool.GetObject();
            obj.Clear();
            return obj;
        }

        public void Dispose()
        {
            if (IsDisposable)
            {
                foreach (var item in this)
                {
                    (item as IDisposable)?.Dispose();
                }
            }
            Pool.ReturnObject(this);
        }
    }
}