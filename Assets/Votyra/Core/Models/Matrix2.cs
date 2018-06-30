namespace Votyra.Core.Models
{
    public class Matrix2<T> : IMatrix2<T>
    {
        public T[,] NativeMatrix;

        public Vector2i Size { get; }

        public Matrix2(Vector2i matrixSize)
        {
            NativeMatrix = new T[matrixSize.X, matrixSize.Y];
            Size = matrixSize;
        }

        public bool IsSameSize(Vector2i size)
        {
            return this.Size == size;
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
    }
}