using System;
using System.Collections.Generic;
using System.Linq;
using Votyra.Core.Images;
using Votyra.Core.Models;
using Votyra.Plannar.ImageSamplers;

namespace Votyra.Plannar.Images.Constraints
{
    public class SimpleTycoonTileConstraint2i : IImageConstraint2i
    {
        public void Constrain(Vector2i cellMin, Vector2i cellMax, IImageSampler2i sampler, IImage2f image, Matrix<float> editableMatrix)
        {
            var queue = new LinkedList<PositionWithValue>(
            Enumerable.Range(0, cellMax.x - cellMin.x + 1)
                .SelectMany(x => Enumerable.Range(0, cellMax.y - cellMin.y + 1)
                    .Select(y => new Vector2i(cellMin.x + x, cellMin.y + y))
                    .Select(cell => new PositionWithValue(cell, sampler.Sample(image, cell).Max)))
                .OrderByDescending(cell => cell.Value));

            while (queue.Count > 0)
            {
                var cellWithValue = queue.First.Value;
                var cell = cellWithValue.Position;
                queue.RemoveFirst();

                var sample = sampler.Sample(image, cell);

                var processedSample = Process(sample);

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
                if (change > 0)
                {
                    for (int offsetX = -1; offsetX <= 1; offsetX++)
                    {
                        for (int offsetY = -1; offsetY <= 1; offsetY++)
                        {
                            if (offsetX == 0 && offsetY == 0)
                                continue;
                            int ix = cell.x + offsetX;
                            int iy = cell.y + offsetY;
                            Vector2i newCellToCheck = new Vector2i(ix, iy);
                            var newCellToCheckValue = sampler.Sample(image, cell).Max;
                            if (newCellToCheck.IsAsIndexContained(editableMatrix.size))
                            {
                                {
                                    var node = queue.First;
                                    while (node != null && node.Value.Position != newCellToCheck)
                                    {
                                        node = node.Next;
                                    }
                                    if (node != null)
                                    {
                                        queue.Remove(node);
                                    }
                                }
                                {
                                    var node = queue.First;
                                    while (node != null && node.Value.Value >= newCellToCheckValue)
                                    {
                                        node = node.Next;
                                    }
                                    if (node == null)
                                    {
                                        queue.AddLast(new PositionWithValue(newCellToCheck, newCellToCheckValue));
                                    }
                                    else
                                    {
                                        queue.AddBefore(node, new PositionWithValue(newCellToCheck, newCellToCheckValue));
                                    }
                                }
                            }
                        }
                    }
                }
            }

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

        private readonly static SampledData2i[] Templates = new SampledData2i[] {
            //plane
            new SampledData2i (1, 1, 1, 1),

            //slope
            new SampledData2i (0, 1, 0, 1),

            //slopeDiagonal
            new SampledData2i (-1, 0, 0, 1),

            //partialUpSlope
            new SampledData2i (0, 0, 0, 1),

            //partialDownSlope
            new SampledData2i (0, 1, 1, 1),

            //slopeDiagonal
            new SampledData2i (1, 0, 0, 1),
        };

        private readonly static SampledData2i[] ExpandedTemplates = Templates
            .SelectMany(template =>
            {
                return new[] {
                template,
                template.GetRotated (1),
                template.GetRotated (2),
                template.GetRotated (3),
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