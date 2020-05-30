using System;

namespace Votyra.Core.Pooling
{
    public class ArcResource<TValue> : IDisposable
    {
        private readonly object @lock = new object();
        private readonly Action<ArcResource<TValue>> onDispose;
        private int activeCounter;

        public ArcResource(TValue value, Action<ArcResource<TValue>> onReturn)
        {
            this.Value = value;
            this.onDispose = onReturn;
            this.activeCounter = 0;
        }

        public TValue Value { get; }

        public void Dispose()
        {
            bool invoke;
            lock (this.@lock)
            {
                this.activeCounter--;
                invoke = this.activeCounter <= 0;
            }

            if (invoke)
            {
                this.onDispose?.Invoke(this);
            }
        }

        public ArcResource<TValue> Activate()
        {
            lock (this.@lock)
            {
                this.activeCounter++;
                return this;
            }
        }
    }
}
