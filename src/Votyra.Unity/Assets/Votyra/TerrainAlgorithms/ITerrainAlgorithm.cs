using Votyra.Common.Models;

namespace Votyra.TerrainAlgorithms
{
    public interface ITerrainAlgorithm
    {
        bool RequiresWalls { get; }

        ResultHeightData Process(HeightData sampleData);
    }
}