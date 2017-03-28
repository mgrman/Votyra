namespace TycoonTerrain.Common.Models
{
    public class MatrixWithOffset<T> : IMatrix<T>
    {
        private T[,] _points;
        public readonly Vector2i offset;
        public readonly Vector2i size;

        public MatrixWithOffset(Vector2i matrixSize, Vector2i indicesOffset)
        {
            _points = new T[matrixSize.x + indicesOffset.x, matrixSize.y + indicesOffset.y];
            offset = indicesOffset;
            size = matrixSize;
        }

        public bool IsSameSize(Vector2i size, Vector2i offset)
        {
            return this.size == size && this.offset == offset;
        }

        public T this[int ix, int iy]
        {
            get
            {
                return _points[ix + offset.x, iy + offset.y];
            }
            set
            {
                _points[ix + offset.x, iy + offset.y] = value;
            }
        }

        public T this[Vector2i i]
        {
            get
            {
                return _points[i.x + offset.x, i.y + offset.y];
            }
            set
            {
                _points[i.x + offset.x, i.y + offset.y] = value;
            }
        }
    }
}