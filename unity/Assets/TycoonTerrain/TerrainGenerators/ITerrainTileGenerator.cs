using System.Collections.Generic;

namespace TycoonTerrain.TerrainGenerators
{
    public interface ITerrainTileGenerator : IGenerator<TerrainOptions, IList<ITerrainGroup>>
    {
    }
}