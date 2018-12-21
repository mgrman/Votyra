using Votyra.Core.Models;

namespace Votyra.Core.TerrainMeshes
{
    public interface ITerrainMesh
    {
        Vector3f Offset { get; }
        int VertexCount { get; }
        int TriangleCount { get; }
        Vector3f this[int index] { get; }

        void Clear(Range3f meshBounds, Vector3f offset);

        void AddTriangle(Vector3f a, Vector3f b, Vector3f c);

        void FinalizeMesh();
    }
}