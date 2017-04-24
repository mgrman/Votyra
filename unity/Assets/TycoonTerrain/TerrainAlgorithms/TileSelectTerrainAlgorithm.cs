using System;
using System.Collections.Generic;
using System.Linq;
using TycoonTerrain.Common.Models;

namespace TycoonTerrain.TerrainAlgorithms
{
    public class TileSelectTerrainAlgorithm : ITerrainAlgorithm
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

            return new ResultHeightData(choosenTemplateTile.data + height, choosenTemplateTile.flip);
        }
        
        private readonly static HeightData[] Templates = new HeightData[]
        {
            //plane
            new HeightData(1,1,1,1),

            //slope
            new HeightData(0,1,0,1),

            //slopeDiagonal
            new HeightData(-1,0,0,1),

            //partialUpSlope
            new HeightData(0,0,0,1),

            //partialDownSlope
            new HeightData(0,1,1,1)
        };
        
        private readonly static ResultHeightData[] ExpandedTemplates = Templates
            .SelectMany(template =>
            {
                return new[]
                {
                    new ResultHeightData(
                        template,
                        false
                    ),new ResultHeightData(
                        template.GetRotated(1),
                        true
                    ),new ResultHeightData(
                        template.GetRotated(2),
                        false
                    ),new ResultHeightData(
                        template.GetRotated(3),
                        true
                    )
                };
            })
            .Distinct()
            .ToArray();


        private readonly static Dictionary<HeightData, ResultHeightData> TileMap = HeightData.GenerateAllValues(new Range2i(-1, 1))
            .ToDictionary(inputValue => inputValue, inputValue =>
            {
                ResultHeightData choosenTemplateTile = default(ResultHeightData);
                float choosenTemplateTileDiff = float.MaxValue;
                for (int it = 0; it < ExpandedTemplates.Length; it++)
                {
                    ResultHeightData tile = ExpandedTemplates[it];
                    var value = HeightData.Dif(tile.data, inputValue);
                    if (value < choosenTemplateTileDiff)
                    {
                        choosenTemplateTile = tile;
                        choosenTemplateTileDiff = value;
                    }
                }
                return choosenTemplateTile;
            });

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

        //private readonly static Dictionary<HeightData, ResultHeightData> TileMap = new Dictionary<HeightData, ResultHeightData>()
        //{
        //    {new HeightData(-1,-1,-1,-1),new ResultHeightData(-1,0,0,1,false)},
        //    {new HeightData(-1,-1,-1,0),new ResultHeightData(-1,0,0,1,false)},
        //    {new HeightData(-1,-1,-1,1),new ResultHeightData(-1,0,0,1,false)},
        //    {new HeightData(-1,-1,0,-1),new ResultHeightData(-1,0,0,1,false)},
        //    {new HeightData(-1,-1,0,0),new ResultHeightData(-1,0,0,1,false)},
        //    {new HeightData(-1,-1,0,1),new ResultHeightData(-1,0,0,1,false)},
        //    {new HeightData(-1,-1,1,-1),new ResultHeightData(0,-1,1,0,true)},
        //    {new HeightData(-1,-1,1,0),new ResultHeightData(0,-1,1,0,true)},
        //    {new HeightData(-1,-1,1,1),new ResultHeightData(0,0,1,1,false)},
        //    {new HeightData(-1,0,-1,-1),new ResultHeightData(-1,0,0,1,false)},
        //    {new HeightData(-1,0,-1,0),new ResultHeightData(-1,0,0,1,false)},
        //    {new HeightData(-1,0,-1,1),new ResultHeightData(-1,0,0,1,false)},
        //    {new HeightData(-1,0,0,-1),new ResultHeightData(-1,0,0,1,false)},
        //    {new HeightData(-1,0,0,0),new ResultHeightData(-1,0,0,1,false)},
        //    {new HeightData(-1,0,0,1),new ResultHeightData(-1,0,0,1,false)},
        //    {new HeightData(-1,0,1,-1),new ResultHeightData(0,0,1,0,true)},
        //    {new HeightData(-1,0,1,0),new ResultHeightData(0,0,1,0,true)},
        //    {new HeightData(-1,0,1,1),new ResultHeightData(0,0,1,1,false)},
        //    {new HeightData(-1,1,-1,-1),new ResultHeightData(0,1,-1,0,true)},
        //    {new HeightData(-1,1,-1,0),new ResultHeightData(0,1,-1,0,true)},
        //    {new HeightData(-1,1,-1,1),new ResultHeightData(0,1,0,1,false)},
        //    {new HeightData(-1,1,0,-1),new ResultHeightData(0,1,0,0,true)},
        //    {new HeightData(-1,1,0,0),new ResultHeightData(0,1,0,0,true)},
        //    {new HeightData(-1,1,0,1),new ResultHeightData(0,1,0,1,false)},
        //    {new HeightData(-1,1,1,-1),new ResultHeightData(0,0,1,0,true)},
        //    {new HeightData(-1,1,1,0),new ResultHeightData(0,0,1,0,true)},
        //    {new HeightData(-1,1,1,1),new ResultHeightData(0,1,1,1,false)},
        //    {new HeightData(0,-1,-1,-1),new ResultHeightData(1,0,0,-1,false)},
        //    {new HeightData(0,-1,-1,0),new ResultHeightData(0,-1,1,0,true)},
        //    {new HeightData(0,-1,-1,1),new ResultHeightData(0,0,0,1,false)},
        //    {new HeightData(0,-1,0,-1),new ResultHeightData(1,0,0,-1,false)},
        //    {new HeightData(0,-1,0,0),new ResultHeightData(0,-1,1,0,true)},
        //    {new HeightData(0,-1,0,1),new ResultHeightData(0,0,0,1,false)},
        //    {new HeightData(0,-1,1,-1),new ResultHeightData(0,-1,1,0,true)},
        //    {new HeightData(0,-1,1,0),new ResultHeightData(0,-1,1,0,true)},
        //    {new HeightData(0,-1,1,1),new ResultHeightData(0,0,1,1,false)},
        //    {new HeightData(0,0,-1,-1),new ResultHeightData(1,0,0,-1,false)},
        //    {new HeightData(0,0,-1,0),new ResultHeightData(0,1,-1,0,true)},
        //    {new HeightData(0,0,-1,1),new ResultHeightData(0,0,0,1,false)},
        //    {new HeightData(0,0,0,-1),new ResultHeightData(1,0,0,-1,false)},
        //    {new HeightData(0,0,0,0),new ResultHeightData(0,0,0,1,false)},
        //    {new HeightData(0,0,0,1),new ResultHeightData(0,0,0,1,false)},
        //    {new HeightData(0,0,1,-1),new ResultHeightData(0,0,1,0,true)},
        //    {new HeightData(0,0,1,0),new ResultHeightData(0,0,1,0,true)},
        //    {new HeightData(0,0,1,1),new ResultHeightData(0,0,1,1,false)},
        //    {new HeightData(0,1,-1,-1),new ResultHeightData(0,1,-1,0,true)},
        //    {new HeightData(0,1,-1,0),new ResultHeightData(0,1,-1,0,true)},
        //    {new HeightData(0,1,-1,1),new ResultHeightData(0,1,0,1,false)},
        //    {new HeightData(0,1,0,-1),new ResultHeightData(0,1,0,0,true)},
        //    {new HeightData(0,1,0,0),new ResultHeightData(0,1,0,0,true)},
        //    {new HeightData(0,1,0,1),new ResultHeightData(0,1,0,1,false)},
        //    {new HeightData(0,1,1,-1),new ResultHeightData(0,0,1,0,true)},
        //    {new HeightData(0,1,1,0),new ResultHeightData(0,0,1,0,true)},
        //    {new HeightData(0,1,1,1),new ResultHeightData(0,1,1,1,false)},
        //    {new HeightData(1,-1,-1,-1),new ResultHeightData(1,0,0,-1,false)},
        //    {new HeightData(1,-1,-1,0),new ResultHeightData(1,0,0,0,false)},
        //    {new HeightData(1,-1,-1,1),new ResultHeightData(0,0,0,1,false)},
        //    {new HeightData(1,-1,0,-1),new ResultHeightData(1,0,0,-1,false)},
        //    {new HeightData(1,-1,0,0),new ResultHeightData(1,0,0,0,false)},
        //    {new HeightData(1,-1,0,1),new ResultHeightData(0,0,0,1,false)},
        //    {new HeightData(1,-1,1,-1),new ResultHeightData(1,0,1,0,false)},
        //    {new HeightData(1,-1,1,0),new ResultHeightData(1,0,1,0,false)},
        //    {new HeightData(1,-1,1,1),new ResultHeightData(1,0,1,1,true)},
        //    {new HeightData(1,0,-1,-1),new ResultHeightData(1,0,0,-1,false)},
        //    {new HeightData(1,0,-1,0),new ResultHeightData(1,0,0,0,false)},
        //    {new HeightData(1,0,-1,1),new ResultHeightData(0,0,0,1,false)},
        //    {new HeightData(1,0,0,-1),new ResultHeightData(1,0,0,-1,false)},
        //    {new HeightData(1,0,0,0),new ResultHeightData(1,0,0,0,false)},
        //    {new HeightData(1,0,0,1),new ResultHeightData(0,0,0,1,false)},
        //    {new HeightData(1,0,1,-1),new ResultHeightData(1,0,1,0,false)},
        //    {new HeightData(1,0,1,0),new ResultHeightData(1,0,1,0,false)},
        //    {new HeightData(1,0,1,1),new ResultHeightData(1,0,1,1,true)},
        //    {new HeightData(1,1,-1,-1),new ResultHeightData(1,1,0,0,false)},
        //    {new HeightData(1,1,-1,0),new ResultHeightData(1,1,0,0,false)},
        //    {new HeightData(1,1,-1,1),new ResultHeightData(1,1,0,1,true)},
        //    {new HeightData(1,1,0,-1),new ResultHeightData(1,1,0,0,false)},
        //    {new HeightData(1,1,0,0),new ResultHeightData(1,1,0,0,false)},
        //    {new HeightData(1,1,0,1),new ResultHeightData(1,1,0,1,true)},
        //    {new HeightData(1,1,1,-1),new ResultHeightData(1,1,1,0,false)},
        //    {new HeightData(1,1,1,0),new ResultHeightData(1,1,1,0,false)},
        //    {new HeightData(1,1,1,1),new ResultHeightData(1,1,1,1,false)},
        //};
    }
}