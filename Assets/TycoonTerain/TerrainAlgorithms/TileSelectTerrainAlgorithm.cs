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

    public ResultHeightData Process(HeightData sampleData)
    {
        int height = sampleData.Max - 1;

        HeightData normalizedHeightData = new HeightData(Math.Max(sampleData.x0y0 - height, -1),
            Math.Max(sampleData.x0y1 - height, -1),
            Math.Max(sampleData.x1y0 - height, -1),
            Math.Max(sampleData.x1y1 - height, -1));

        ResultHeightData choosenTemplateTile = TileMap[normalizedHeightData];
        
        return new ResultHeightData(choosenTemplateTile.data+height,choosenTemplateTile.flip);
    }


    private readonly static Dictionary<HeightData, ResultHeightData> TileMap = new Dictionary<HeightData, ResultHeightData>()
{
{new HeightData(-1,-1,-1,-1),new ResultHeightData(-1,0,0,1,false)},
{new HeightData(-1,-1,-1,0),new ResultHeightData(-1,0,0,1,false)},
{new HeightData(-1,-1,-1,1),new ResultHeightData(-1,0,0,1,false)},
{new HeightData(-1,-1,0,-1),new ResultHeightData(-1,0,0,1,false)},
{new HeightData(-1,-1,0,0),new ResultHeightData(-1,0,0,1,false)},
{new HeightData(-1,-1,0,1),new ResultHeightData(-1,0,0,1,false)},
{new HeightData(-1,-1,1,-1),new ResultHeightData(0,-1,1,0,true)},
{new HeightData(-1,-1,1,0),new ResultHeightData(0,-1,1,0,true)},
{new HeightData(-1,-1,1,1),new ResultHeightData(0,0,1,1,false)},
{new HeightData(-1,0,-1,-1),new ResultHeightData(-1,0,0,1,false)},
{new HeightData(-1,0,-1,0),new ResultHeightData(-1,0,0,1,false)},
{new HeightData(-1,0,-1,1),new ResultHeightData(-1,0,0,1,false)},
{new HeightData(-1,0,0,-1),new ResultHeightData(-1,0,0,1,false)},
{new HeightData(-1,0,0,0),new ResultHeightData(-1,0,0,1,false)},
{new HeightData(-1,0,0,1),new ResultHeightData(-1,0,0,1,false)},
{new HeightData(-1,0,1,-1),new ResultHeightData(0,0,1,0,true)},
{new HeightData(-1,0,1,0),new ResultHeightData(0,0,1,0,true)},
{new HeightData(-1,0,1,1),new ResultHeightData(0,0,1,1,false)},
{new HeightData(-1,1,-1,-1),new ResultHeightData(0,1,-1,0,true)},
{new HeightData(-1,1,-1,0),new ResultHeightData(0,1,-1,0,true)},
{new HeightData(-1,1,-1,1),new ResultHeightData(0,1,0,1,false)},
{new HeightData(-1,1,0,-1),new ResultHeightData(0,1,0,0,true)},
{new HeightData(-1,1,0,0),new ResultHeightData(0,1,0,0,true)},
{new HeightData(-1,1,0,1),new ResultHeightData(0,1,0,1,false)},
{new HeightData(-1,1,1,-1),new ResultHeightData(0,0,1,0,true)},
{new HeightData(-1,1,1,0),new ResultHeightData(0,0,1,0,true)},
{new HeightData(-1,1,1,1),new ResultHeightData(0,1,1,1,false)},
{new HeightData(0,-1,-1,-1),new ResultHeightData(1,0,0,-1,false)},
{new HeightData(0,-1,-1,0),new ResultHeightData(0,-1,1,0,true)},
{new HeightData(0,-1,-1,1),new ResultHeightData(0,0,0,1,false)},
{new HeightData(0,-1,0,-1),new ResultHeightData(1,0,0,-1,false)},
{new HeightData(0,-1,0,0),new ResultHeightData(0,-1,1,0,true)},
{new HeightData(0,-1,0,1),new ResultHeightData(0,0,0,1,false)},
{new HeightData(0,-1,1,-1),new ResultHeightData(0,-1,1,0,true)},
{new HeightData(0,-1,1,0),new ResultHeightData(0,-1,1,0,true)},
{new HeightData(0,-1,1,1),new ResultHeightData(0,0,1,1,false)},
{new HeightData(0,0,-1,-1),new ResultHeightData(1,0,0,-1,false)},
{new HeightData(0,0,-1,0),new ResultHeightData(0,1,-1,0,true)},
{new HeightData(0,0,-1,1),new ResultHeightData(0,0,0,1,false)},
{new HeightData(0,0,0,-1),new ResultHeightData(1,0,0,-1,false)},
{new HeightData(0,0,0,0),new ResultHeightData(0,0,0,1,false)},
{new HeightData(0,0,0,1),new ResultHeightData(0,0,0,1,false)},
{new HeightData(0,0,1,-1),new ResultHeightData(0,0,1,0,true)},
{new HeightData(0,0,1,0),new ResultHeightData(0,0,1,0,true)},
{new HeightData(0,0,1,1),new ResultHeightData(0,0,1,1,false)},
{new HeightData(0,1,-1,-1),new ResultHeightData(0,1,-1,0,true)},
{new HeightData(0,1,-1,0),new ResultHeightData(0,1,-1,0,true)},
{new HeightData(0,1,-1,1),new ResultHeightData(0,1,0,1,false)},
{new HeightData(0,1,0,-1),new ResultHeightData(0,1,0,0,true)},
{new HeightData(0,1,0,0),new ResultHeightData(0,1,0,0,true)},
{new HeightData(0,1,0,1),new ResultHeightData(0,1,0,1,false)},
{new HeightData(0,1,1,-1),new ResultHeightData(0,0,1,0,true)},
{new HeightData(0,1,1,0),new ResultHeightData(0,0,1,0,true)},
{new HeightData(0,1,1,1),new ResultHeightData(0,1,1,1,false)},
{new HeightData(1,-1,-1,-1),new ResultHeightData(1,0,0,-1,false)},
{new HeightData(1,-1,-1,0),new ResultHeightData(1,0,0,0,false)},
{new HeightData(1,-1,-1,1),new ResultHeightData(0,0,0,1,false)},
{new HeightData(1,-1,0,-1),new ResultHeightData(1,0,0,-1,false)},
{new HeightData(1,-1,0,0),new ResultHeightData(1,0,0,0,false)},
{new HeightData(1,-1,0,1),new ResultHeightData(0,0,0,1,false)},
{new HeightData(1,-1,1,-1),new ResultHeightData(1,0,1,0,false)},
{new HeightData(1,-1,1,0),new ResultHeightData(1,0,1,0,false)},
{new HeightData(1,-1,1,1),new ResultHeightData(1,0,1,1,true)},
{new HeightData(1,0,-1,-1),new ResultHeightData(1,0,0,-1,false)},
{new HeightData(1,0,-1,0),new ResultHeightData(1,0,0,0,false)},
{new HeightData(1,0,-1,1),new ResultHeightData(0,0,0,1,false)},
{new HeightData(1,0,0,-1),new ResultHeightData(1,0,0,-1,false)},
{new HeightData(1,0,0,0),new ResultHeightData(1,0,0,0,false)},
{new HeightData(1,0,0,1),new ResultHeightData(0,0,0,1,false)},
{new HeightData(1,0,1,-1),new ResultHeightData(1,0,1,0,false)},
{new HeightData(1,0,1,0),new ResultHeightData(1,0,1,0,false)},
{new HeightData(1,0,1,1),new ResultHeightData(1,0,1,1,true)},
{new HeightData(1,1,-1,-1),new ResultHeightData(1,1,0,0,false)},
{new HeightData(1,1,-1,0),new ResultHeightData(1,1,0,0,false)},
{new HeightData(1,1,-1,1),new ResultHeightData(1,1,0,1,true)},
{new HeightData(1,1,0,-1),new ResultHeightData(1,1,0,0,false)},
{new HeightData(1,1,0,0),new ResultHeightData(1,1,0,0,false)},
{new HeightData(1,1,0,1),new ResultHeightData(1,1,0,1,true)},
{new HeightData(1,1,1,-1),new ResultHeightData(1,1,1,0,false)},
{new HeightData(1,1,1,0),new ResultHeightData(1,1,1,0,false)},
{new HeightData(1,1,1,1),new ResultHeightData(1,1,1,1,false)},
}

;



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
