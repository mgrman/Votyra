namespace Votyra.Core.Models
{
    public static class MatrixExtensions
    {
        public static int SizeX<T>(this T[,] matrix) => matrix.GetLength(0);

        public static int SizeY<T>(this T[,] matrix) => matrix.GetLength(1);

        public static Vector2i Size<T>(this T[,] matrix) => new Vector2i(matrix.SizeX(), matrix.SizeY());

        public static Range2i Range<T>(this T[,] matrix) => Range2i.FromMinAndMax(new Vector2i(matrix.GetLowerBound(0), matrix.GetLowerBound(1)), new Vector2i(matrix.GetUpperBound(0) + 1, matrix.GetUpperBound(1) + 1));

        public static int SizeX<T>(this T[,,] matrix) => matrix.GetLength(0);

        public static int SizeY<T>(this T[,,] matrix) => matrix.GetLength(1);

        public static int SizeZ<T>(this T[,,] matrix) => matrix.GetLength(2);

        public static Vector3i Size<T>(this T[,,] matrix) => new Vector3i(matrix.SizeX(), matrix.SizeY(), matrix.SizeZ());

        public static Range3i Range<T>(this T[,,] matrix) => Range3i.FromMinAndMax(new Vector3i(matrix.GetLowerBound(0), matrix.GetLowerBound(1), matrix.GetLowerBound(2)), new Vector3i(matrix.GetUpperBound(0) + 1, matrix.GetUpperBound(1) + 1, matrix.GetUpperBound(2) + 1));

        public static bool ContainsIndex<T>(this T[,] matrix, Vector2i index) => (index.X >= 0) && (index.Y >= 0) && (index.X < matrix.SizeX()) && (index.Y < matrix.SizeY());

        public static T TryGet<T>(this T[,] matrix, Vector2i i, T defaultValue) => matrix.ContainsIndex(i) ? matrix[i.X, i.Y] : defaultValue;

        public static T Get<T>(this T[,] matrix, Vector2i i) => matrix[i.X, i.Y];

        public static bool TrySet<T>(this T[,] matrix, Vector2i i, T value)
        {
            if (!matrix.ContainsIndex(i))
            {
                return false;
            }

            matrix[i.X, i.Y] = value;
            return true;
        }

        public static void Set<T>(this T[,] matrix, Vector2i i, T value) => matrix[i.X, i.Y] = value;

        public static bool ContainsIndex<T>(this T[,,] matrix, Vector3i index) => (index.X >= 0) && (index.Y >= 0) && (index.Z >= 0) && (index.X < matrix.SizeX()) && (index.Y < matrix.SizeY()) && (index.Z < matrix.SizeZ());

        public static T TryGet<T>(this T[,,] matrix, Vector3i i, T defaultValue) => matrix.ContainsIndex(i) ? matrix[i.X, i.Y, i.Z] : defaultValue;

        public static T Get<T>(this T[,,] matrix, Vector3i i) => matrix[i.X, i.Y, i.Z];

        public static bool TrySet<T>(this T[,,] matrix, Vector3i i, T value)
        {
            if (!matrix.ContainsIndex(i))
            {
                return false;
            }

            matrix[i.X, i.Y, i.Z] = value;
            return true;
        }

        public static void Set<T>(this T[,,] matrix, Vector3i i, T value) => matrix[i.X, i.Y, i.Z] = value;
    }
}
