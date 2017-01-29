using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Profiling;

public class TileSelectMeshGenerator : MonoBehaviour, IMeshGenerator
{
    [Range(0.0F, 5.0f)]
    public float HeightDifferenceAbs = 1;
    
    public bool RequiresWalls { get { return true; } }

    public HeightData Process(HeightData sampleData)
    {
        //Profiler.BeginSample("Rounding samples");
        HeightData heightData = sampleData.Round(HeightDifferenceAbs);
        //Profiler.EndSample();

        HeightData resultingHeightData;
        if (HeightDifferenceAbs > 0)
        {
            //Profiler.BeginSample("Normalizing heightData");
            HeightData normalizedHeightData = heightData.Normalize(heightData.Max-HeightDifferenceAbs, heightData.Max);
            //Profiler.EndSample();

            //choose tile
            //Profiler.BeginSample("Choosing tile");
            HeightData choosenTemplateTile = default(HeightData);
            float choosenTemplateTileDiff = float.MaxValue;
            for(int it=0;it< PossibleTiles.Length; it++)
            {
                HeightData tile = PossibleTiles[it];
                var value = HeightData.Dif(tile, normalizedHeightData);
                if (value < choosenTemplateTileDiff)
                {
                    choosenTemplateTile = tile;
                    choosenTemplateTileDiff = value;
                }
            }
            //Profiler.EndSample();

            //Profiler.BeginSample("Converting tile to result");
            resultingHeightData = choosenTemplateTile.Denormalize(heightData.Max - HeightDifferenceAbs, heightData.Max);
            //Profiler.EndSample();
        }
        else
        {
            resultingHeightData = heightData;
        }
        return resultingHeightData;
    }

    private readonly static HeightData[] PossibleTiles = new HeightData[]
    {
        //plane
        new HeightData(1,1,1,1),

        //slopeX+
        new HeightData(0,1,0,1),
        //slopeX-
        new HeightData(1,0,1,0),
        
        //slopeY+
        new HeightData(0,0,1,1),
        //slopeY-
        new HeightData(1,1,0,0),
        
        //slopeX+Y+
        new HeightData(-1,0,0,1),
        //slopeX-Y-
        new HeightData(1,0,0,-1),
        //slopeX+Y-
        new HeightData(0,-1,1,0),
        //slopeX-Y+
        new HeightData(0,1,-1,0),
        
        //partialUpSlopeX+Y+
        new HeightData(0,0,0,1),
        //partialUpSlopeX-Y-
        new HeightData(1,0,0,0),
        //partialUpSlopeX+Y-
        new HeightData(0,0,1,0),
        //partialUpSlopeX-Y+
        new HeightData(0,1,0,0),

        //partialDownSlopeX+Y+
        new HeightData(0,1,1,1),
        //partialDownSlopeX-Y-
        new HeightData(1,1,1,0),
        //partialDownSlopeX+Y-
        new HeightData(1,0,1,1),
        //partialDownSlopeX-Y+
        new HeightData(1,1,0,1),
    };
}
