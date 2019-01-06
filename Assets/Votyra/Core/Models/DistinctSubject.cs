using System;
using System.Collections.Generic;
using UniRx;

namespace Votyra.Core.Models
{
    public static class DistinctSubjectUtils
    {
        public static IBehaviorSubject<T> MakeDistinct<T>(this IBehaviorSubject<T> subject) => new DistinctSubject<T>(subject, null);

        public static ISubject<T> MakeDistinct<T>(this ISubject<T> subject) => new DistinctSubject<T>(subject, null);

        public static IBehaviorSubject<T> MakeDistinct<T>(this IBehaviorSubject<T> subject, IEqualityComparer<T> comparer) => new DistinctSubject<T>(subject, comparer);

        public static ISubject<T> MakeDistinct<T>(this ISubject<T> subject, IEqualityComparer<T> comparer) => new DistinctSubject<T>(subject, comparer);
    }

    public class DistinctSubject<T> : IBehaviorSubject<T>
    {
        private readonly IEqualityComparer<T> _comparer;

        private readonly Func<T> _getValue;
        private readonly IObservable<T> _observable;
        private readonly IObserver<T> _observer;

        public DistinctSubject(ISubject<T> subject, IEqualityComparer<T> comparer)
        {
            _observable = subject;
            _comparer = comparer ?? EqualityComparer<T>.Default;

            _observer = subject;
            if (subject is IBehaviorSubject<T>)
                _getValue = () => (subject as IBehaviorSubject<T>).Value;
            else
                _getValue = () =>
                {
                    throw new NotSupportedException();
                };
        }

        public T Value => _getValue();

        public void OnCompleted()
        {
            _observer.OnCompleted();
        }

        public void OnError(Exception error)
        {
            _observer.OnError(error);
        }

        public void OnNext(T value)
        {
            _observer.OnNext(value);
        }

        public IDisposable Subscribe(IObserver<T> observer) =>
            _observable.DistinctUntilChanged()
                .Subscribe(observer);
    }
}