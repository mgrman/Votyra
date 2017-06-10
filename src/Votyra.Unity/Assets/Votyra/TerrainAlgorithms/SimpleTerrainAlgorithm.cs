using System;
using System.Collections.Generic;
using System.Linq;
using Votyra.Common.Models;

namespace Votyra.TerrainAlgorithms
{
    public class SimpleTerrainAlgorithm : ITerrainAlgorithm
    {
        public ResultHeightData Process(HeightData sampleData)
        {
            var difMain = Math.Abs(sampleData.x0y0 - sampleData.x1y1);
            var difMinor = Math.Abs(sampleData.x1y0 - sampleData.x0y1);
            bool flip;
            if (difMain == difMinor)
            {
                var sumMain = Math.Abs(sampleData.x0y0 + sampleData.x1y1);
                var sumMinor = Math.Abs(sampleData.x1y0 + sampleData.x0y1);
                flip = sumMain < sumMinor;
            }
            else
            {
                flip = difMain < difMinor;
            }
            return new ResultHeightData(sampleData, flip);
        }
    }
}