namespace Votyra.Core.Models
{
    public class Matrix2<T> : IMatrix2<T>
    {
        public T[,] NativeMatrix;

        public Vector2i size { get; }

        public Matrix2(Vector2i matrixSize)
        {
            NativeMatrix = new T[matrixSize.X, matrixSize.Y];
            size = matrixSize;
        }

        public bool IsSameSize(Vector2i size)
        {
            return this.size == size;
        }

        public T this[int ix, int iy]
        {
            get
            {
                return NativeMatrix[ix, iy];
            }
            set
            {
                NativeMatrix[ix, iy] = value;
            }
        }

        public T this[Vector2i i]
        {
            get
            {
                return NativeMatrix[i.X, i.Y];
            }
            set
            {
                NativeMatrix[i.X, i.Y] = value;
            }
        }

        public T TryGet(Vector2i i, T defaultValue)
        {
            return size.ContainsIndex(i) ? NativeMatrix[i.X, i.Y] : defaultValue;
        }
    }
}