using System;
using System.Collections;
using System.Collections.Generic;
using Votyra.Core.Models.ObjectPool;

namespace Votyra.Core.Pooling
{
    public class PooledArray<T> : PoolWithKey<ArrayContainer<T>,int>
    {
        public PooledArray()
            : base(Create)
        {
        }

        private static ArrayContainer<T> Create(int size)
        {
            return new ArrayContainer<T>(size);
        }
    }

    public class ArrayContainer<T> : IPoolable<ArrayContainer<T>, int>
    {
        public ArrayContainer(int size)
        {
            Array = new T[size];
        }

        public T[] Array { get; }
        public int Key => Array.Length;
        
        public void Dispose()
        {
            OnDispose?.Invoke(this);
        }

        public event Action<ArrayContainer<T>> OnDispose;
    }
}