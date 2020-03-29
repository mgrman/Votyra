using Votyra.Core.Models;

namespace Votyra.Core.TerrainGenerators.TerrainMeshers
{
    public interface ITerrainVertexPostProcessor
    {
        Vector3f PostProcessVertex(Vector3f vertex);
    }
}
