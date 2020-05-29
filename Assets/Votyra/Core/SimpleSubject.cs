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
            get => this._value;
            set
            {
                if (EqualityComparer<T>.Default.Equals(this._value, value))
                {
                    return;
                }

                this._value = value;
                this.OnNext(value);
            }
        }

        public void Dispose()
        {
            this.OnCompleted();
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            this._observers.Add(observer);
            observer.OnNext(this.Value);
            return new CallbackDisposable(this, observer);
        }

        public void OnCompleted()
        {
            this._observers.ForEach(o => o.OnCompleted());
            this._observers.Clear();
        }

        public void OnError(Exception error)
        {
            this._observers.ForEach(o => o.OnError(error));
        }

        public void OnNext(T value)
        {
            this._observers.ForEach(o => o.OnNext(value));
        }

        private struct CallbackDisposable : IDisposable
        {
            private readonly SimpleSubject<T> _parent;
            private readonly IObserver<T> _observer;

            public CallbackDisposable(SimpleSubject<T> parent, IObserver<T> observer)
            {
                this._parent = parent;
                this._observer = observer;
            }

            public void Dispose()
            {
                this._parent._observers.Remove(this._observer);
            }
        }
    }
}
