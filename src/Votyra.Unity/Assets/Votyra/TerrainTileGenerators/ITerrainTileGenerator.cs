using System.Collections.Generic;
using Votyra.Models;
using Votyra.Unity.Assets.Votyra.Pooling;
using Votyra.TerrainTileGenerators.TerrainGroups;

namespace Votyra.TerrainTileGenerators
{
    public interface ITerrainTileGenerator
    {
        IReadOnlyPooledCollection<ITerrainGroup> Generate(ITerrainTileContext options, IEnumerable<Vector2i> groupsToUpdate);
    }
}