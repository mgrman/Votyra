using System;
using System.Collections.Generic;
using Votyra.Common.Models.ObjectPool;

namespace Votyra.Unity.Assets.Votyra.Pooling
{
    public class PooledList<T> : List<T>, IReadOnlyPooledCollection<T>, IReadOnlyPooledList<T>
    {
        private static readonly ConcurentObjectPool<PooledList<T>> Pool = new ConcurentObjectPool<PooledList<T>>(5, () => new PooledList<T>());

        private PooledList()
        {

        }

        public static PooledList<T> Create()
        {
            return Pool.GetObject();
        }

        public void Dispose()
        {
            this.Clear();
            Pool.ReturnObject(this);
        }
    }
}