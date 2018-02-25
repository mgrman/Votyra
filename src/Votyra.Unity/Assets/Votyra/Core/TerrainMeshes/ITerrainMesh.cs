using UnityEngine;

namespace Votyra.Core.TerrainMeshes
{
    public interface ITerrainMesh
    {
        int TriangleCount { get; }

        void Clear(Bounds meshBounds);

        void AddTriangle(Vector3 a, Vector3 b, Vector3 c);
    }
}