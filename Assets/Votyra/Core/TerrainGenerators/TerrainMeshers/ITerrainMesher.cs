using Votyra.Core.Images;
using Votyra.Core.Models;
using Votyra.Core.TerrainMeshes;

namespace Votyra.Core.TerrainGenerators.TerrainMeshers
{
    public interface ITerrainMesher
    {
        void UpdateMesh(ITerrainMesh mesh, Vector2i group, IImage2f image);
    }
}