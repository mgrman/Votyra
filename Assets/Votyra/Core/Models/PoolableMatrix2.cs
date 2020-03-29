using Votyra.Core.Models.ObjectPool;

namespace Votyra.Core.Models
{
    public class PoolableMatrix2<T>
    {
        private static readonly ConcurentObjectDictionaryPool<PoolableMatrix2<T>, Vector2i> Pool = new ConcurentObjectDictionaryPool<PoolableMatrix2<T>, Vector2i>(5, matrixSize => new PoolableMatrix2<T>(matrixSize));

        public readonly T[,] RawMatrix;

        private PoolableMatrix2(Vector2i matrixSize)
        {
            RawMatrix = new T[matrixSize.X, matrixSize.Y];
        }

        private Vector2i Size => RawMatrix.Size();

        public void Dispose()
        {
            Pool.ReturnObject(this, Size);
        }

        public static PoolableMatrix2<T> CreateDirty(Vector2i matrixSize)
        {
            var obj = Pool.GetObject(matrixSize);
            return obj;
        }
    }
}
