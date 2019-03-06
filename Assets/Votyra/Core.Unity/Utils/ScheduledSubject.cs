using System;
using UniRx;

namespace Votyra.Core.Models
{
    public static class ScheduledSubjectUtils
    {
        public static ISubject<T> MakeScheduledOnMainThread<T>(this ISubject<T> subject) => new ScheduledSubject<T>(subject);
    }

    public class ScheduledSubject<T> : ISubject<T>
    {
        private readonly Func<T> _getValue;
        private readonly IObservable<T> _observable;
        private readonly IObserver<T> _observer;
        private readonly IScheduler _scheduler;

        public ScheduledSubject(ISubject<T> subject)
        {
            _scheduler = Scheduler.MainThread;
            _observable = subject;

            _observer = subject;
        }

        public void OnCompleted()
        {
            if (MainThreadDispatcher.IsInMainThread)
                _observer.OnCompleted();
            else
                _scheduler.Schedule(() =>
                {
                    _observer.OnCompleted();
                });
        }

        public void OnError(Exception error)
        {
            if (MainThreadDispatcher.IsInMainThread)
                _observer.OnError(error);
            else
                _scheduler.Schedule(() =>
                {
                    _observer.OnError(error);
                });
        }

        public void OnNext(T value)
        {
            if (MainThreadDispatcher.IsInMainThread)
                _observer.OnNext(value);
            else
                ScheduledOnNext(value);
        }

        public IDisposable Subscribe(IObserver<T> observer) => _observable.Subscribe(observer);

        private void ScheduledOnNext(T value)
        {
            _scheduler.Schedule(() =>
            {
                _observer.OnNext(value);
            });
        }
    }
}