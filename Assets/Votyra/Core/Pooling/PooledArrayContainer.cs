using System;
using System.Collections;
using System.Collections.Generic;
using Votyra.Core.Models.ObjectPool;

namespace Votyra.Core.Pooling
{
    public class PooledArrayContainer<T> : IReadOnlyPooledList<T>
    {
        private static readonly bool IsDisposable = typeof(IDisposable).IsAssignableFrom(typeof(T));
        private static readonly ConcurentObjectDictionaryPool<PooledArrayContainer<T>, int> Pool = new ConcurentObjectDictionaryPool<PooledArrayContainer<T>, int>(5, count => new PooledArrayContainer<T>(count));

        private PooledArrayContainer(int count)
        {
            Array = new T[count];
        }

        public T[] Array { get; }
        public int Count => ((IReadOnlyList<T>) Array).Count;

        public T this[int index] => ((IReadOnlyList<T>) Array)[index];

        public void Dispose()
        {
            if (IsDisposable)
                foreach (var item in Array)
                {
                    (item as IDisposable)?.Dispose();
                }

            Pool.ReturnObject(this, Array.Length);
        }

        public IEnumerator<T> GetEnumerator() => ((IReadOnlyList<T>) Array).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IReadOnlyList<T>) Array).GetEnumerator();

        public static PooledArrayContainer<T> CreateDirty(int count)
        {
            var obj = Pool.GetObject(count);
            return obj;
        }
    }
}