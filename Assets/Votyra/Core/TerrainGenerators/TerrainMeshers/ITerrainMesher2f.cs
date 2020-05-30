using Votyra.Core.Images;
using Votyra.Core.Models;
using Votyra.Core.TerrainMeshes;

namespace Votyra.Core.TerrainGenerators.TerrainMeshers
{
    public interface ITerrainMesher2F
    {
        void GetResultingMesh(ITerrainMesh2F mesh, Vector2i group, IImage2F image);
    }
}
