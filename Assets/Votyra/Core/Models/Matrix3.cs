namespace Votyra.Core.Models
{
    public class Matrix3<T> : IMatrix3<T>
    {

        public T[,,] NativeMatrix;

        //public readonly Vector2i offset;
        public Vector3i size { get; }

        public Matrix3(Vector3i matrixSize) //, Vector2i indicesOffset)
        {
            NativeMatrix = new T[matrixSize.x, matrixSize.y, matrixSize.z];
            //_points = new T[matrixSize.x+indicesOffset.x, matrixSize.y + indicesOffset.y];
            //offset = indicesOffset;
            size = matrixSize;
        }

        public bool IsSameSize(Vector3i size)
        {
            return this.size == size; //&& this.offset == offset;
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
                return NativeMatrix[i.x, i.y, i.z];
            }
            set
            {
                NativeMatrix[i.x, i.y, i.z] = value;
            }
        }

        public T TryGet(Vector3i i, T defaultValue)
        {
            return i.IsAsIndexContained(size) ? NativeMatrix[i.x, i.y, i.z] : defaultValue;
        }
    }
}