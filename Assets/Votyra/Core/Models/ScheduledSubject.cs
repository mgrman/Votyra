using System;
using System.Threading;
using UniRx;

namespace Votyra.Core.Models
{
    public static class ScheduledSubjectUtils
    {
        public static IBehaviorSubject<T> MakeScheduledOnMainThread<T>(this IBehaviorSubject<T> subject)
        {
            return new ScheduledSubject<T>(subject);
        }

        public static ISubject<T> MakeScheduledOnMainThread<T>(this ISubject<T> subject)
        {
            return new ScheduledSubject<T>(subject);
        }
    }

    public class ScheduledSubject<T> : IBehaviorSubject<T>
    {
        private readonly IScheduler _scheduler;
        private readonly IObservable<T> _observable;
        private readonly IObserver<T> _observer;

        private readonly Func<T> _getValue;

        public ScheduledSubject(ISubject<T> subject)
        {
            _scheduler = Scheduler.MainThread;
            _observable = subject;

            _observer = subject;
            if (subject is IBehaviorSubject<T>)
            {
                _getValue = () => (subject as IBehaviorSubject<T>).Value;
            }
            else
            {
                _getValue = () => { throw new NotSupportedException(); };
            }
        }

        public T Value => _getValue();

        public void OnCompleted()
        {
            if (MainThreadDispatcher.IsInMainThread)
            {
                _observer.OnCompleted(); 
            }
            else
            {
                _scheduler.Schedule(() => { _observer.OnCompleted(); });
            }
        }

        public void OnError(Exception error)
        {
            if (MainThreadDispatcher.IsInMainThread)
            {
                _observer.OnError(error);
            }
            else
            {
                _scheduler.Schedule(() => { _observer.OnError(error); });
            }
        }

        public void OnNext(T value)
        {
            if (MainThreadDispatcher.IsInMainThread)
            {
                _observer.OnNext(value);
            }
            else
            {
                ScheduledOnNext(value);
            }
        }

        private void ScheduledOnNext(T value)
        {
            _scheduler.Schedule(() => { _observer.OnNext(value); });
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            return _observable.Subscribe(observer);
        }
    }
}