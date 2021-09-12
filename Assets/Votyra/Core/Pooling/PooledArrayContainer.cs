using System;
using System.Collections;
using System.Collections.Generic;
using Votyra.Core.Models.ObjectPool;

namespace Votyra.Core.Pooling
{
    public static class PooledArrayContainer<T> 
    {
        private static readonly ConcurentObjectDictionaryPool<T[], int> Pool = new ConcurentObjectDictionaryPool<T[], int>(5, count => new T[count]);

        public static T[] CreateDirty(int count)
        {
            var obj = Pool.GetObject(count);
            return obj;
        }
        public static void Return(T[] array)
        {
            Pool.ReturnObject(array,array.Length);
        }
    }
}