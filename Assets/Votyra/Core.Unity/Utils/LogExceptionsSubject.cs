using System;
using UniRx;
using Votyra.Core.Logging;

namespace Votyra.Core.Models
{
    public class LogExceptionsSubject<T> : ISubject<T>
    {
        private readonly IThreadSafeLogger logger;
        private readonly IObservable<T> observable;
        private readonly IObserver<T> observer;

        public LogExceptionsSubject(IObservable<T> subject, IThreadSafeLogger logger)
        {
            this.observable = subject;
            this.logger = logger;

            this.observer = subject as IObserver<T>;
        }

        public void OnCompleted()
        {
            this.observer?.OnCompleted();
        }

        public void OnError(Exception error)
        {
            this.observer?.OnError(error);
        }

        public void OnNext(T value)
        {
            this.observer?.OnNext(value);
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            return this.observable.Subscribe(Observer.Create<T>(OnNext, observer.OnError, observer.OnCompleted));

            void OnNext(T o)
            {
                try
                {
                    this.logger.LogMessage(o);
                    observer.OnNext(o);
                }
                catch (Exception ex)
                {
                    this.logger.LogException(ex);
                    this.OnError(ex);
                }
            }
        }
    }
}
