using Votyra.Common.Models;

namespace Votyra.TerrainAlgorithms
{
    public interface ITerrainAlgorithm
    {
        ResultHeightData Process(HeightData sampleData);
    }
}