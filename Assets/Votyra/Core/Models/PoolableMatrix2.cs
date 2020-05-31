using Votyra.Core.Models.ObjectPool;

namespace Votyra.Core.Models
{
    public class PoolableMatrix2<T>
    {
        private static readonly ConcurentObjectDictionaryPool<PoolableMatrix2<T>, Vector2i> Pool = new ConcurentObjectDictionaryPool<PoolableMatrix2<T>, Vector2i>(5, matrixSize => new PoolableMatrix2<T>(matrixSize));

        private PoolableMatrix2(Vector2i matrixSize)
        {
            this.RawMatrix = new T[matrixSize.X, matrixSize.Y];
        }

        public T[,] RawMatrix { get; }

        private Vector2i Size => this.RawMatrix.Size();

        public static PoolableMatrix2<T> CreateDirty(Vector2i matrixSize)
        {
            var obj = Pool.GetObject(matrixSize);
            return obj;
        }

        public void Dispose()
        {
            Pool.ReturnObject(this, this.Size);
        }
    }
}
