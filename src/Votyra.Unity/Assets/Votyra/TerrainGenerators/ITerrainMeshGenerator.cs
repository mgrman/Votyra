using System.Collections.Generic;
using Votyra.Common.Models;
using Votyra.TerrainMeshers.TriangleMesh;
using Votyra.Unity.Assets.Votyra.Pooling;

namespace Votyra.TerrainGenerators
{
    public interface ITerrainMeshGenerator
    {
        IReadOnlyPooledDictionary<Vector2i, ITriangleMesh> Generate(ITerrainContext options, IEnumerable<Vector2i> groupsToUpdate);
    }
}