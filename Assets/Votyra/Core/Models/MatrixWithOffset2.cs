namespace Votyra.Core.Models
{
    public class MatrixWithOffset2<T> : IMatrix2<T>
    {
        private readonly T[,] _points;
        public Vector2i offset { get; }
        public Vector2i Size { get; }

        public MatrixWithOffset2(Vector2i matrixSize, Vector2i indicesOffset)
        {
            _points = new T[matrixSize.X + indicesOffset.X, matrixSize.Y + indicesOffset.Y];
            offset = indicesOffset;
            Size = matrixSize;
        }

        public bool IsSameSize(Vector2i size, Vector2i offset)
        {
            return this.Size == size && this.offset == offset;
        }

        public T this[int ix, int iy]
        {
            get
            {
                return _points[ix + offset.X, iy + offset.Y];
            }
            set
            {
                _points[ix + offset.X, iy + offset.Y] = value;
            }
        }

        public T this[Vector2i i]
        {
            get
            {
                return _points[i.X + offset.X, i.Y + offset.Y];
            }
            set
            {
                _points[i.X + offset.X, i.Y + offset.Y] = value;
            }
        }
    }
}