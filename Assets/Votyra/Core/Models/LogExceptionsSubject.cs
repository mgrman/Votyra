using System;
using UniRx;
using UnityEngine;

namespace Votyra.Core.Models
{
    public static class LogExceptionsSubjectUtils
    {
        public static IBehaviorSubject<T> MakeLogExceptions<T>(this IBehaviorSubject<T> subject)
        {
            return new LogExceptionsSubject<T>(subject);
        }

        public static ISubject<T> MakeLogExceptions<T>(this ISubject<T> subject)
        {
            return new LogExceptionsSubject<T>(subject);
        }

        public static IObservable<T> MakeLogExceptions<T>(this IObservable<T> observable)
        {
            return new LogExceptionsSubject<T>(observable);
        }
    }

    public class LogExceptionsSubject<T> : IBehaviorSubject<T>
    {
        private readonly IObservable<T> _observable;
        private readonly IObserver<T> _observer;

        private readonly Func<T> _getValue;

        public LogExceptionsSubject(IObservable<T> subject)
        {
            _observable = subject;

            _observer = subject as IObserver<T>;
            if (subject is IBehaviorSubject<T>)
            {
                _getValue = () => (subject as IBehaviorSubject<T>).Value;
            }
            else
            {
                _getValue = () =>
                {
                    throw new NotSupportedException();
                };
            }
        }

        public T Value => _getValue();

        public void OnCompleted() => _observer?.OnCompleted();

        public void OnError(Exception error) => _observer?.OnError(error);

        public void OnNext(T value) => _observer?.OnNext(value);

        public IDisposable Subscribe(IObserver<T> observer)
        {
            return _observable.Subscribe(Observer.Create<T>((o) =>
            {
                try
                {
                    Debug.Log(o);
                    observer.OnNext(o);
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                    OnError(ex);
                }
            }, observer.OnError, observer.OnCompleted));
        }
    }
}