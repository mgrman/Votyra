using System;
using System.Collections.Generic;

namespace Votyra.Core.Models.ObjectPool
{
    public class ObjectPool<T> : IObjectPool<T>
    {
        private readonly int limit;
        private readonly Func<T> objectGenerator;
        private readonly List<T> objects;

        public ObjectPool(int limit, Func<T> objectGenerator)
        {
            if (objectGenerator == null)
            {
                throw new ArgumentNullException("objectGenerator");
            }

            this.objects = new List<T>();
            this.objectGenerator = objectGenerator;
            this.limit = limit;
        }

        public virtual T GetObject()
        {
            T obj;
            if (this.objects.Count > 0)
            {
                obj = this.objects[this.objects.Count - 1];
                this.objects.RemoveAt(this.objects.Count - 1);
            }
            else
            {
                obj = this.objectGenerator();
            }

            return obj;
        }

        public virtual void ReturnObject(T obj)
        {
            if (this.objects.Count < this.limit)
            {
                this.objects.Add(obj);
            }
        }
    }
}
