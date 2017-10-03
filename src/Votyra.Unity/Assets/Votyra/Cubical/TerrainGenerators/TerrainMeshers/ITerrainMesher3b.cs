using Votyra.Models;
using Votyra.Unity.Assets.Votyra.Pooling;

namespace Votyra.TerrainGenerators.TerrainMeshers
{
    public interface ITerrainMesher3b
    {
        void AddCell(Vector3i cellInGroup);
        IPooledTerrainMesh GetResultingMesh();
        void Initialize(ITerrainGeneratorContext3b terrainOptions);
        void InitializeGroup(Vector3i group);
    }
}