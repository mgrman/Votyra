using System.Collections.Generic;
using Votyra.Core.Models;
using Votyra.Core.Pooling;
using Votyra.Core.TerrainMeshes;

namespace Votyra.Core.TerrainGenerators
{
    public interface ITerrainGenerator2i
    {
        IReadOnlyPooledDictionary<Vector2i, ITerrainMesh> Generate(ITerrainGeneratorContext2i options, IEnumerable<Vector2i> groupsToUpdate);
    }
}