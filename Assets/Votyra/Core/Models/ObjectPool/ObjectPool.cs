using System;
using System.Collections.Generic;

namespace Votyra.Core.Models.ObjectPool
{
    public class ObjectPool<T> : IObjectPool<T>
    {
        private readonly int _limit;
        private readonly Func<T> _objectGenerator;
        private readonly List<T> _objects;

        public ObjectPool(int limit, Func<T> objectGenerator)
        {
            if (objectGenerator == null) throw new ArgumentNullException("objectGenerator");
            _objects = new List<T>();
            _objectGenerator = objectGenerator;
            _limit = limit;
        }

        public virtual T GetObject()
        {
            T obj;
            if (_objects.Count > 0)
            {
                obj = _objects[_objects.Count - 1];
                _objects.RemoveAt(_objects.Count - 1);
            }
            else
            {
                obj = _objectGenerator();
            }
            return obj;
        }

        public virtual void ReturnObject(T obj)
        {
            if (_objects.Count < _limit)
            {
                _objects.Add(obj);
            }
        }
    }
}