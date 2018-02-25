using System;
using System.Collections.Generic;
using Votyra.Core.Models.ObjectPool;

namespace Votyra.Core.Pooling
{
    public class PooledList<T> : List<T>, IReadOnlyPooledList<T>
    {
        private static readonly bool IsDisposable = typeof(IDisposable).IsAssignableFrom(typeof(T));

        private static readonly ConcurentObjectPool<PooledList<T>> Pool = new ConcurentObjectPool<PooledList<T>>(5, () => new PooledList<T>());

        private PooledList()
        {
        }

        public static PooledList<T> Create()
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