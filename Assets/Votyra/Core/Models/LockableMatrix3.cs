using System;

namespace Votyra.Core.Models
{
    public class LockableMatrix3<T> : IMatrix3<T>
    {
        private T[,,] _points;

        //public readonly Vector2i offset;
        public Vector3i size { get; }

        private readonly object _syncLock = new object();

        private object _accessLock;

        public bool IsLocked
        {
            get
            {
                return _accessLock != null;
            }
        }

        public void Lock(object lockObject)
        {
            lock (_syncLock)
            {
                if (IsLocked)
                    throw new MatrixLockedException();

                _accessLock = lockObject;
            }
        }

        public void Unlock(object lockObject)
        {
            lock (_syncLock)
            {
                if (_accessLock != lockObject)
                    throw new MatrixNotLockedWithThisKeyException();

                _accessLock = null;
            }
        }

        public LockableMatrix3(Vector3i matrixSize)
        {
            _points = new T[matrixSize.x, matrixSize.y, matrixSize.z];
            //_points = new T[matrixSize.x+indicesOffset.x, matrixSize.y + indicesOffset.y];
            //offset = indicesOffset;
            size = matrixSize;
        }

        public bool IsSameSize(Vector3i size)
        {
            return this.size == size;//&& this.offset == offset;
        }

        public T this[int ix, int iy, int iz]
        {
            get
            {
                return _points[ix, iy, iz];
            }
            set
            {
                if (IsLocked)
                {
                    throw new MatrixLockedException();
                }
                _points[ix, iy, iz] = value;
            }
        }

        public T this[Vector3i i]
        {
            get
            {
                return _points[i.x, i.y, i.z];
            }
            set
            {
                if (IsLocked)
                {
                    throw new MatrixLockedException();
                }
                _points[i.x, i.y, i.z] = value;
            }
        }
    }

}