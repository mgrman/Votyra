using System.Collections.Generic;
using TycoonTerrain.TerrainMeshers.TriangleMesh;

namespace TycoonTerrain.TerrainGenerators
{
    public interface ITerrainMeshGenerator:IGenerator<TerrainOptions, IList<ITriangleMesh>>
    {
    }
}