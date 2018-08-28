using Votyra.Core.Models;

namespace Votyra.Core.TerrainMeshes
{
    public interface ITerrainMesh
    {
        Vector3f this[int index] { get; }
        int TriangleCount { get; }

        void Clear(Range3f meshBounds);

        void AddTriangle(Vector3f a, Vector3f b, Vector3f c);

        void FinalizeMesh();
    }
}