using UnityEngine;
using Votyra.Models;

namespace Votyra.TerrainGenerators.TerrainMeshers.TerrainMeshes
{
    public interface ITerrainMesh2i : ITerrainMesh
    {
        Vector2i CellInGroupCount { get; }
        void Initialize(Vector2i cellInGroupCount);
    }
}