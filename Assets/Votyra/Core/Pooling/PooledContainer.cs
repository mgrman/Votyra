using System;

namespace Votyra.Core.Pooling
{
    public interface IPool<out TValue, in TKey> where TKey : struct
        where TValue : IPoolable<TValue, TKey>
    {
        TValue Get(TKey arg);
    }

    public interface IPoolable<out TValue, out TKey> : IPoolable<TValue> where TKey : struct
        where TValue : IPoolable<TValue, TKey>
    {
        TKey Key { get; }
    }


    public interface IPool<out TValue> 
        where TValue : IPoolable<TValue>
    {
        TValue Get();
    }

    public interface IPoolable<out TValue> : IDisposable
        where TValue : IPoolable<TValue>
    {
        event Action<TValue> OnDispose;
    }
}