using System;
using System.Collections.Generic;
using System.Linq;

namespace Votyra.Core.Utils
{
    public static class LinqUtils
    {
        public static T MaxByOrDefault<T, R>(this IEnumerable<T> items, Func<T, R> func) where R : IComparable<R>
        {
            return items.ExtremeByOrDefault(func, (currentValue, previousBestValue) => currentValue.CompareTo(previousBestValue) > 0);
        }

        public static T MinByOrDefault<T, R>(this IEnumerable<T> items, Func<T, R> func) where R : IComparable<R>
        {
            return items.ExtremeByOrDefault(func, (currentValue, previousBestValue) => currentValue.CompareTo(previousBestValue) < 0);
        }

        public static T ExtremeByOrDefault<T, R>(this IEnumerable<T> items, Func<T, R> func, Func<R, R, bool> compareFunc) where R : IComparable<R>
        {
            var bestItem = default(T);
            var bestItemsValue = default(R);
            var isFirst = true;

            foreach (var item in items)
            {
                var value = func(item);
                if (isFirst || compareFunc(value, bestItemsValue))
                {
                    bestItem = item;
                    bestItemsValue = value;
                    isFirst = false;
                }
            }

            return bestItem;
        }

        public static IEnumerable<IEnumerable<T>> Batch<T>(this IEnumerable<T> source, int batchSize)
        {
            using (var enumerator = source.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    yield return YieldBatchElements(enumerator, batchSize - 1);
                }
            }
        }

        public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> items) => items ?? Enumerable.Empty<T>();

        private static IEnumerable<T> YieldBatchElements<T>(IEnumerator<T> source, int batchSize)
        {
            yield return source.Current;
            for (var i = 0; i < batchSize && source.MoveNext(); i++)
            {
                yield return source.Current;
            }
        }
    }
}