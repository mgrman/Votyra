using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.Profiling;

public class TileSelectTerrainAlgorithm : MonoBehaviour, ITerrainAlgorithm
{
    public bool RequiresWalls { get { return true; } }

    public HeightData Process(HeightData sampleData)
    {
        int height = sampleData.Max - 1;
        
        HeightData normalizedHeightData = new HeightData(Math.Max(sampleData.x0y0 - height, -1),
            Math.Max(sampleData.x0y1 - height, -1),
            Math.Max(sampleData.x1y0 - height, -1),
            Math.Max(sampleData.x1y1 - height, -1));
        
        HeightData choosenTemplateTile = TileMap[normalizedHeightData]; 
      
        var resultingHeightData = choosenTemplateTile+ height;

        return resultingHeightData;
    }


    private readonly static Dictionary<HeightData, HeightData> TileMap = new Dictionary<HeightData, HeightData>()
        {
{new HeightData(-1,-1,-1,-1),new HeightData(-1,0,0,1)},
{new HeightData(-1,-1,-1,0),new HeightData(-1,0,0,1)},
{new HeightData(-1,-1,-1,1),new HeightData(-1,0,0,1)},
{new HeightData(-1,-1,0,-1),new HeightData(-1,0,0,1)},
{new HeightData(-1,-1,0,0),new HeightData(-1,0,0,1)},
{new HeightData(-1,-1,0,1),new HeightData(-1,0,0,1)},
{new HeightData(-1,-1,1,-1),new HeightData(0,-1,1,0)},
{new HeightData(-1,-1,1,0),new HeightData(0,-1,1,0)},
{new HeightData(-1,-1,1,1),new HeightData(0,0,1,1)},
{new HeightData(-1,0,-1,-1),new HeightData(-1,0,0,1)},
{new HeightData(-1,0,-1,0),new HeightData(-1,0,0,1)},
{new HeightData(-1,0,-1,1),new HeightData(-1,0,0,1)},
{new HeightData(-1,0,0,-1),new HeightData(-1,0,0,1)},
{new HeightData(-1,0,0,0),new HeightData(-1,0,0,1)},
{new HeightData(-1,0,0,1),new HeightData(-1,0,0,1)},
{new HeightData(-1,0,1,-1),new HeightData(0,0,1,0)},
{new HeightData(-1,0,1,0),new HeightData(0,0,1,0)},
{new HeightData(-1,0,1,1),new HeightData(0,0,1,1)},
{new HeightData(-1,1,-1,-1),new HeightData(0,1,-1,0)},
{new HeightData(-1,1,-1,0),new HeightData(0,1,-1,0)},
{new HeightData(-1,1,-1,1),new HeightData(0,1,0,1)},
{new HeightData(-1,1,0,-1),new HeightData(0,1,0,0)},
{new HeightData(-1,1,0,0),new HeightData(0,1,0,0)},
{new HeightData(-1,1,0,1),new HeightData(0,1,0,1)},
{new HeightData(-1,1,1,-1),new HeightData(0,0,1,0)},
{new HeightData(-1,1,1,0),new HeightData(0,0,1,0)},
{new HeightData(-1,1,1,1),new HeightData(0,1,1,1)},
{new HeightData(0,-1,-1,-1),new HeightData(1,0,0,-1)},
{new HeightData(0,-1,-1,0),new HeightData(0,-1,1,0)},
{new HeightData(0,-1,-1,1),new HeightData(0,0,0,1)},
{new HeightData(0,-1,0,-1),new HeightData(1,0,0,-1)},
{new HeightData(0,-1,0,0),new HeightData(0,-1,1,0)},
{new HeightData(0,-1,0,1),new HeightData(0,0,0,1)},
{new HeightData(0,-1,1,-1),new HeightData(0,-1,1,0)},
{new HeightData(0,-1,1,0),new HeightData(0,-1,1,0)},
{new HeightData(0,-1,1,1),new HeightData(0,0,1,1)},
{new HeightData(0,0,-1,-1),new HeightData(1,0,0,-1)},
{new HeightData(0,0,-1,0),new HeightData(0,1,-1,0)},
{new HeightData(0,0,-1,1),new HeightData(0,0,0,1)},
{new HeightData(0,0,0,-1),new HeightData(1,0,0,-1)},
{new HeightData(0,0,0,0),new HeightData(0,0,0,1)},
{new HeightData(0,0,0,1),new HeightData(0,0,0,1)},
{new HeightData(0,0,1,-1),new HeightData(0,0,1,0)},
{new HeightData(0,0,1,0),new HeightData(0,0,1,0)},
{new HeightData(0,0,1,1),new HeightData(0,0,1,1)},
{new HeightData(0,1,-1,-1),new HeightData(0,1,-1,0)},
{new HeightData(0,1,-1,0),new HeightData(0,1,-1,0)},
{new HeightData(0,1,-1,1),new HeightData(0,1,0,1)},
{new HeightData(0,1,0,-1),new HeightData(0,1,0,0)},
{new HeightData(0,1,0,0),new HeightData(0,1,0,0)},
{new HeightData(0,1,0,1),new HeightData(0,1,0,1)},
{new HeightData(0,1,1,-1),new HeightData(0,0,1,0)},
{new HeightData(0,1,1,0),new HeightData(0,0,1,0)},
{new HeightData(0,1,1,1),new HeightData(0,1,1,1)},
{new HeightData(1,-1,-1,-1),new HeightData(1,0,0,-1)},
{new HeightData(1,-1,-1,0),new HeightData(1,0,0,0)},
{new HeightData(1,-1,-1,1),new HeightData(0,0,0,1)},
{new HeightData(1,-1,0,-1),new HeightData(1,0,0,-1)},
{new HeightData(1,-1,0,0),new HeightData(1,0,0,0)},
{new HeightData(1,-1,0,1),new HeightData(0,0,0,1)},
{new HeightData(1,-1,1,-1),new HeightData(1,0,1,0)},
{new HeightData(1,-1,1,0),new HeightData(1,0,1,0)},
{new HeightData(1,-1,1,1),new HeightData(1,0,1,1)},
{new HeightData(1,0,-1,-1),new HeightData(1,0,0,-1)},
{new HeightData(1,0,-1,0),new HeightData(1,0,0,0)},
{new HeightData(1,0,-1,1),new HeightData(0,0,0,1)},
{new HeightData(1,0,0,-1),new HeightData(1,0,0,-1)},
{new HeightData(1,0,0,0),new HeightData(1,0,0,0)},
{new HeightData(1,0,0,1),new HeightData(0,0,0,1)},
{new HeightData(1,0,1,-1),new HeightData(1,0,1,0)},
{new HeightData(1,0,1,0),new HeightData(1,0,1,0)},
{new HeightData(1,0,1,1),new HeightData(1,0,1,1)},
{new HeightData(1,1,-1,-1),new HeightData(1,1,0,0)},
{new HeightData(1,1,-1,0),new HeightData(1,1,0,0)},
{new HeightData(1,1,-1,1),new HeightData(1,1,0,1)},
{new HeightData(1,1,0,-1),new HeightData(1,1,0,0)},
{new HeightData(1,1,0,0),new HeightData(1,1,0,0)},
{new HeightData(1,1,0,1),new HeightData(1,1,0,1)},
{new HeightData(1,1,1,-1),new HeightData(1,1,1,0)},
{new HeightData(1,1,1,0),new HeightData(1,1,1,0)},
{new HeightData(1,1,1,1),new HeightData(1,1,1,1)},
};



