using Votyra.Models;
using Votyra.Unity.Assets.Votyra.Pooling;

namespace Votyra.Plannar.TerrainGenerators.TerrainMeshers
{
    public interface ITerrainMesher2i
    {
        void AddCell(Vector2i cellInGroup);
        IPooledTerrainMesh2i GetResultingMesh();
        void Initialize(ITerrainGeneratorContext2i terrainOptions);
        void InitializeGroup(Vector2i group);
    }
}
