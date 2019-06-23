using System;
using System.Threading.Tasks;
using Votyra.Core.Models;
using Votyra.Core.TerrainMeshes;

namespace Votyra.Core
{
    public interface IUnityTerrainGeneratorManager2i
    {
        event Action<Vector2i, ITerrainMesh2f> NewTerrain;
        event Action<Vector2i> ChangedTerrain;
        event Action<Vector2i> RemovedTerrain;
    }
}