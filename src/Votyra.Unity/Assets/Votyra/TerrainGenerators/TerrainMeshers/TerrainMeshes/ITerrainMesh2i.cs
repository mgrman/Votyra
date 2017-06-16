using UnityEngine;
using Votyra.Models;

namespace Votyra.TerrainGenerators.TerrainMeshers.TerrainMeshes
{
    public interface ITerrainMesh2i
    {
        Vector2i CellInGroupCount { get; }
        void Initialize(Vector2i CellInGroupCount);
        void Clear(Bounds meshBounds);

        void AddQuad(Vector2i cellInGroup, Vector3i x0y0, Vector3i x0y1, Vector3i x1y0, Vector3i x1y1, bool flipSides);
        void AddWallX(Vector2i cellInGroup, Vector3i a, Vector3i b, Vector3i b_lower, Vector3i a_lower);
        void AddWallY(Vector2i cellInGroup, Vector3i a, Vector3i b, Vector3i b_lower, Vector3i a_lower);
    }
}