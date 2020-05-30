using System;
using System.Collections.Generic;
using Votyra.Core.Logging;

namespace Votyra.Core.Pooling
{
    public class ArcPool<TValue> : IArcPool<TValue>
    {
        private readonly Func<TValue> factory;
        private readonly List<ArcResource<TValue>> list = new List<ArcResource<TValue>>();
        private readonly object @lock = new object();

        public ArcPool(Func<TValue> factory)
        {
            this.factory = factory;
        }

        public int PoolCount { get; private set; }

        public int ActiveCount { get; private set; }

        public ArcResource<TValue> Get()
        {
            lock (this.@lock)
            {
                this.ActiveCount++;
                ArcResource<TValue> value;
                if (this.list.Count == 0)
                {
                    value = new ArcResource<TValue>(this.factory(), this.ReturnRaw);
                }
                else
                {
                    this.PoolCount--;
                    value = this.list[this.list.Count - 1];
                    this.list.RemoveAt(this.list.Count - 1);
                }

                value.Activate();
                return value;
            }
        }

        public void ReturnRaw(ArcResource<TValue> value)
        {
            lock (this.@lock)
            {
                this.ActiveCount--;
                this.PoolCount++;

                this.list.Add(value);
#if UNITY_EDITOR
                if (this.list.Count != this.PoolCount)
                {
                    StaticLogger.LogError("ArcPool in inconsistent state!");
                }
#endif
            }
        }
    }
}
