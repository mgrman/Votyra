using System;
using Votyra.Core.Models;

namespace Votyra.Core.Utils
{
    public static class ArrayUtils
    {
        public static T[] CreateNonNull<T>(T item1, T item2) where T : class
        {
            if (item1 != default(T) && item2 != default(T))
                return new[] {item1, item2};
            if (item1 != default(T))
                return new[] {item1};
            if (item2 != default(T))
                return new[] {item2};
            return Array.Empty<T>();
        }
    }
}