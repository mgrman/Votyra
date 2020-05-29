using System;
using System.Collections.Generic;
using Votyra.Core.Logging;

namespace Votyra.Core.Pooling
{
    public class ArcPool<TValue> : IArcPool<TValue>
    {
        private readonly Func<TValue> _factory;
        private readonly List<ArcResource<TValue>> _list = new List<ArcResource<TValue>>();
        private readonly object _lock = new object();

        public ArcPool(Func<TValue> factory)
        {
            this._factory = factory;
        }

        public int PoolCount { get; private set; }

        public int ActiveCount { get; private set; }

        public ArcResource<TValue> Get()
        {
            lock (this._lock)
            {
                this.ActiveCount++;
                ArcResource<TValue> value;
                if (this._list.Count == 0)
                {
                    value = new ArcResource<TValue>(this._factory(), this.ReturnRaw);
                }
                else
                {
                    this.PoolCount--;
                    value = this._list[this._list.Count - 1];
                    this._list.RemoveAt(this._list.Count - 1);
                }

                value.Activate();
                return value;
            }
        }

        public void ReturnRaw(ArcResource<TValue> value)
        {
            lock (this._lock)
            {
                this.ActiveCount--;
                this.PoolCount++;

                this._list.Add(value);
#if UNITY_EDITOR
                if (this._list.Count != this.PoolCount)
                {
                    StaticLogger.LogError("ArcPool in inconsistent state!");
                }
#endif
            }
        }
    }
}
