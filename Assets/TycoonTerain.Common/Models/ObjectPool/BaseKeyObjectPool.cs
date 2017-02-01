using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public abstract class BaseKeyObjectPool<T, TKey> : IObjectDictionaryPool<T, TKey>
    where TKey:struct
{
    private readonly Func<TKey, T> _objectGenerator;
    private readonly int _limit;

    public BaseKeyObjectPool(int limit, Func<TKey, T> objectGenerator)
    {
        if (objectGenerator == null) throw new ArgumentNullException("objectGenerator");
        _objectGenerator = objectGenerator;

        _limit = Math.Max(limit, 1);
    }

    public virtual T GetObject(TKey key)
    {
        var objectPool = GetPool(key);
        T obj;
        if (objectPool.Count > 0)
        {
            obj = objectPool[objectPool.Count - 1];
            objectPool.RemoveAt(objectPool.Count - 1);
        }
        else
        {
            obj = _objectGenerator(key);
        }
        return obj;
    }

    protected abstract List<T> GetPool(TKey key);

    public virtual void ReturnObject(T obj, TKey key)
    {
        var objectPool = GetPool(key);
        if (objectPool.Count < _limit)
        {
            objectPool.Add(obj);
        }
    }
}
