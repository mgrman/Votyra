using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.XR.Interaction;
using Votyra.Core.Images;
using Votyra.Core.Models;
using Votyra.Core.Pooling;

namespace Votyra.Core.Pooling
{
    public class ArcResource<TValue> : IDisposable
    {
        private readonly object _lock=new object();
        private readonly Action<ArcResource<TValue>> _onDispose;
        private int _activeCounter;
        public TValue Value { get; }

        public ArcResource(TValue value, Action<ArcResource<TValue>> onReturn)
        {
            Value = value;
            _onDispose = onReturn;
            _activeCounter = 0;
        }

        public ArcResource<TValue> Activate()
        {
            lock (_lock)
            {
                _activeCounter++;
                return this;
            }
        }

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
    }
}