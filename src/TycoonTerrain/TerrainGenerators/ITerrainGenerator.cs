using System.Collections.Generic;
using TycoonTerrain.TerrainMeshers.TriangleMesh;

namespace TycoonTerrain.TerrainGenerators
{
    public interface ITerrainGenerator
    {
        IList<ITriangleMesh> Generate(TerrainOptions options);
    }
}