using Votyra.Common.Models;
using Votyra.TerrainGenerators;
using Votyra.TerrainMeshers.TriangleMesh;

namespace Votyra.TerrainMeshers
{
    public interface ITerrainMesher
    {
        void Initialize(ITerrainContext terrainOptions);

        void InitializeGroup(Vector2i group, ITriangleMesh mesh, IMatrix<ResultHeightData> data);

        int CellCount { get; }

        void AddCell(Vector2i cellInGroup);
    }
}