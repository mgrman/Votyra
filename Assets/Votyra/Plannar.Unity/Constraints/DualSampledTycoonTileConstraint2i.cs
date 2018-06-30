using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Votyra.Core.Images;
using Votyra.Core.Images.Constraints;
using Votyra.Core.ImageSamplers;
using Votyra.Core.Models;
using Votyra.Core.Utils;

namespace Votyra.Plannar.Images.Constraints
{
    public class DualSampledTycoonTileConstraint2i : IImageConstraint2i
    {
        private IImageSampler2i _sampler;

        public DualSampledTycoonTileConstraint2i(IImageSampler2i sampler)
        {
            _sampler = sampler;
        }

        public Range2i FixImage(Matrix2<int?> editableMatrix, Range2i invalidatedImageArea, Direction direction)
        {
            if (_sampler == null)
            {
                return invalidatedImageArea;
            }

            var invalidatedCellArea = _sampler.ImageToWorld(invalidatedImageArea).RoundToContain();
            var newInvalidatedCellArea = Constrain(direction, invalidatedCellArea, _sampler, editableMatrix);
            var newInvalidatedImageArea = _sampler.WorldToImage(newInvalidatedCellArea);
            return invalidatedImageArea.CombineWith(newInvalidatedImageArea);
        }

        public Range2i Constrain(Direction direction, Range2i invalidatedCellArea, IImageSampler2i sampler, Matrix2<int?> editableMatrix)
        {
            if (direction != Direction.Up && direction != Direction.Down)
            {
                direction = Direction.Down;
            }
            invalidatedCellArea.ForeachPointExlusive(cell =>
            {
                var sample = sampler.Sample(editableMatrix, cell);

                var processedSample = Process(sample);

                Vector2i cell_x0y0 = sampler.CellToX0Y0(cell);
                Vector2i cell_x0y1 = sampler.CellToX0Y1(cell);
                Vector2i cell_x1y0 = sampler.CellToX1Y0(cell);
                Vector2i cell_x1y1 = sampler.CellToX1Y1(cell);

                if (editableMatrix.ContainsIndex(cell_x0y0))
                    editableMatrix[cell_x0y0] = processedSample.x0y0;
                if (editableMatrix.ContainsIndex(cell_x0y1))
                    editableMatrix[cell_x0y1] = processedSample.x0y1;
                if (editableMatrix.ContainsIndex(cell_x1y0))
                    editableMatrix[cell_x1y0] = processedSample.x1y0;
                if (editableMatrix.ContainsIndex(cell_x1y1))
                    editableMatrix[cell_x1y1] = processedSample.x1y1;
            });

            return invalidatedCellArea;
        }

        private SampledData2i Process(SampledData2i sampleData)
        {
            var height = sampleData.Max - 1;
            SampledData2i normalizedHeightData = sampleData.NormalizeFromTop(Range1i.PlusMinusOne);
            SampledData2i choosenTemplateTile = TileMap.GetTile(normalizedHeightData);
            return choosenTemplateTile + height;
        }

        private readonly static TileMap2i TileMap = new TileMap2i(new[]
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

            //plane with hole
            new SampledData2i(1, 1, 1, null),

            //slope with hole
            new SampledData2i(0, 1, 0, null),

            //slope with hole
            new SampledData2i(0, 1, null, 1),

            //slope with hole
            new SampledData2i(0, null, 0, 1),

            //slope with hole
            new SampledData2i(null, 1, 0, 1),

            //partialUpSlope with hole
            new SampledData2i(null, 0, 0, 1),

            //partialDownSlope with hole
            new SampledData2i(0, 1, 1, null),
        });
    }
}