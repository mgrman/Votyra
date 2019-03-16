using System;
using UnityEngine;

namespace Votyra.Core.Pooling
{
    public interface IPool<out TValue, in TKey> : IPool where TKey : struct
        where TValue : IPoolable<TValue, TKey>
    {
        TValue Get(TKey arg);
    }

    public interface IPoolable<out TValue, out TKey> : IPoolable<TValue> where TKey : struct
        where TValue : IPoolable<TValue, TKey>
    {
        TKey Key { get; }
    }


    public interface IPool<out TValue> : IPool where TValue : IPoolable<TValue>
    {
        TValue Get();
    }

    public interface IPoolable<out TValue>
        where TValue : IPoolable<TValue>
    {
        void Return();
        event Action<TValue> OnReturn;
    }

    public interface IPool
    {
        int PoolCount { get; }
        int ActiveCount { get; }
    }
}