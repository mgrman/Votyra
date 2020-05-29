using System;

namespace Votyra.Core.Pooling
{
    public class ArcResource<TValue> : IDisposable
    {
        private readonly object _lock = new object();
        private readonly Action<ArcResource<TValue>> _onDispose;
        private int _activeCounter;

        public ArcResource(TValue value, Action<ArcResource<TValue>> onReturn)
        {
            this.Value = value;
            this._onDispose = onReturn;
            this._activeCounter = 0;
        }

        public TValue Value { get; }

        public void Dispose()
        {
            bool invoke;
            lock (this._lock)
            {
                this._activeCounter--;
                invoke = this._activeCounter <= 0;
            }

            if (invoke)
            {
                this._onDispose?.Invoke(this);
            }
        }

        public ArcResource<TValue> Activate()
        {
            lock (this._lock)
            {
                this._activeCounter++;
                return this;
            }
        }
    }
}
