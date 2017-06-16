using UnityEngine;
using Votyra.Models;

namespace Votyra.TerrainGenerators.TerrainMeshers.TerrainMeshes
{
    public interface ITerrainMesh2
    {
        Vector2i CellInGroupCount { get; }
        void Initialize(Vector2i CellInGroupCount);
        void Clear(Bounds meshBounds);

        void AddQuad(Vector2i cellInGroup, Vector3 x0y0, Vector3 x0y1, Vector3 x1y0, Vector3 x1y1, bool flipSides);
        void AddWallX(Vector2i cellInGroup, Vector3 a, Vector3 b, Vector3 b_lower, Vector3 a_lower);
        void AddWallY(Vector2i cellInGroup, Vector3 a, Vector3 b, Vector3 b_lower, Vector3 a_lower);
    }
}