using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Votyra.Core.Images;
using Votyra.Core.Models;
using Votyra.Plannar.ImageSamplers;

namespace Votyra.Plannar.Images.Constraints
{
    public class SimpleTycoonTileConstraint2i : IImageConstraint2i
    {
        public Rect2i Constrain(Direction direction, Rect2i invalidArea, IImageSampler2i sampler, IImage2f image, Matrix<float> editableMatrix)
        {
            IComparer<float> comparer;
            Func<Vector2i, float> getValue;
            switch (direction)
            {
                case Direction.Up:
                    direction = Direction.Up;
                    comparer = Comparer<float>.Create((a, b) => -a.CompareTo(b));
                    getValue = cell => sampler.Sample(image, cell).Max;
                    break;
                case Direction.Down:
                default:
                    direction = Direction.Down;
                    comparer = Comparer<float>.Create((a, b) => a.CompareTo(b));
                    getValue = cell => sampler.Sample(image, cell).Min;
                    break;
            }

            var pointsToExamine = Enumerable.Range(0, invalidArea.max.x - invalidArea.min.x + 1)
                .SelectMany(x => Enumerable.Range(0, invalidArea.max.y - invalidArea.min.y + 1)
                    .Select(y => new Vector2i(invalidArea.min.x + x, invalidArea.min.y + y)));

            foreach (var pointToExamine in pointsToExamine)
            {
                var queue = new PrioritySetQueue<Vector2i, float>
                    (
                        new [] { pointToExamine },
                        EqualityComparer<Vector2i>.Default,
                        getValue,
                        comparer
                    );

                while (queue.Count > 0)
                {
                    var cell = queue.GetFirst().Value;
                    invalidArea = invalidArea.CombineWith(cell);

                    var sample = sampler.Sample(image, cell);
                    if (direction == Direction.Down)
                    {
                        sample = new SampledData2i(sample, o => o * -1);
                    }

                    var processedSample = Process(sample);
                    if (direction == Direction.Down)
                    {
                        processedSample = new SampledData2i(processedSample, o => o * -1);
                    }

                    Vector2i cell_x0y0 = sampler.CellToX0Y0(cell);
                    Vector2i cell_x0y1 = sampler.CellToX0Y1(cell);
                    Vector2i cell_x1y0 = sampler.CellToX1Y0(cell);
                    Vector2i cell_x1y1 = sampler.CellToX1Y1(cell);

                    float change = 0;
                    if (cell_x0y0.IsAsIndexContained(editableMatrix.size))
                    {
                        change += Math.Abs(editableMatrix[cell_x0y0] - processedSample.x0y0);
                        editableMatrix[cell_x0y0] = processedSample.x0y0;
                    }
                    if (cell_x0y1.IsAsIndexContained(editableMatrix.size))
                    {
                        change += Math.Abs(editableMatrix[cell_x0y0] - processedSample.x0y0);
                        editableMatrix[cell_x0y1] = processedSample.x0y1;
                    }
                    if (cell_x1y0.IsAsIndexContained(editableMatrix.size))
                    {
                        change += Math.Abs(editableMatrix[cell_x0y0] - processedSample.x0y0);
                        editableMatrix[cell_x1y0] = processedSample.x1y0;
                    }
                    if (cell_x1y1.IsAsIndexContained(editableMatrix.size))
                    {
                        change += Math.Abs(editableMatrix[cell_x0y0] - processedSample.x0y0);
                        editableMatrix[cell_x1y1] = processedSample.x1y1;
                    }
                    if (change > 0.01)
                    {
                        const int areaSize = 1;
                        for (int offsetX = -areaSize; offsetX <= areaSize; offsetX++)
                        {
                            for (int offsetY = -areaSize; offsetY <= areaSize; offsetY++)
                            {
                                if (offsetX == 0 && offsetY == 0)
                                    continue;
                                int ix = cell.x + offsetX;
                                int iy = cell.y + offsetY;
                                Vector2i newCellToCheck = new Vector2i(ix, iy);
                                var newCellToCheckValue = getValue(cell);
                                if (newCellToCheck.IsAsIndexContained(editableMatrix.size))
                                {
                                    queue.Add(newCellToCheck, newCellToCheckValue);
                                }
                            }
                        }
                    }
                }

            }
            return invalidArea;
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