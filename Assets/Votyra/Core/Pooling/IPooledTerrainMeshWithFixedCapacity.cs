using Votyra.Core.TerrainMeshes;

namespace Votyra.Core.Pooling
{
    public interface IPooledTerrainMeshWithFixedCapacity : IPooledTerrainMesh, ITerrainMeshWithFixedCapacity
    {
        new ITerrainMeshWithFixedCapacity Mesh { get; }
    }
}