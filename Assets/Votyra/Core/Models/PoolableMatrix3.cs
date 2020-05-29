using Votyra.Core.Models.ObjectPool;

namespace Votyra.Core.Models
{
    public class PoolableMatrix3<T>
    {
        private static readonly ConcurentObjectDictionaryPool<PoolableMatrix3<T>, Vector3i> Pool = new ConcurentObjectDictionaryPool<PoolableMatrix3<T>, Vector3i>(5, matrixSize => new PoolableMatrix3<T>(matrixSize));

        public readonly T[,,] RawMatrix;

        private PoolableMatrix3(Vector3i matrixSize)
        {
            this.RawMatrix = new T[matrixSize.X, matrixSize.Y, matrixSize.Z];
        }

        private Vector3i Size => this.RawMatrix.Size();

        public void Dispose()
        {
            Pool.ReturnObject(this, this.Size);
        }

        public static PoolableMatrix3<T> CreateDirty(Vector3i matrixSize)
        {
            var obj = Pool.GetObject(matrixSize);
            return obj;
        }
    }
}
