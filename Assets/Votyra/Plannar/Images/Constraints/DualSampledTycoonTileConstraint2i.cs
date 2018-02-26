using System;
using System.Collections.Generic;
using System.Linq;
using Votyra.Core.Images;
using Votyra.Core.Models;
using Votyra.Plannar.ImageSamplers;

namespace Votyra.Plannar.Images.Constraints
{
    public class DualSampledTycoonTileConstraint2i : IImageConstraint2i
    {

        public Rect2i Constrain(Direction direction, Rect2i invalidatedArea, IImageSampler2i sampler, IImage2f image, Matrix<float> editableMatrix)
        {

            if (direction != Direction.Up && direction != Direction.Down)
            {
                direction = Direction.Down;
            }

            for (int ix = invalidatedArea.min.x; ix <= invalidatedArea.max.x; ix++)
            {
                for (int iy = invalidatedArea.min.y; iy <= invalidatedArea.max.y; iy++)
                {
                    var cell = new Vector2i(ix, iy);
                    var sample = sampler.Sample(image, cell);

                    var processedSample = Process(sample);

                    Vector2i cell_x0y0 = sampler.CellToX0Y0(cell);
                    Vector2i cell_x0y1 = sampler.CellToX0Y1(cell);
                    Vector2i cell_x1y0 = sampler.CellToX1Y0(cell);
                    Vector2i cell_x1y1 = sampler.CellToX1Y1(cell);

                    if (cell_x0y0.IsAsIndexContained(editableMatrix.size))
                        editableMatrix[cell_x0y0] = processedSample.x0y0;
                    if (cell_x0y1.IsAsIndexContained(editableMatrix.size))
                        editableMatrix[cell_x0y1] = processedSample.x0y1;
                    if (cell_x1y0.IsAsIndexContained(editableMatrix.size))
                        editableMatrix[cell_x1y0] = processedSample.x1y0;
                    if (cell_x1y1.IsAsIndexContained(editableMatrix.size))
                        editableMatrix[cell_x1y1] = processedSample.x1y1;
                }
            }
            return invalidatedArea;
        }

        private struct PositionWithValue
        {
            public readonly Vector2i Position;
            public readonly float Value;

            public PositionWithValue(Vector2i pos, float value)
            {
                Position = pos;
                Value = value;
            }

        };

        private SampledData2i Process(SampledData2i sampleData)
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
            new SampledData2i(1, 1, 1, 1),

            //slope
            new SampledData2i(0, 1, 0, 1),

            //slopeDiagonal
            new SampledData2i(-1, 0, 0, 1),

            //partialUpSlope
            new SampledData2i(0, 0, 0, 1),

            //partialDownSlope
            new SampledData2i(0, 1, 1, 1),

            //slopeDiagonal
            new SampledData2i(1, 0, 0, 1),
        };

        private readonly static SampledData2i[] ExpandedTemplates = Templates
            .SelectMany(template =>
            {
                return new []
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