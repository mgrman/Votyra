using System;
using UniRx;

namespace Votyra.Core.Models
{
    public class ScheduledSubject<T> : ISubject<T>
    {
        private readonly IObservable<T> observable;
        private readonly IObserver<T> observer;
        private readonly IScheduler scheduler;

        public ScheduledSubject(ISubject<T> subject)
        {
            this.scheduler = Scheduler.MainThread;
            this.observable = subject;

            this.observer = subject;
        }

        public void OnCompleted()
        {
            if (MainThreadDispatcher.IsInMainThread)
            {
                this.observer.OnCompleted();
            }
            else
            {
                this.scheduler.Schedule(() =>
                {
                    this.observer.OnCompleted();
                });
            }
        }

        public void OnError(Exception error)
        {
            if (MainThreadDispatcher.IsInMainThread)
            {
                this.observer.OnError(error);
            }
            else
            {
                this.scheduler.Schedule(() =>
                {
                    this.observer.OnError(error);
                });
            }
        }

        public void OnNext(T value)
        {
            if (MainThreadDispatcher.IsInMainThread)
            {
                this.observer.OnNext(value);
            }
            else
            {
                this.ScheduledOnNext(value);
            }
        }

        public IDisposable Subscribe(IObserver<T> observer) => this.observable.Subscribe(observer);

        private void ScheduledOnNext(T value)
        {
            this.scheduler.Schedule(() =>
            {
                this.observer.OnNext(value);
            });
        }
    }
}
