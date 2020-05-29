using System;
using System.Collections.Generic;

namespace Votyra.Core.Pooling
{
    public class Pool<TValue> : IRawPool<TValue>
    {
        private readonly Func<TValue> _factory;
        private readonly List<TValue> _list = new List<TValue>();
        private readonly object _lock = new object();

        public Pool(Func<TValue> factory)
        {
            this._factory = factory;
        }

        public int PoolCount { get; private set; }

        public int ActiveCount { get; private set; }

        public TValue GetRaw()
        {
            this.ActiveCount++;
            lock (this._lock)
            {
                TValue value;
                if (this._list.Count == 0)
                {
                    value = this._factory();
                }
                else
                {
                    this.PoolCount--;
                    value = this._list[this._list.Count - 1];
                    this._list.RemoveAt(this._list.Count - 1);
                }

                return value;
            }
        }

        public void ReturnRaw(TValue value)
        {
            this.ActiveCount--;
            this.PoolCount++;
            lock (this._lock)
            {
                this._list.Add(value);
            }
        }
    }
}
