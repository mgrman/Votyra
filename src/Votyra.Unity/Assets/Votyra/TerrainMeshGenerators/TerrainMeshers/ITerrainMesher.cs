using Votyra.Models;
using Votyra.Unity.Assets.Votyra.Pooling;

namespace Votyra.TerrainMeshGenerators.TerrainMeshers
{
    public interface ITerrainMesher
    {
        void AddCell(Vector2i cellInGroup);
        IPooledTerrainMesh GetResultingMesh();
        void Initialize(ITerrainMeshContext terrainOptions);
        void InitializeGroup(Vector2i group, IMatrix<ResultHeightData> data);
    }
}