using Votyra.Core.Models;

namespace Votyra.Core.TerrainMeshes
{
    public interface ITerrainMesh
    {
        int TriangleCount { get; }

        void Clear(Rect3f meshBounds);

        void AddTriangle(Vector3f a, Vector3f b, Vector3f c);
    }
}