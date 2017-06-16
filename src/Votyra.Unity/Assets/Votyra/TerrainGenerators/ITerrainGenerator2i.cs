using System.Collections.Generic;
using Votyra.Models;
using Votyra.TerrainGenerators.TerrainMeshers.TerrainMeshes;
using Votyra.Unity.Assets.Votyra.Pooling;

namespace Votyra.TerrainGenerators
{
    public interface ITerrainGenerator2i
    {
        IReadOnlyPooledDictionary<Vector2i, ITerrainMesh2i> Generate(ITerrainGeneratorContext2i options, IEnumerable<Vector2i> groupsToUpdate);
    }
}