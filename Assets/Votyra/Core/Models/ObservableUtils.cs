using System;
using System.Collections.Generic;
using UniRx;

namespace Votyra.Core.Models
{
    public static class ObservableUtils
    {
        public static IObservable<ValueWithChange<TSource>> PairWithPrevious<TSource>(this IObservable<TSource> source)
        {
            return source.Scan(default(ValueWithChange<TSource>),
                (acc, current) => new ValueWithChange<TSource>(current, acc.NewValue));
        }

        public struct ValueWithChange<T>
        {
            public readonly T NewValue;
            public readonly T OldValue;

            public ValueWithChange(T newValue, T oldValue)
            {
                NewValue = newValue;
                OldValue = oldValue;
            }
        }
    }
}