using System.Collections.Generic;
using Votyra.Models;
using Votyra.TerrainGenerators.TerrainMeshers.TerrainMeshes;
using Votyra.Unity.Assets.Votyra.Pooling;

namespace Votyra.TerrainGenerators
{
    public interface ITerrainGenerator3b
    {
        IReadOnlyPooledDictionary<Vector3i, ITerrainMesh> Generate(ITerrainGeneratorContext3b options, IEnumerable<Vector3i> groupsToUpdate);
    }
}