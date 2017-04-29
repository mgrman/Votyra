using System.Collections.Generic;

namespace Votyra.TerrainGenerators
{
    public interface ITerrainTileGenerator : IGenerator<TerrainOptions, IList<ITerrainGroup>>
    {
    }
}