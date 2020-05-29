using System;
using UniRx;
using Votyra.Core.Logging;

namespace Votyra.Core.Models
{
    public static class LogExceptionsSubjectUtils
    {
        public static ISubject<T> MakeLogExceptions<T>(this ISubject<T> subject, IThreadSafeLogger logger) => new LogExceptionsSubject<T>(subject, logger);

        public static IObservable<T> MakeLogExceptions<T>(this IObservable<T> observable, IThreadSafeLogger logger) => new LogExceptionsSubject<T>(observable, logger);
    }

    public class LogExceptionsSubject<T> : ISubject<T>
    {
        private readonly Func<T> _getValue;
        private readonly IThreadSafeLogger _logger;
        private readonly IObservable<T> _observable;
        private readonly IObserver<T> _observer;

        public LogExceptionsSubject(IObservable<T> subject, IThreadSafeLogger logger)
        {
            this._observable = subject;
            this._logger = logger;

            this._observer = subject as IObserver<T>;
        }

        public void OnCompleted()
        {
            this._observer?.OnCompleted();
        }

        public void OnError(Exception error)
        {
            this._observer?.OnError(error);
        }

        public void OnNext(T value)
        {
            this._observer?.OnNext(value);
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            return this._observable.Subscribe(Observer.Create<T>(o =>
                {
                    try
                    {
                        this._logger.LogMessage(o);
                        observer.OnNext(o);
                    }
                    catch (Exception ex)
                    {
                        this._logger.LogException(ex);
                        this.OnError(ex);
                    }
                },
                observer.OnError,
                observer.OnCompleted));
        }
    }
}
