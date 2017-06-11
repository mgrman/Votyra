using System.Collections.Generic;
using Votyra.Models;
using Votyra.Unity.Assets.Votyra.Pooling;

namespace Votyra.TerrainGenerators
{
    public interface ITerrainTileGenerator
    {
        IReadOnlyPooledCollection<ITerrainGroup> Generate(ITerrainContext options, IEnumerable<Vector2i> groupsToUpdate);
    }
}