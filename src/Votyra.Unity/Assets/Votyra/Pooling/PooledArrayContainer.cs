using System;
using System.Collections;
using System.Collections.Generic;
using Votyra.Common.Models.ObjectPool;

namespace Votyra.Unity.Assets.Votyra.Pooling
{
    public class PooledArrayContainer<T> : IReadOnlyPooledList<T>
    {
        public T[] Array { get; }

        private static readonly bool IsDisposable = typeof(IDisposable).IsAssignableFrom(typeof(T));

        private static readonly ConcurentObjectDictionaryPool<PooledArrayContainer<T>, int> Pool = new ConcurentObjectDictionaryPool<PooledArrayContainer<T>, int>(5, (count) => new PooledArrayContainer<T>(count));

        private PooledArrayContainer(int count)
        {
            Array = new T[count];
        }

        public static PooledArrayContainer<T> CreateDirty(int count)
        {
            var obj = Pool.GetObject(count);
            return obj;
        }

        public void Dispose()
        {
            if (IsDisposable)
            {
                foreach (var item in this.Array)
                {
                    (item as IDisposable)?.Dispose();
                }
            }
            Pool.ReturnObject(this, this.Array.Length);
        }

        #region IReadOnlyList<T>

        public int Count => ((IReadOnlyList<T>)this.Array).Count;

        public T this[int index] => ((IReadOnlyList<T>)this.Array)[index];

        public IEnumerator<T> GetEnumerator()
        {
            return ((IReadOnlyList<T>)this.Array).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IReadOnlyList<T>)this.Array).GetEnumerator();
        }
        #endregion
    }
}