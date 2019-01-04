using System;
using UnityEngine;
using Votyra.Core.Models.ObjectPool;
using Votyra.Core.Pooling;

namespace Votyra.Core.Models
{
    public class PoolableMatrix<T>: IPoolableMatrix2<T>
    {
        private static readonly ConcurentObjectDictionaryPool<PoolableMatrix<T>, Vector2i> Pool
            = new ConcurentObjectDictionaryPool<PoolableMatrix<T>, Vector2i>(5,
                (matrixSize) => new PoolableMatrix<T>(matrixSize));

        
        private readonly T[,] _points;

        private PoolableMatrix(Vector2i matrixSize)
        {
            _points = new T[matrixSize.X, matrixSize.Y];
            Size = matrixSize;
        }

        public static PoolableMatrix<T> CreateDirty(Vector2i matrixSize)
        {
            var obj = Pool.GetObject(matrixSize);
            return obj;
        }

        public void Dispose()
        {
            Pool.ReturnObject(this, this.Size);
        }

        public Vector2i Size { get; }

        public T this[int ix, int iy]
        {
            get => _points[ix, iy];
            set => _points[ix, iy] = value;
        }

        public T this[Vector2i i]
        {
            get => _points[i.X, i.Y];
            set => _points[i.X, i.Y] = value;
        }
    }
}