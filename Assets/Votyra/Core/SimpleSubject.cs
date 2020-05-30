using System;
using System.Collections.Generic;

namespace Votyra.Core
{
    public class SimpleSubject<T> : IObservable<T>, IObserver<T>, IDisposable
    {
        private readonly List<IObserver<T>> observers = new List<IObserver<T>>();
        private T value;

        public T Value
        {
            get => this.value;
            set
            {
                if (EqualityComparer<T>.Default.Equals(this.value, value))
                {
                    return;
                }

                this.value = value;
                this.OnNext(value);
            }
        }

        public void Dispose()
        {
            this.OnCompleted();
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            this.observers.Add(observer);
            observer.OnNext(this.Value);
            return new CallbackDisposable(this, observer);
        }

        public void OnCompleted()
        {
            this.observers.ForEach(o => o.OnCompleted());
            this.observers.Clear();
        }

        public void OnError(Exception error)
        {
            this.observers.ForEach(o => o.OnError(error));
        }

        public void OnNext(T value)
        {
            this.observers.ForEach(o => o.OnNext(value));
        }

        private struct CallbackDisposable : IDisposable
        {
            private readonly SimpleSubject<T> parent;
            private readonly IObserver<T> observer;

            public CallbackDisposable(SimpleSubject<T> parent, IObserver<T> observer)
            {
                this.parent = parent;
                this.observer = observer;
            }

            public void Dispose()
            {
                this.parent.observers.Remove(this.observer);
            }
        }
    }
}
