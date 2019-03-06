using System;
using System.Collections.Generic;

namespace Votyra.Core
{
    public class SimpleSubject<T> : IObservable<T>, IObserver<T>, IDisposable
    {
        private readonly List<IObserver<T>> _observers = new List<IObserver<T>>();
        private T _value;

        public T Value
        {
            get => _value;
            set
            {
                if (EqualityComparer<T>.Default.Equals(_value, value))
                    return;

                _value = value;
                OnNext(value);
            }
        }

        public void Dispose()
        {
            OnCompleted();
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            _observers.Add(observer);
            observer.OnNext(Value);
            return new CallbackDisposable(this, observer);
        }

        public void OnCompleted()
        {
            _observers.ForEach(o => o.OnCompleted());
            _observers.Clear();
        }

        public void OnError(Exception error)
        {
            _observers.ForEach(o => o.OnError(error));
        }

        public void OnNext(T value)
        {
            _observers.ForEach(o => o.OnNext(value));
        }

        private struct CallbackDisposable : IDisposable
        {
            private readonly SimpleSubject<T> _parent;
            private readonly IObserver<T> _observer;

            public CallbackDisposable(SimpleSubject<T> parent, IObserver<T> observer)
            {
                _parent = parent;
                _observer = observer;
            }

            public void Dispose()
            {
                _parent._observers.Remove(_observer);
            }
        }
    }
}