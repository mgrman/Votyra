namespace Votyra.Core.Pooling
{
    public interface IArcPool<in TKey, TValue> : IPool
    {
        ArcResource<TValue> Get(TKey arg);
    }

    public interface IRawPool<in TKey, TValue> : IPool
    {
        TValue GetRaw(TKey arg);
        void ReturnRaw(TValue value);
    }

    public interface IArcPool<TValue> : IPool
    {
        ArcResource<TValue> Get();
    }
    
    public interface IRawPool<TValue> : IPool
    {
        TValue GetRaw();
        void ReturnRaw(TValue value);
    }

    public interface IPool
    {
        int PoolCount { get; }
        int ActiveCount { get; }
    }
}