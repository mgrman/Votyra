namespace Votyra.Core.Models.ObjectPool
{
    public interface IObjectDictionaryPool<T, TKey> where TKey : struct
    {
        T GetObject(TKey key);

        void ReturnObject(T obj, TKey key);
    }
}
