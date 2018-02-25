using System.Collections.Generic;

namespace Votyra.Core.Utils
{
    public static class EnumerableUtils
    {
        public static IEnumerable<T> AsEnumerable<T>(this T item)
        {
            yield return item;
        }
    }
}