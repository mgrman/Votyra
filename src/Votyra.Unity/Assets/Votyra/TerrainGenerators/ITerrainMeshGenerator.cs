using System.Collections.Generic;
using Votyra.Common.Models;
using Votyra.TerrainMeshers.TriangleMesh;

namespace Votyra.TerrainGenerators
{
    public interface ITerrainMeshGenerator : IGenerator<TerrainOptions, IReadOnlyDictionary<Vector2i, ITriangleMesh>>
    {
    }
}