namespace Votyra.Core.Models
{
    public class MatrixWithOffset2<T> : IMatrix2<T>
    {
        private readonly T[,] _points;

        public MatrixWithOffset2(Vector2i matrixSize, Vector2i indicesOffset)
        {
            _points = new T[matrixSize.X + indicesOffset.X, matrixSize.Y + indicesOffset.Y];
            offset = indicesOffset;
            Size = matrixSize;
        }

        public Vector2i offset { get; }
        public Vector2i Size { get; }

        public T this[int ix, int iy]
        {
            get => _points[ix + offset.X, iy + offset.Y];
            set => _points[ix + offset.X, iy + offset.Y] = value;
        }

        public T this[Vector2i i]
        {
            get => _points[i.X + offset.X, i.Y + offset.Y];
            set => _points[i.X + offset.X, i.Y + offset.Y] = value;
        }

        public bool IsSameSize(Vector2i size, Vector2i offset) => Size == size && this.offset == offset;
    }
}