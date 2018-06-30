using System;
using System.Collections.Generic;
using System.Linq;

namespace Votyra.Core.Utils
{
    public static class LinqUtils
    {
        public static IEnumerable<IEnumerable<T>> Batch<T>(
             this IEnumerable<T> source, int batchSize)
        {
            using (var enumerator = source.GetEnumerator())
                while (enumerator.MoveNext())
                    yield return YieldBatchElements(enumerator, batchSize - 1);
        }

        public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> items)
        {
            return items ?? Enumerable.Empty<T>();
        }

        public static T ExtremeByOrDefault<T, R>(this IEnumerable<T> items, Func<T, R> func, Func<R, R, bool> compareFunc)
            where R : IComparable<R>
        {
            T bestItem = default(T);
            R bestItemsValue = default(R);
            bool isFirst = true;

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

        public static T MaxByOrDefault<T, R>(this IEnumerable<T> items, Func<T, R> func)
                                    where R : IComparable<R>
        {
            return items.ExtremeByOrDefault(func, (currentValue, previousBestValue) => currentValue.CompareTo(previousBestValue) > 0);
        }

        public static T MinByOrDefault<T, R>(this IEnumerable<T> items, Func<T, R> func)
            where R : IComparable<R>
        {
            return items.ExtremeByOrDefault(func, (currentValue, previousBestValue) => currentValue.CompareTo(previousBestValue) < 0);
        }

        private static IEnumerable<T> YieldBatchElements<T>(
              IEnumerator<T> source, int batchSize)
        {
            yield return source.Current;
            for (int i = 0; i < batchSize && source.MoveNext(); i++)
                yield return source.Current;
        }
    }
}