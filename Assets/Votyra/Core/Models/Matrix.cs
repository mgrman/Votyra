namespace Votyra.Core.Models
{
    public class Matrix<T> : IMatrix<T>
    {

        public T[,] NativeMatrix;

        //public readonly Vector2i offset;
        public Vector2i size { get; }

        public Matrix(Vector2i matrixSize) //, Vector2i indicesOffset)
        {
            NativeMatrix = new T[matrixSize.x, matrixSize.y];
            //_points = new T[matrixSize.x+indicesOffset.x, matrixSize.y + indicesOffset.y];
            //offset = indicesOffset;
            size = matrixSize;
        }

        public bool IsSameSize(Vector2i size)
        {
            return this.size == size; //&& this.offset == offset;
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
                return NativeMatrix[i.x, i.y];
            }
            set
            {
                NativeMatrix[i.x, i.y] = value;
            }
        }

        public T TryGet(Vector2i i, T defaultValue)
        {
            return i.IsAsIndexContained(size) ? NativeMatrix[i.x, i.y] : defaultValue;
        }
    }
}