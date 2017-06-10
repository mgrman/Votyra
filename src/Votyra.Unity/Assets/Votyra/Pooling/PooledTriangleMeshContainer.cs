using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Votyra.Common.Models.ObjectPool;
using Votyra.TerrainMeshers.TriangleMesh;

namespace Votyra.Unity.Assets.Votyra.Pooling
{
    public class PooledTriangleMeshContainer<T> : IPooledTriangleMesh
        where T : ITriangleMesh, new()
    {
        public T Mesh { get; }

        public int CellCount => Mesh.CellCount;

        ITriangleMesh IPooledTriangleMesh.Mesh => Mesh;

        private static readonly bool IsDisposable = typeof(IDisposable).IsAssignableFrom(typeof(T));

        private static readonly ConcurentObjectDictionaryPool<PooledTriangleMeshContainer<T>, int> Pool = new ConcurentObjectDictionaryPool<PooledTriangleMeshContainer<T>, int>(5, (cellCount) => new PooledTriangleMeshContainer<T>(cellCount));

        private PooledTriangleMeshContainer(int cellCount)
        {
            Mesh = new T();
            Mesh.Initialize(cellCount);
        }

        public static PooledTriangleMeshContainer<T> CreateDirty(int cellCount)
        {
            var obj = Pool.GetObject(cellCount);
            return obj;
        }

        public void Dispose()
        {
            if (IsDisposable)
            {
                (Mesh as IDisposable)?.Dispose();
            }
            Pool.ReturnObject(this, this.CellCount);
        }

        public void Clear(Bounds meshBounds)
        {
            Mesh.Clear(meshBounds);
        }

        public void AddQuad(int quadIndex, Vector3 x0y0, Vector3 x0y1, Vector3 x1y0, Vector3 x1y1, bool flipSides)
        {
            Mesh.AddQuad(quadIndex, x0y0, x0y1, x1y0, x1y1, flipSides);
        }

        public void AddWall(int quadIndex, Vector3 a, Vector3 b, Vector3 b_lower, Vector3 a_lower)
        {
            Mesh.AddWall(quadIndex, a, b, b_lower, a_lower);
        }

        public void Initialize(int cellCount)
        {
            throw new InvalidOperationException("Cannot initialize pooled mesh.");
        }
    }
}