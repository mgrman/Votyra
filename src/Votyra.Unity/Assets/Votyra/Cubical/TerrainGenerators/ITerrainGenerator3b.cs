using System.Collections.Generic;
using Votyra.Core.Models;
using Votyra.Core.Pooling;
using Votyra.Core.TerrainMeshes;

namespace Votyra.Cubical.TerrainGenerators
{
    public interface ITerrainGenerator3b
    {
        IReadOnlyPooledDictionary<Vector3i, ITerrainMesh> Generate(ITerrainGeneratorContext3b options, IEnumerable<Vector3i> groupsToUpdate);
    }
}
