using Votyra.Core;
using Votyra.Core.Images;
using Votyra.Core.Images.Constraints;
using Votyra.Core.ImageSamplers;
using Votyra.Core.Models;

namespace Votyra.Plannar.Images.Constraints
{
    public class DualSampledTycoonTileConstraint2i : IImageConstraint2i
    {
        private static TileMap2i _tileMap;
        private static int? _tileMapScaleFactor;
        private readonly int _scaleFactor;
        private IImageSampler2i _sampler;

        public DualSampledTycoonTileConstraint2i(IImageSampler2i sampler, [ConfigInject("scaleFactor")] int scaleFactor)
        {
            _sampler = sampler;
            _scaleFactor = scaleFactor;
            if (_scaleFactor != _tileMapScaleFactor)
            {
                _tileMap = new[]
                    {
                        //plane
                        new SampledData2h(0, 0, 0, 0),

                        //slope
                        new SampledData2h(-1, 0, -1, 0),

                        //slopeDiagonal
                        new SampledData2h(-2, -1, -1, 0),

                        //partialUpSlope
                        new SampledData2h(-1, -1, -1, 0),

                        //partialDownSlope
                        new SampledData2h(-1, 0, 0, 0),

                        //slopeDiagonal
                        new SampledData2h(0, -1, -1, 0)
                    }
                    .CreateExpandedTileMap2i(scaleFactor);
                _tileMapScaleFactor = scaleFactor;
            }
        }

        public Range2i Constrain(Direction direction, Range2i invalidatedCellArea, IImageSampler2i sampler, Matrix2<Height> editableMatrix)
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

        public Range2i FixImage(Matrix2<Height> editableMatrix, Range2i invalidatedImageArea, Direction direction)
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

        private SampledData2h Process(SampledData2h sampleData)
        {
            var height = sampleData.Max - Height.Default;
            SampledData2h normalizedHeightData = (sampleData - height).ClipMin(-2.CreateHeight() * _scaleFactor);
            SampledData2h choosenTemplateTile = _tileMap.GetTile(normalizedHeightData);
            return choosenTemplateTile + height;
        }
    }
}