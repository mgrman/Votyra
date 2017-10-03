using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Votyra.Core.Models.ObjectPool;
using Votyra.Core.TerrainMeshes;

namespace Votyra.Core.Pooling
{
    public class PooledTerrainMeshContainer<T> : IPooledTerrainMesh
        where T : ITerrainMesh, new()
    {
        public T Mesh { get; }

        public int TriangleCount => Mesh.TriangleCount;

        ITerrainMesh IPooledTerrainMesh.Mesh => Mesh;

        private static readonly bool IsDisposable = typeof(IDisposable).IsAssignableFrom(typeof(T));

        private static readonly ConcurentObjectPool<PooledTerrainMeshContainer<T>> Pool = new ConcurentObjectPool<PooledTerrainMeshContainer<T>>(5, () => new PooledTerrainMeshContainer<T>());

        private PooledTerrainMeshContainer()
        {
            Mesh = new T();
        }

        public static PooledTerrainMeshContainer<T> CreateDirty()
        {
            var obj = Pool.GetObject();
            return obj;
        }

        public void Dispose()
        {
            if (IsDisposable)
            {
                (Mesh as IDisposable)?.Dispose();
            }
            Pool.ReturnObject(this);
        }

        public void Clear(Bounds meshBounds)
        {
            Mesh.Clear(meshBounds);
        }
        public void AddTriangle(Vector3 a, Vector3 b, Vector3 c)
        {
            Mesh.AddTriangle(a, b, c);
        }
    }
}
