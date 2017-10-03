using UnityEngine;
using Votyra.Core.Models;

namespace Votyra.Core.TerrainMeshes
{
    public interface ITerrainMesh2i : ITerrainMesh
    {
        Vector2i CellInGroupCount { get; }
        void Initialize(Vector2i cellInGroupCount);
    }
}
