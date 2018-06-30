namespace Votyra.Core.Models
{
    public class Matrix2<T> : IMatrix2<T>
    {
        public T[,] NativeMatrix;

        public Matrix2(Vector2i matrixSize)
        {
            NativeMatrix = new T[matrixSize.X, matrixSize.Y];
            Size = matrixSize;
        }

        public Vector2i Size { get; }

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

        public bool IsSameSize(Vector2i size)
        {
            return this.Size == size;
        }
    }
}