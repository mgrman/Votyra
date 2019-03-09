using System;
using Votyra.Core.Models.ObjectPool;
using Votyra.Core.TerrainMeshes;

namespace Votyra.Core.Pooling
{
    public class PooledTerrainMeshContainer<T> : IPooledTerrainMesh where T : ITerrainMesh, new()
    {
        private static readonly ConcurentObjectPool<PooledTerrainMeshContainer<T>> Pool = new ConcurentObjectPool<PooledTerrainMeshContainer<T>>(5, () => new PooledTerrainMeshContainer<T>());

        private PooledTerrainMeshContainer()
        {
            Mesh = new T();
        }

        public T Mesh { get; }

        ITerrainMesh IPooledTerrainMesh.Mesh => Mesh;

        public event Action<IPooledTerrainMesh> OnDispose; 
        
        public void Dispose()
        {
            OnDispose?.Invoke(this);
            OnDispose = null;
            Pool.ReturnObject(this);
        }

        public static PooledTerrainMeshContainer<T> CreateDirty()
        {
            var obj = Pool.GetObject();
            return obj;
        }

        public void Initialize(int triangleCount)
        {
            throw new InvalidOperationException("Cannot initialize pooled mesh.");
        }
    }
}