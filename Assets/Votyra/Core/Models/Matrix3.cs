namespace Votyra.Core.Models
{
    public class Matrix3<T> : IMatrix3<T>
    {
        public T[,,] NativeMatrix;

        public Vector3i Size { get; }

        public Matrix3(Vector3i matrixSize)
        {
            NativeMatrix = new T[matrixSize.X, matrixSize.Y, matrixSize.Z];
            Size = matrixSize;
        }

        public bool IsSameSize(Vector3i size)
        {
            return this.Size == size;
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
    }
}