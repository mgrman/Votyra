using System;
using System.Collections.Generic;
using System.Linq;

namespace Votyra.Core.Utils
{
    public static class LinqUtils
    {
        public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> items) => items ?? Enumerable.Empty<T>();
    }
}