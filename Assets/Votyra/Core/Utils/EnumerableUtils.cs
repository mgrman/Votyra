using System.Collections.Generic;

namespace Votyra.Core.Utils
{
    public static class EnumerableUtils
    {
        public static IEnumerable<T> AsEnumerable<T>(this T item)
        {
            yield return item;
        }

        public static string StringJoin(this IEnumerable<string> items, string separator = ", ")
        {
            return string.Join(separator, items);
        }

        public static string StringJoin<T>(this IEnumerable<T> items, string separator = ", ")
        {
            return string.Join(separator, items);
        }
    }
}