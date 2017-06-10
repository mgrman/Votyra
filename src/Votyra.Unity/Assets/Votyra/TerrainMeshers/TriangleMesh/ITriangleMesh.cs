using UnityEngine;

namespace Votyra.TerrainMeshers.TriangleMesh
{
    public interface ITriangleMesh
    {
        int CellCount { get; }
        void Initialize(int triangleCount);
        void Clear(Bounds meshBounds);

        void AddQuad(int quadIndex, Vector3 x0y0, Vector3 x0y1, Vector3 x1y0, Vector3 x1y1, bool flipSides);
        void AddWall(int quadIndex, Vector3 a, Vector3 b, Vector3 b_lower, Vector3 a_lower);
    }
}