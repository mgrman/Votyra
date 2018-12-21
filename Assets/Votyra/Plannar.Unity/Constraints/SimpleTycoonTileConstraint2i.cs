using System;
using System.Collections.Generic;
using Votyra.Core;
using Votyra.Core.Images;
using Votyra.Core.Images.Constraints;
using Votyra.Core.ImageSamplers;
using Votyra.Core.Models;

namespace Votyra.Plannar.Images.Constraints
{
    public class SimpleTycoonTileConstraint2i : TycoonTileConstraint2i
    {
        private static readonly IComparer<Height> DefaultComparer = Comparer<Height>.Default;


        public SimpleTycoonTileConstraint2i(IImageSampler2i sampler, [ConfigInject("scaleFactor")] int scaleFactor)
            : base(sampler, scaleFactor)
        {
        }

        public Range2i Constrain(Direction direction, Range2i invalidatedCellArea, IImageSampler2i sampler, Matrix2<Height> editableMatrix)
        {
            IComparer<Height> comparer;
            Func<Vector2i, Height> getValue;
            Func<SampledData2h, SampledData2h> process;
            switch (direction)
            {
                case Direction.Up:
                    direction = Direction.Up;
                    comparer = Comparer<Height>.Create((a, b) => -(DefaultComparer.Compare(a, b)));
                    getValue = cell => sampler.Sample(editableMatrix, cell).Max;
                    process = ProcessUp;
                    break;

                case Direction.Down:
                default:
                    direction = Direction.Down;
                    comparer = DefaultComparer;
                    getValue = cell => sampler.Sample(editableMatrix, cell).Min;
                    process = ProcessDown;
                    break;
            }
            invalidatedCellArea.ForeachPointExlusive(seedCell =>
            {
                var queue = new PrioritySetQueue<Vector2i, Height>(EqualityComparer<Vector2i>.Default, comparer);
                queue.Add(seedCell, getValue(seedCell));

                while (queue.Count > 0)
                {
#if UNITY_EDITOR
                    if (!UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
                    {
                        return;
                    }
#endif

                    var cell = queue.GetFirst().Value;
                    invalidatedCellArea = invalidatedCellArea.CombineWith(cell);

                    var sample = sampler.Sample(editableMatrix, cell);
                    var processedSample = process(sample);

                    Vector2i cell_x0y0 = sampler.CellToX0Y0(cell);
                    Vector2i cell_x0y1 = sampler.CellToX0Y1(cell);
                    Vector2i cell_x1y0 = sampler.CellToX1Y0(cell);
                    Vector2i cell_x1y1 = sampler.CellToX1Y1(cell);

                    Height.Difference change = Height.Difference.Zero;
                    if (editableMatrix.ContainsIndex(cell_x0y0))
                    {
                        change += (editableMatrix[cell_x0y0] - processedSample.x0y0).Abs;
                        editableMatrix[cell_x0y0] = processedSample.x0y0;
                    }
                    if (editableMatrix.ContainsIndex(cell_x0y1))
                    {
                        change += (editableMatrix[cell_x0y1] - processedSample.x0y1).Abs;
                        editableMatrix[cell_x0y1] = processedSample.x0y1;
                    }
                    if (editableMatrix.ContainsIndex(cell_x1y0))
                    {
                        change += (editableMatrix[cell_x1y0] - processedSample.x1y0).Abs;
                        editableMatrix[cell_x1y0] = processedSample.x1y0;
                    }
                    if (editableMatrix.ContainsIndex(cell_x1y1))
                    {
                        change += (editableMatrix[cell_x1y1] - processedSample.x1y1).Abs;
                        editableMatrix[cell_x1y1] = processedSample.x1y1;
                    }
                    if (change > Height.Difference.Zero)
                    {
                        const int areaSize = 1;
                        for (int offsetX = -areaSize; offsetX <= areaSize; offsetX++)
                        {
                            for (int offsetY = -areaSize; offsetY <= areaSize; offsetY++)
                            {
                                if (offsetX == 0 && offsetY == 0)
                                    continue;
                                int ix = cell.X + offsetX;
                                int iy = cell.Y + offsetY;
                                Vector2i newCellToCheck = new Vector2i(ix, iy);
                                var newCellToCheckValue = getValue(cell);
                                if (editableMatrix.ContainsIndex(newCellToCheck))
                                {
                                    queue.Add(newCellToCheck, newCellToCheckValue);
                                }
                            }
                        }
                    }
                }
            });

            return invalidatedCellArea;
        }

        private SampledData2h ProcessDown(SampledData2h sampleData) => -Process(-sampleData);

        private SampledData2h ProcessUp(SampledData2h sampleData) => Process(sampleData);
    }
}