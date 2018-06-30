namespace Votyra.Core.Models
{
    public class LockableMatrix3<T> : IMatrix3<T>
    {
        private readonly T[,,] _points;

        private readonly object _syncLock = new object();

        private object _accessLock;

        public Vector3i Size { get; }

        public bool IsLocked => _accessLock != null;

        public LockableMatrix3(Vector3i matrixSize)
        {
            _points = new T[matrixSize.X, matrixSize.Y, matrixSize.Z];
            Size = matrixSize;
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

        public bool IsSameSize(Vector3i size)
        {
            return this.Size == size;
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