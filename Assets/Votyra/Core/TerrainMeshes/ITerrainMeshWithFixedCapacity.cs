namespace Votyra.Core.TerrainMeshes
{
    public interface ITerrainMeshWithFixedCapacity : ITerrainMesh
    {
        int TriangleCapacity { get; }

        void Initialize(int triangleCapacity);
    }
}