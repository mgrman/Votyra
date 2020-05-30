using System;
using System.Collections.Generic;

namespace Votyra.Core.Pooling
{
    public class Pool<TValue> : IRawPool<TValue>
    {
        private readonly Func<TValue> factory;
        private readonly List<TValue> list = new List<TValue>();
        private readonly object @lock = new object();

        public Pool(Func<TValue> factory)
        {
            this.factory = factory;
        }

        public int PoolCount { get; private set; }

        public int ActiveCount { get; private set; }

        public TValue GetRaw()
        {
            this.ActiveCount++;
            lock (this.@lock)
            {
                TValue value;
                if (this.list.Count == 0)
                {
                    value = this.factory();
                }
                else
                {
                    this.PoolCount--;
                    value = this.list[this.list.Count - 1];
                    this.list.RemoveAt(this.list.Count - 1);
                }

                return value;
            }
        }

        public void ReturnRaw(TValue value)
        {
            this.ActiveCount--;
            this.PoolCount++;
            lock (this.@lock)
            {
                this.list.Add(value);
            }
        }
    }
}
