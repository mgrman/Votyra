using Votyra.Core.Models.ObjectPool;
using Votyra.Core.TerrainMeshes;

namespace Votyra.Core.Pooling
{
    public class PooledTerrainMeshWithFixedCapacityContainer<T> : IPooledTerrainMeshWithFixedCapacity where T : ITerrainMeshWithFixedCapacity, new()
    {
        private static readonly ConcurentObjectDictionaryPool<PooledTerrainMeshWithFixedCapacityContainer<T>, int> Pool = new ConcurentObjectDictionaryPool<PooledTerrainMeshWithFixedCapacityContainer<T>, int>(5, triangleCount => new PooledTerrainMeshWithFixedCapacityContainer<T>(triangleCount));

        private PooledTerrainMeshWithFixedCapacityContainer(int triangleCount)
        {
            Mesh = new T();
            Mesh.Initialize(triangleCount);
        }

        public T Mesh { get; }

        ITerrainMesh IPooledTerrainMesh.Mesh => Mesh;

        ITerrainMeshWithFixedCapacity IPooledTerrainMeshWithFixedCapacity.Mesh => Mesh;

        public void Dispose()
        {
            Pool.ReturnObject(this, Mesh.TriangleCount);
        }

        public static PooledTerrainMeshWithFixedCapacityContainer<T> CreateDirty(int triangleCount)
        {
            var obj = Pool.GetObject(triangleCount);
            return obj;
        }
    }
}