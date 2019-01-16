using System;

namespace Votyra.Core.Models
{
    public interface IPoolableMatrix2<T> : IMatrix2<T>, IDisposable
    {
    }

    public interface IMatrix2<T>
    {
        Vector2i Size { get; }
        T this[int ix, int iy] { get; set; }
        T this[Vector2i i] { get; set; }
    }

    public static class Matrix2Extensions
    {
        public static bool ContainsIndex<T>(this IMatrix2<T> matrix, Vector2i index) => index.X >= 0 && index.Y >= 0 && index.X < matrix.Size.X && index.Y < matrix.Size.Y;

        public static T TryGet<T>(this IMatrix2<T> matrix, Vector2i i, T defaultValue) => matrix.ContainsIndex(i) ? matrix[i] : defaultValue;
    }
}