using Votyra.Core.Models;
using Votyra.Core.TerrainMeshes;

namespace Votyra.Core
{
    public interface ITerrainGeneratorManager2i
    {
        ITerrainMesh GetMeshForGroup(Vector2i group);
    }
}