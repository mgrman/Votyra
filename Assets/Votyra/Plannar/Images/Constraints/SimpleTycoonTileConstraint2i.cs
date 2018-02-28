using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Votyra.Core.Images;
using Votyra.Core.Models;
using Votyra.Core.Utils;
using Votyra.Plannar.ImageSamplers;

namespace Votyra.Plannar.Images.Constraints
{
    public class SimpleTycoonTileConstraint2i : IImageConstraint2i
    {
        private IImageSampler2i _sampler;

        public SimpleTycoonTileConstraint2i(IImageSampler2i sampler)
        {
            _sampler = sampler;
        }

        public Rect2i FixImage(Matrix<float> editableMatrix, Rect2i invalidatedImageArea, Direction direction)
        {
            if (_sampler == null)
            {
                return invalidatedImageArea;
            }

            var invalidatedCellArea = _sampler.ImageToWorld(invalidatedImageArea)
                .RoundToContain();

            var newInvalidatedCellArea = Constrain(direction, invalidatedCellArea, _sampler, editableMatrix);

            var newInvalidatedImageArea = _sampler.WorldToImage(newInvalidatedCellArea);

            return invalidatedImageArea.CombineWith(newInvalidatedImageArea);
        }


        public Rect2i Constrain(Direction direction, Rect2i invalidatedCellArea, IImageSampler2i sampler, Matrix<float> editableMatrix)
        {
            IComparer<float> comparer;
            Func<Vector2i, float> getValue;
            Func<SampledData2i, SampledData2i> process;
            switch (direction)
            {
                case Direction.Up:
                    direction = Direction.Up;
                    comparer = Comparer<float>.Create((a, b) => -a.CompareTo(b));
                    getValue = cell => sampler.Sample(editableMatrix, cell).Max;
                    process = ProcessUp;
                    break;
                case Direction.Down:
                default:
                    direction = Direction.Down;
                    comparer = Comparer<float>.Create((a, b) => a.CompareTo(b));
                    getValue = cell => sampler.Sample(editableMatrix, cell).Min;
                    process = ProcessDown;
                    break;
            }

            for (int cell_x = invalidatedCellArea.min.x; cell_x <= invalidatedCellArea.max.x; cell_x++)
            {
                for (int cell_y = invalidatedCellArea.min.y; cell_y <= invalidatedCellArea.max.y; cell_y++)
                {
                    var seedCell = new Vector2i(cell_x, cell_y);
                    var queue = new PrioritySetQueue<Vector2i, float>
                        (
                            EqualityComparer<Vector2i>.Default,
                            comparer
                        );
                    queue.Add(seedCell, getValue(seedCell));

                    while (queue.Count > 0)
                    {
                        var cell = queue.GetFirst().Value;
                        invalidatedCellArea = invalidatedCellArea.CombineWith(cell);

                        var sample = sampler.Sample(editableMatrix, cell);
                        var processedSample = process(sample);

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
                            change += Math.Abs(editableMatrix[cell_x0y1] - processedSample.x0y1);
                            editableMatrix[cell_x0y1] = processedSample.x0y1;
                        }
                        if (cell_x1y0.IsAsIndexContained(editableMatrix.size))
                        {
                            change += Math.Abs(editableMatrix[cell_x1y0] - processedSample.x1y0);
                            editableMatrix[cell_x1y0] = processedSample.x1y0;
                        }
                        if (cell_x1y1.IsAsIndexContained(editableMatrix.size))
                        {
                            change += Math.Abs(editableMatrix[cell_x1y1] - processedSample.x1y1);
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
            }
            return invalidatedCellArea;
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

        private SampledData2i ProcessUp(SampledData2i sampleData)
        {
            int height = sampleData.Max;
            SampledData2i normalizedHeightData = (sampleData - height).ClipMin(-2);
            SampledData2i choosenTemplateTile = TileMap[normalizedHeightData];
            return choosenTemplateTile + height;
        }
        private SampledData2i ProcessDown(SampledData2i sampleData)
        {
            return -ProcessUp(-sampleData);
        }

        private readonly static SampledData2i[] Templates = new SampledData2i[]
        {
            //plane
            new SampledData2i(0, 0, 0, 0),

            //slope
            new SampledData2i(-1, 0, -1, 0),

            //slopeDiagonal
            new SampledData2i(-2, -1, -1, 0),

            //partialUpSlope
            new SampledData2i(-1, -1, -1, 0),

            //partialDownSlope
            new SampledData2i(-1, 0, 0, 0),

            //slopeDiagonal
            new SampledData2i(0, -1, -1, 0),
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

        private readonly static Dictionary<SampledData2i, SampledData2i> TileMap = SampledData2i
            .GenerateAllValues(new Range2i(-2, 0))
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