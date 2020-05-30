using System;
using Votyra.Core.Models;
using Votyra.Core.TerrainMeshes;

namespace Votyra.Core
{
    public interface IUnityTerrainGeneratorManager2I
    {
        event Action<Vector2i, ITerrainMesh2F> NewTerrain;

        event Action<Vector2i, ITerrainMesh2F> ChangedTerrain;

        event Action<Vector2i, ITerrainMesh2F> RemovedTerrain;
    }
}
