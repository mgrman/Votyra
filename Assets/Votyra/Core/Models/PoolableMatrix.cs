using Votyra.Core.Models.ObjectPool;

namespace Votyra.Core.Models
{
    public class PoolableMatrix<T> : IPoolableMatrix2<T>
    {
        private static readonly ConcurentObjectDictionaryPool<PoolableMatrix<T>, Vector2i> Pool = new ConcurentObjectDictionaryPool<PoolableMatrix<T>, Vector2i>(5, matrixSize => new PoolableMatrix<T>(matrixSize));


        public readonly T[,] RawMatrix;

        private PoolableMatrix(Vector2i matrixSize)
        {
            RawMatrix = new T[matrixSize.X, matrixSize.Y];
            Size = matrixSize;
        }

        public void Dispose()
        {
            Pool.ReturnObject(this, Size);
        }

        public Vector2i Size { get; }

        public T this[int ix, int iy]
        {
            get => RawMatrix[ix, iy];
            set => RawMatrix[ix, iy] = value;
        }

        public T this[Vector2i i]
        {
            get => RawMatrix[i.X, i.Y];
            set => RawMatrix[i.X, i.Y] = value;
        }

        public static PoolableMatrix<T> CreateDirty(Vector2i matrixSize)
        {
            var obj = Pool.GetObject(matrixSize);
            return obj;
        }
    }
}