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
            if (objectGenerator == null)
            {
                throw new ArgumentNullException("objectGenerator");
            }

            this._objects = new List<T>();
            this._objectGenerator = objectGenerator;
            this._limit = limit;
        }

        public virtual T GetObject()
        {
            T obj;
            if (this._objects.Count > 0)
            {
                obj = this._objects[this._objects.Count - 1];
                this._objects.RemoveAt(this._objects.Count - 1);
            }
            else
            {
                obj = this._objectGenerator();
            }

            return obj;
        }

        public virtual void ReturnObject(T obj)
        {
            if (this._objects.Count < this._limit)
            {
                this._objects.Add(obj);
            }
        }
    }
}
