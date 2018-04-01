using System;

namespace Votyra.Core.Models
{
    public interface IMatrix3<T>
    {
        Vector3i Size { get; }
        T this[Vector3i i] { get; set; }
    }

    public static class Matrix3Extensions
    {
        public static bool ContainsIndex<T>(this IMatrix3<T> matrix, Vector3i index)
        {
            return index.X >= 0 && index.Y >= 0 && index.Z >= 0 && index.X < matrix.Size.X && index.Y < matrix.Size.Y && index.Z < matrix.Size.Z;
        }

        public static T TryGet<T>(this IMatrix3<T> matrix, Vector3i i, T defaultValue)
        {
            return matrix.ContainsIndex(i) ? matrix[i] : defaultValue;
        }

        public static void ForeachPointExlusive<T>(this IMatrix3<T> matrix, Action<Vector3i> action)
        {
            matrix.Size.ToRange3i().ForeachPointExlusive(action);
        }
    }
}