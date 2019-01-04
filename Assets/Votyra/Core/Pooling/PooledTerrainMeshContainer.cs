using System;
using Votyra.Core.Models;
using Votyra.Core.Models.ObjectPool;
using Votyra.Core.TerrainMeshes;

namespace Votyra.Core.Pooling
{
    public class PooledTerrainMeshContainer<T> : IPooledTerrainMesh
        where T : ITerrainMesh, new()
    {
        private static readonly bool IsDisposable = typeof(IDisposable).IsAssignableFrom(typeof(T));
        private static readonly ConcurentObjectPool<PooledTerrainMeshContainer<T>> Pool = new ConcurentObjectPool<PooledTerrainMeshContainer<T>>(5, () => new PooledTerrainMeshContainer<T>());

        private PooledTerrainMeshContainer()
        {
            Mesh = new T();
        }

        public T Mesh { get; }

        public int VertexCount => Mesh.VertexCount;

        public int TriangleCount => Mesh.TriangleCount;

        ITerrainMesh IPooledTerrainMesh.Mesh => Mesh;


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

        public void Clear(Area3f meshBounds, Func<Vector3f, Vector3f> vertexPostProcessor, Func<Vector2f, Vector2f> uvAdjustor)
        {
            Mesh.Clear(meshBounds, vertexPostProcessor, uvAdjustor);
        }

        public void AddTriangle(Vector3f a, Vector3f b, Vector3f c)
        {
            Mesh.AddTriangle(a, b, c);
        }

        public void Initialize(int triangleCount)
        {
            throw new InvalidOperationException("Cannot initialize pooled mesh.");
        }

        public void FinalizeMesh()
        {
            Mesh.FinalizeMesh();
        }
    }
}