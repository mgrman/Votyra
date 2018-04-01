using System;

namespace Votyra.Core.Models
{
    public class LockableMatrix3<T> : IMatrix3<T>
    {
        private T[,,] _points;

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
            _points = new T[matrixSize.X, matrixSize.Y, matrixSize.Z];
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
                return _points[i.X, i.Y, i.Z];
            }
            set
            {
                if (IsLocked)
                {
                    throw new MatrixLockedException();
                }
                _points[i.X, i.Y, i.Z] = value;
            }
        }
    }

}