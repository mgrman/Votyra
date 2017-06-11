using Votyra.Models;

namespace Votyra.TerrainAlgorithms
{
    public interface ITerrainAlgorithm
    {
        ResultHeightData Process(HeightData sampleData);
    }
}