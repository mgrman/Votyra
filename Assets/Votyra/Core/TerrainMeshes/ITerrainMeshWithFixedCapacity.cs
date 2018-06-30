using Votyra.Core.Models;

namespace Votyra.Core.TerrainMeshes
{
    public interface ITerrainMeshWithFixedCapacity : ITerrainMesh
    {
        void Initialize(int triangleCapacity);

        int TriangleCapacity { get; }
    }
}