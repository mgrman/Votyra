using TycoonTerrain.Common.Models;

namespace TycoonTerrain.TerrainAlgorithms
{
    public interface ITerrainAlgorithm
    {
        bool RequiresWalls { get; }

        ResultHeightData Process(HeightData sampleData);
    }
}