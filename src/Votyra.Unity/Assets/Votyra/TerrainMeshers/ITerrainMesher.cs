using Votyra.Models;
using Votyra.TerrainGenerators;
using Votyra.TerrainMeshers.TriangleMesh;
using Votyra.Unity.Assets.Votyra.Pooling;

namespace Votyra.TerrainMeshers
{
    public interface ITerrainMesher
    {
        void Initialize(ITerrainContext terrainOptions);

        void InitializeGroup(Vector2i group, IMatrix<ResultHeightData> data);

        void AddCell(Vector2i cellInGroup);

        IPooledTriangleMesh GetResultingMesh();
    }
}