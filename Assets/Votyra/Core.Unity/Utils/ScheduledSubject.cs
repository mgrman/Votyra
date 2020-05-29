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
            this._scheduler = Scheduler.MainThread;
            this._observable = subject;

            this._observer = subject;
        }

        public void OnCompleted()
        {
            if (MainThreadDispatcher.IsInMainThread)
            {
                this._observer.OnCompleted();
            }
            else
            {
                this._scheduler.Schedule(() =>
                {
                    this._observer.OnCompleted();
                });
            }
        }

        public void OnError(Exception error)
        {
            if (MainThreadDispatcher.IsInMainThread)
            {
                this._observer.OnError(error);
            }
            else
            {
                this._scheduler.Schedule(() =>
                {
                    this._observer.OnError(error);
                });
            }
        }

        public void OnNext(T value)
        {
            if (MainThreadDispatcher.IsInMainThread)
            {
                this._observer.OnNext(value);
            }
            else
            {
                this.ScheduledOnNext(value);
            }
        }

        public IDisposable Subscribe(IObserver<T> observer) => this._observable.Subscribe(observer);

        private void ScheduledOnNext(T value)
        {
            this._scheduler.Schedule(() =>
            {
                this._observer.OnNext(value);
            });
        }
    }
}
