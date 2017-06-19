using System.Collections.Generic;

namespace Votyra.Utils
{
    public static class EnumerableUtils
    {
        public static IEnumerable<T> AsEnumerable<T>(this T item)
        {
            yield return item;
        }
    }
}