using System.Collections.Generic;
using Votyra.TerrainMeshers.TriangleMesh;

namespace Votyra.TerrainGenerators
{
    public interface ITerrainMeshGenerator:IGenerator<TerrainOptions, IList<ITriangleMesh>>
    {
    }
}