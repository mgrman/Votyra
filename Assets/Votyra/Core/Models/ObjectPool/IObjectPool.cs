namespace Votyra.Core.Models.ObjectPool
{
    public interface IObjectPool<T>
    {
        T GetObject();

        void ReturnObject(T obj);
    }
}
