using System;
using Votyra.Core.Models;

namespace Votyra.Core.Utils
{
    public static class ArrayUtils
    {
        public static Vector2i GetCount<T>(this T[,] data) => new Vector2i(data.GetCountX(), data.GetCountY());

        public static int GetCountX<T>(this T[,] data) => data.GetUpperBound(0) + 1;

        public static int GetCountY<T>(this T[,] data) => data.GetUpperBound(1) + 1;

        public static bool IsInRangeX<T>(this T[,] data, int ix) => data.IsInRange(ix, 0);

        public static bool IsInRangeY<T>(this T[,] data, int iy) => data.IsInRange(iy, 1);

        public static bool IsInRange<T>(this T[,] data, int i, int dimension) => i >= 0 && i <= data.GetUpperBound(dimension);

        public static T[] CreateNonNull<T>(T item1, T item2) where T : class
        {
            if (item1 != default(T) && item2 != default(T))
            {
                return new[]
                {
                    item1,
                    item2
                };
            }

            if (item1 != default(T))
            {
                return new[]
                {
                    item1
                };
            }

            if (item2 != default(T))
            {
                return new[]
                {
                    item2
                };
            }

            return Array.Empty<T>();
        }
    }
}
