using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TycoonTerrain.Common.Models;

namespace TycoonTerrain.Models
{
    class LockableMatrix<T> : IMatrix<T>
    {
        private T[,] _points;

        //public readonly Vector2i offset;
        public readonly Vector2i size;
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


        public LockableMatrix(Vector2i matrixSize)
        {
            _points = new T[matrixSize.x, matrixSize.y];
            //_points = new T[matrixSize.x+indicesOffset.x, matrixSize.y + indicesOffset.y];
            //offset = indicesOffset;
            size = matrixSize;
        }


        public bool IsSameSize(Vector2i size)
        {
            return this.size == size;//&& this.offset == offset;
        }

        public T this[int ix, int iy]
        {
            get
            {
                return _points[ix, iy];
            }
            set
            {
                if (IsLocked)
                {
                    throw new MatrixLockedException();
                }
                _points[ix, iy] = value;
            }
        }

        public T this[Vector2i i]
        {
            get
            {
                return _points[i.x, i.y];
            }
            set
            {
                if (IsLocked)
                {
                    throw new MatrixLockedException();
                }
                _points[i.x, i.y] = value;
            }
        }
    }

    public class MatrixNotLockedWithThisKeyException : Exception
    {

    }

    public class MatrixLockedException : Exception
    {

    }
}
