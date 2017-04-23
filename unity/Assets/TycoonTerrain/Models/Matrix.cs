namespace TycoonTerrain.Common.Models
{
    public class Matrix<T> : IMatrix<T>
    {
        private T[,] _points;

        //public readonly Vector2i offset;
        public readonly Vector2i size;

        public Matrix(Vector2i matrixSize)//, Vector2i indicesOffset)
        {
            _points = new T[matrixSize.x, matrixSize.y];
            //_points = new T[matrixSize.x+indicesOffset.x, matrixSize.y + indicesOffset.y];
            //offset = indicesOffset;
            size = matrixSize;
        }

        public bool IsSameSize(Vector2i size)
        {
            return this.size == size;//&& this.offset == offset;
        }

        public T this[int ix, int iy]
        {
            get
            {
                return _points[ix, iy];
            }
            set
            {
                _points[ix, iy] = value;
            }
        }

        public T this[Vector2i i]
        {
            get
            {
                return _points[i.x, i.y];
            }
            set
            {
                _points[i.x, i.y] = value;
            }
        }
    }
}