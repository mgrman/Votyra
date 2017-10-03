using System;
using System.Collections.Generic;
using System.Linq;
using Votyra.Models;

namespace Votyra.Core.Images.EditableImages.Constraints
{
    public class TycoonTileConstraint2i : IImageConstraint2i
    {
        public SampledData2i Process(SampledData2i sampleData)
        {
            int height = sampleData.Max - 1;

            SampledData2i normalizedHeightData = new SampledData2i(Math.Max(sampleData.x0y0 - height, -1),
                Math.Max(sampleData.x0y1 - height, -1),
                Math.Max(sampleData.x1y0 - height, -1),
                Math.Max(sampleData.x1y1 - height, -1));

            SampledData2i choosenTemplateTile = TileMap[normalizedHeightData];

            return choosenTemplateTile + height;
        }

        private readonly static SampledData2i[] Templates = new SampledData2i[]
        {
            //plane
            new SampledData2i(1,1,1,1),

            //slope
            new SampledData2i(0,1,0,1),

            //slopeDiagonal
            new SampledData2i(-1,0,0,1),

            //partialUpSlope
            new SampledData2i(0,0,0,1),

            //partialDownSlope
            new SampledData2i(0,1,1,1),

            //slopeDiagonal
            new SampledData2i(1,0,0,1),
        };

        private readonly static SampledData2i[] ExpandedTemplates = Templates
            .SelectMany(template =>
            {
                return new[]
                {
                    template,
                    template.GetRotated(1),
                    template.GetRotated(2),
                    template.GetRotated(3),
                };
            })
            .Distinct()
            .ToArray();

        private readonly static Dictionary<SampledData2i, SampledData2i> TileMap = SampledData2i.GenerateAllValues(new Range2i(-1, 1))
            .ToDictionary(inputValue => inputValue, inputValue =>
            {
                SampledData2i choosenTemplateTile = default(SampledData2i);
                float choosenTemplateTileDiff = float.MaxValue;
                for (int it = 0; it < ExpandedTemplates.Length; it++)
                {
                    SampledData2i tile = ExpandedTemplates[it];
                    var value = SampledData2i.Dif(tile, inputValue);
                    if (value < choosenTemplateTileDiff)
                    {
                        choosenTemplateTile = tile;
                        choosenTemplateTileDiff = value;
                    }
                }
                return choosenTemplateTile;
            });
    }
}
