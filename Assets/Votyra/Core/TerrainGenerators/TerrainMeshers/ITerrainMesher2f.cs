using Votyra.Core.Images;
using Votyra.Core.Models;
using Votyra.Core.TerrainMeshes;

namespace Votyra.Core.TerrainGenerators.TerrainMeshers
{
    public interface ITerrainMesher2f
    {
        void GetResultingMesh(ITerrainMesh mesh, Vector2i group, IImage2f image, IMask2e mask);
    }
}