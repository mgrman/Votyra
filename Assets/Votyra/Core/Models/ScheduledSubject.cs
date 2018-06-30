using System;
using UniRx;

namespace Votyra.Core.Models
{
    public static class ScheduledSubjectUtils
    {
        public static IBehaviorSubject<T> MakeScheduled<T>(this IBehaviorSubject<T> subject, IScheduler scheduler)
        {
            return new ScheduledSubject<T>(subject, scheduler);
        }

        public static ISubject<T> MakeScheduled<T>(this ISubject<T> subject, IScheduler scheduler)
        {
            return new ScheduledSubject<T>(subject, scheduler);
        }

        public static IBehaviorSubject<T> MakeScheduledOnCurrentThread<T>(this IBehaviorSubject<T> subject)
        {
            return new ScheduledSubject<T>(subject, Scheduler.CurrentThread);
        }

        public static ISubject<T> MakeScheduledOnCurrentThread<T>(this ISubject<T> subject)
        {
            return new ScheduledSubject<T>(subject, Scheduler.CurrentThread);
        }

        public static IBehaviorSubject<T> MakeScheduledOnMainThread<T>(this IBehaviorSubject<T> subject)
        {
            return new ScheduledSubject<T>(subject, Scheduler.MainThread);
        }

        public static ISubject<T> MakeScheduledOnMainThread<T>(this ISubject<T> subject)
        {
            return new ScheduledSubject<T>(subject, Scheduler.MainThread);
        }
    }

    public class ScheduledSubject<T> : IBehaviorSubject<T>
    {
        private readonly Func<T> _getValue;
        private readonly IObservable<T> _observable;
        private readonly IObserver<T> _observer;
        private readonly IScheduler _scheduler;

        public ScheduledSubject(ISubject<T> subject, IScheduler scheduler)
        {
            _scheduler = scheduler;
            _observable = subject;

            _observer = subject;
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

        public void OnCompleted()
        {
            _scheduler.Schedule(() =>
            {
                _observer
                    .OnCompleted();
            });
        }

        public void OnError(Exception error)
        {
            _scheduler.Schedule(() =>
            {
                _observer.OnError(error);
            });
        }

        public void OnNext(T value)
        {
            _scheduler.Schedule(() =>
            {
                _observer.OnNext(value);
            });
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            return _observable.Subscribe(observer);
        }
    }
}