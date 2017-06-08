using System;
using System.Collections.Generic;
using System.Linq;
using Votyra.Common.Models;

namespace Votyra.TerrainAlgorithms
{
    public class SimpleTerrainAlgorithm : ITerrainAlgorithm
    {
        public bool RequiresWalls { get { return true; } }

        public ResultHeightData Process(HeightData sampleData)
        {
            bool flip = (sampleData.x0y0 + sampleData.x1y1) > (sampleData.x1y0 + sampleData.x0y1);

            return new ResultHeightData(sampleData, flip);
        }
    }
}