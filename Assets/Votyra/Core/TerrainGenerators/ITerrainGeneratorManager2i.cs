using System;
using Votyra.Core.Models;
using Votyra.Core.TerrainMeshes;

namespace Votyra.Core
{
    public interface ITerrainGeneratorManager2i
    {
        ITerrainMesh2f GetMeshForGroup(Vector2i group);

        event Action<Vector2i, ITerrainMesh2f> NewTerrain;
        event Action<Vector2i> ChangedTerrain;
        event Action<Vector2i> RemovedTerrain;
    }
}