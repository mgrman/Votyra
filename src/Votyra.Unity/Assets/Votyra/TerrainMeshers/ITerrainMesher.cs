using Votyra.Common.Models;
using Votyra.TerrainGenerators;
using Votyra.TerrainMeshers.TriangleMesh;

namespace Votyra.TerrainMeshers
{
    public interface ITerrainMesher
    {
        void Initialize(ITerrainContext terrainOptions);

        void InitializeGroup(Vector2i group, ITerrainMesh mesh, IMatrix<ResultHeightData> data);

        Vector2i CellInGroupCount { get; }

        void AddCell(Vector2i cellInGroup);
    }
}