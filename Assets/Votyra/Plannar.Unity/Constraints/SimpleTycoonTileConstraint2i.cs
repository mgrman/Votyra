using System;
using System.Collections.Generic;
using System.Linq;
using Votyra.Core.Images;
using Votyra.Core.Images.Constraints;
using Votyra.Core.ImageSamplers;
using Votyra.Core.Models;
using Votyra.Core.Utils;

namespace Votyra.Plannar.Images.Constraints
{
    public class SimpleTycoonTileConstraint2i : IImageConstraint2i
    {
        public const int ScaleFactor = 2;

        private IImageSampler2i _sampler;

        public SimpleTycoonTileConstraint2i(IImageSampler2i sampler)
        {
            _sampler = sampler;
        }

        public Range2i FixImage(Matrix2<int?> editableMatrix, Range2i invalidatedImageArea, Direction direction)
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

        private static readonly IComparer<int?> DefaultComparer = Comparer<int?>.Default;

        public Range2i Constrain(Direction direction, Range2i invalidatedCellArea, IImageSampler2i sampler, Matrix2<int?> editableMatrix)
        {
            IComparer<int?> comparer;
            Func<Vector2i, int?> getValue;
            Func<SampledData2i, SampledData2i> process;
            switch (direction)
            {
                case Direction.Up:
                    direction = Direction.Up;
                    comparer = Comparer<int?>.Create((a, b) => -(DefaultComparer.Compare(a, b)));
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
                var queue = new PrioritySetQueue<Vector2i, int?>(EqualityComparer<Vector2i>.Default, comparer);
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
                    if (sample.GetHoleCount() != 0)
                        continue;
                    var processedSample = process(sample);

                    Vector2i cell_x0y0 = sampler.CellToX0Y0(cell);
                    Vector2i cell_x0y1 = sampler.CellToX0Y1(cell);
                    Vector2i cell_x1y0 = sampler.CellToX1Y0(cell);
                    Vector2i cell_x1y1 = sampler.CellToX1Y1(cell);

                    int change = 0;
                    if (editableMatrix.ContainsIndex(cell_x0y0))
                    {
                        change += MathUtils.Abs(editableMatrix[cell_x0y0] - processedSample.x0y0) ?? 0;
                        editableMatrix[cell_x0y0] = processedSample.x0y0;
                    }
                    if (editableMatrix.ContainsIndex(cell_x0y1))
                    {
                        change += MathUtils.Abs(editableMatrix[cell_x0y1] - processedSample.x0y1) ?? 0;
                        editableMatrix[cell_x0y1] = processedSample.x0y1;
                    }
                    if (editableMatrix.ContainsIndex(cell_x1y0))
                    {
                        change += MathUtils.Abs(editableMatrix[cell_x1y0] - processedSample.x1y0) ?? 0;
                        editableMatrix[cell_x1y0] = processedSample.x1y0;
                    }
                    if (editableMatrix.ContainsIndex(cell_x1y1))
                    {
                        change += MathUtils.Abs(editableMatrix[cell_x1y1] - processedSample.x1y1) ?? 0;
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

        private SampledData2i ProcessUp(SampledData2i sampleData)
        {
            var height = sampleData.Max;
            SampledData2i normalizedHeightData = (sampleData - height).ClipMin(-2 * ScaleFactor);
            SampledData2i choosenTemplateTile = TileMap.GetTile(normalizedHeightData);
            return choosenTemplateTile + height;
            // var normalizer = new SampledData2iNormalizer(sampleData, TileMap.ValueRange);
            // SampledData2i choosenTemplateTile = TileMap.GetTile(normalizer.NormalizedValue);
            // return normalizer.Denormalize(choosenTemplateTile);
        }

        private SampledData2i ProcessDown(SampledData2i sampleData)
        {
            return -ProcessUp(-sampleData);
        }

        private readonly static TileMap2i TileMap = new[]
        {
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
            new SampledData2i(0, -1, -1, 0)
        }
        .CreateExpandedTileMap2i(ScaleFactor);
    }
}