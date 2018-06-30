namespace Votyra.Core.Models
{
    public class LockableMatrix2<T> : IMatrix2<T>
    {
        private readonly T[,] _points;

        private readonly object _syncLock = new object();

        private object _accessLock;

        public LockableMatrix2(Vector2i matrixSize)
        {
            _points = new T[matrixSize.X, matrixSize.Y];
            Size = matrixSize;
        }

        public bool IsLocked => _accessLock != null;
        public Vector2i Size { get; }

        public T this[Vector2i i]
        {
            get
            {
                return _points[i.X, i.Y];
            }
            set
            {
                if (IsLocked)
                {
                    throw new MatrixLockedException();
                }
                _points[i.X, i.Y] = value;
            }
        }

        public bool IsSameSize(Vector2i size)
        {
            return this.Size == size;
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
    }
}