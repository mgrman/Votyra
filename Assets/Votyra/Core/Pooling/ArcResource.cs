using System;
using System.Collections.Generic;
using Votyra.Core.Images;
using Votyra.Core.Models;
using Votyra.Core.Pooling;

namespace Votyra.Core.Pooling
{
    public class ArcResource<TValue> : IDisposable
    {
        private readonly Action<ArcResource<TValue>> _onDispose;
        private ushort _activeCounter;
        public TValue Value { get; }

        public ArcResource(TValue value, Action<ArcResource<TValue>> onReturn)
        {
            Value = value;
            _onDispose = onReturn;
            _activeCounter = 0;
        }

        public ArcResource<TValue> Activate()
        {
            _activeCounter++;
            return this;
        }

        private void Deactivate(ArcResource<TValue> value)
        {
            _activeCounter--;
            if (_activeCounter <= 0)
            {
                _onDispose?.Invoke(value);
            }
        }

        public void Dispose()
        {
            Deactivate(this);
        }
    }
}