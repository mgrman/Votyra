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
            Value = value;
            _onDispose = onReturn;
            _activeCounter = 0;
        }

        public TValue Value { get; }

        public void Dispose()
        {
            bool invoke;
            lock (_lock)
            {
                _activeCounter--;
                invoke = _activeCounter <= 0;
            }

            if (invoke)
            {
                _onDispose?.Invoke(this);
            }
        }

        public ArcResource<TValue> Activate()
        {
            lock (_lock)
            {
                _activeCounter++;
                return this;
            }
        }
    }
}