    //private readonly static HeightData[] PossibleTiles = new HeightData[]
    //{
    //    //plane
    //    new HeightData(1,1,1,1),

    //    //slopeX+
    //    new HeightData(0,1,0,1),
    //    //slopeX-
    //    new HeightData(1,0,1,0),

    //    //slopeY+
    //    new HeightData(0,0,1,1),
    //    //slopeY-
    //    new HeightData(1,1,0,0),

    //    //slopeX+Y+
    //    new HeightData(-1,0,0,1),
    //    //slopeX-Y-
    //    new HeightData(1,0,0,-1),
    //    //slopeX+Y-
    //    new HeightData(0,-1,1,0),
    //    //slopeX-Y+
    //    new HeightData(0,1,-1,0),

    //    //partialUpSlopeX+Y+
    //    new HeightData(0,0,0,1),
    //    //partialUpSlopeX-Y-
    //    new HeightData(1,0,0,0),
    //    //partialUpSlopeX+Y-
    //    new HeightData(0,0,1,0),
    //    //partialUpSlopeX-Y+
    //    new HeightData(0,1,0,0),

    //    //partialDownSlopeX+Y+
    //    new HeightData(0,1,1,1),
    //    //partialDownSlopeX-Y-
    //    new HeightData(1,1,1,0),
    //    //partialDownSlopeX+Y-
    //    new HeightData(1,0,1,1),
    //    //partialDownSlopeX-Y+
    //    new HeightData(1,1,0,1),
    //};
}
