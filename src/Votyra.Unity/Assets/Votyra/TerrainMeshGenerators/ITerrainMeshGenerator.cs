using System.Collections.Generic;
using Votyra.Models;
using Votyra.TerrainMeshers.TriangleMesh;
using Votyra.Unity.Assets.Votyra.Pooling;
using Votyra.TerrainTileGenerators.TerrainGroups;

namespace Votyra.TerrainMeshGenerators
{
    public interface ITerrainMeshGenerator
    {
        IReadOnlyPooledDictionary<Vector2i, ITerrainMesh> Generate(ITerrainMeshContext options, IReadOnlyPooledCollection<ITerrainGroup> terrainGroups);
    }
}