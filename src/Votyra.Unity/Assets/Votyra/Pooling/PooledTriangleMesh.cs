using System;
using System.Collections.Generic;
using UnityEngine;
using Votyra.Common.Models.ObjectPool;
using Votyra.TerrainMeshers.TriangleMesh;

namespace Votyra.Unity.Assets.Votyra.Pooling
{
    public class PooledTriangleMesh : FixedTriangleMesh, IDisposable
    {
        private readonly int _key;
        public static readonly ConcurentObjectDictionaryPool<PooledTriangleMesh, int> Pool = new ConcurentObjectDictionaryPool<PooledTriangleMesh, int>(100, triangleCount => new PooledTriangleMesh(triangleCount));

        private PooledTriangleMesh(int triangleCount)
        : base(triangleCount)
        {
            _key = triangleCount;
        }

        public static PooledTriangleMesh CreateDirty(int triangleCount)
        {
            var obj = Pool.GetObject(triangleCount);
            return obj;
        }

        public void Dispose()
        {
            Pool.ReturnObject(this, this._key);
        }
    }
}