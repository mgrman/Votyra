namespace Votyra.Core.Models
{
    public class Matrix3<T> : IMatrix3<T>
    {
        public T[,,] NativeMatrix;

        public Vector3i size { get; }

        public Matrix3(Vector3i matrixSize)
        {
            NativeMatrix = new T[matrixSize.X, matrixSize.Y, matrixSize.Z];
            size = matrixSize;
        }

        public bool IsSameSize(Vector3i size)
        {
            return this.size == size;
        }

        public T this[int ix, int iy, int iz]
        {
            get
            {
                return NativeMatrix[ix, iy, iz];
            }
            set
            {
                NativeMatrix[ix, iy, iz] = value;
            }
        }

        public T this[Vector3i i]
        {
            get
            {
                return NativeMatrix[i.X, i.Y, i.Z];
            }
            set
            {
                NativeMatrix[i.X, i.Y, i.Z] = value;
            }
        }

        public T TryGet(Vector3i i, T defaultValue)
        {
            return size.ContainsIndex(i) ? NativeMatrix[i.X, i.Y, i.Z] : defaultValue;
        }
    }
}