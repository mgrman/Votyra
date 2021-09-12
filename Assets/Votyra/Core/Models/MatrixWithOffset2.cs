namespace Votyra.Core.Models
{
    public class MatrixWithOffset2<T> : IMatrix2<T>
    {
        private readonly T[,] _points;

        public MatrixWithOffset2(Vector2i matrixSize, Vector2i indicesOffset)
        {
            _points = new T[matrixSize.X + indicesOffset.X, matrixSize.Y + indicesOffset.Y];
            Offset = indicesOffset;
            Size = matrixSize;
        }

        public Vector2i Offset { get; }
        public Vector2i Size { get; }

        public T this[int ix, int iy]
        {
            get => _points[ix + Offset.X, iy + Offset.Y];
            set => _points[ix + Offset.X, iy + Offset.Y] = value;
        }

        public T this[Vector2i i]
        {
            get => _points[i.X + Offset.X, i.Y + Offset.Y];
            set => _points[i.X + Offset.X, i.Y + Offset.Y] = value;
        }

        public bool IsSameSize(Vector2i size, Vector2i offset) => Size == size && this.Offset == offset;
    }
}