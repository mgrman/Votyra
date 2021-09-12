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
            ArrayEnumerable = Array;
        }

        public T[] Array { get; }

        private IEnumerable<T> ArrayEnumerable;
        
        public int Count =>  Array.Length;

        public T this[int index] =>  Array[index];

        public void Dispose()
        {
            if (IsDisposable)
                foreach (var item in Array)
                {
                    (item as IDisposable)?.Dispose();
                }

            Pool.ReturnObject(this, Array.Length);
        }

        public IEnumerator<T> GetEnumerator() => ArrayEnumerable.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => Array.GetEnumerator();

        public static PooledArrayContainer<T> CreateDirty(int count)
        {
            var obj = Pool.GetObject(count);
            return obj;
        }
    }
}