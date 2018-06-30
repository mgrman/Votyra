using System;
using Votyra.Core.Models;
using Votyra.Core.Models.ObjectPool;
using Votyra.Core.TerrainMeshes;

namespace Votyra.Core.Pooling
{
    public class PooledTerrainMeshWithFixedCapacityContainer<T> : IPooledTerrainMeshWithFixedCapacity
        where T : ITerrainMeshWithFixedCapacity, new()
    {
        private static readonly bool IsDisposable = typeof(IDisposable).IsAssignableFrom(typeof(T));
        private static readonly ConcurentObjectDictionaryPool<PooledTerrainMeshWithFixedCapacityContainer<T>, int> Pool = new ConcurentObjectDictionaryPool<PooledTerrainMeshWithFixedCapacityContainer<T>, int>(5, (triangleCount) => new PooledTerrainMeshWithFixedCapacityContainer<T>(triangleCount));

        private PooledTerrainMeshWithFixedCapacityContainer(int triangleCount)
        {
            Mesh = new T();
            Mesh.Initialize(triangleCount);
        }

        public T Mesh { get; }

        ITerrainMesh IPooledTerrainMesh.Mesh => Mesh;
        ITerrainMeshWithFixedCapacity IPooledTerrainMeshWithFixedCapacity.Mesh => Mesh;
        public int TriangleCapacity => Mesh.TriangleCapacity;
        public int TriangleCount => Mesh.TriangleCount;

        public static PooledTerrainMeshWithFixedCapacityContainer<T> CreateDirty(int triangleCount)
        {
            var obj = Pool.GetObject(triangleCount);
            return obj;
        }

        public void AddTriangle(Vector3f a, Vector3f b, Vector3f c)
        {
            Mesh.AddTriangle(a, b, c);
        }

        public void Clear(Range3f meshBounds)
        {
            Mesh.Clear(meshBounds);
        }

        public void Dispose()
        {
            if (IsDisposable)
            {
                (Mesh as IDisposable)?.Dispose();
            }
            Pool.ReturnObject(this, this.TriangleCount);
        }

        public void FinalizeMesh()
        {
            Mesh.FinalizeMesh();
        }

        public void Initialize(int triangleCount)
        {
            throw new InvalidOperationException("Cannot initialize pooled mesh.");
        }
    }
}