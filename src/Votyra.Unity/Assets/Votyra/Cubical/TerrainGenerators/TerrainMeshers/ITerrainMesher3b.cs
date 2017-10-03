


using Votyra.Core.Models;
using Votyra.Core.Pooling;

namespace Votyra.Cubical.TerrainGenerators.TerrainMeshers
{
    public interface ITerrainMesher3b
    {
        void AddCell(Vector3i cellInGroup);
        IPooledTerrainMesh GetResultingMesh();
        void Initialize(ITerrainGeneratorContext3b terrainOptions);
        void InitializeGroup(Vector3i group);
    }
}
