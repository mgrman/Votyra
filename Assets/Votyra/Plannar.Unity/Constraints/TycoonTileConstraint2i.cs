using Votyra.Core;
using Votyra.Core.Images;
using Votyra.Core.Images.Constraints;
using Votyra.Core.ImageSamplers;
using Votyra.Core.Models;

namespace Votyra.Plannar.Images.Constraints
{
    public class TycoonTileConstraint2i : IImageConstraint2i
    {
        private static TileMap2i _tileMap;
        private static int? _tileMapScaleFactor;
        private readonly int _scaleFactor;
        protected Range2i _invalidatedCellArea;
        protected Direction _direction;
        protected Matrix2<Height> _editableMatrix;
        public TycoonTileConstraint2i([ConfigInject("scaleFactor")] int scaleFactor)
        {
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

        public Range2i FixImage(Matrix2<Height> editableMatrix, Range2i invalidatedImageArea, Direction direction)
        {
            _invalidatedCellArea = invalidatedImageArea;
            if (direction != Direction.Up && direction != Direction.Down)
            {
                direction = Direction.Down;
            }
            _direction = direction;
            _editableMatrix = editableMatrix;
            Constrain();
            return _invalidatedCellArea;
        }

        public virtual void Constrain()
        {
            _invalidatedCellArea.ForeachPointExlusive(cell =>
            {
                ConstrainCell(cell);
            });
        }

        protected virtual void ConstrainCell(Vector2i cell)
        {
            Vector2i cell_x0y0 = ImageSampler2iUtils.CellToX0Y0(cell);
            Vector2i cell_x1y1 = ImageSampler2iUtils.CellToX1Y1(cell);
            if (_editableMatrix.ContainsIndex(cell_x0y0) && _editableMatrix.ContainsIndex(cell_x1y1))
            {
                var sample = _editableMatrix.SampleCell(cell);
                var processedSample = Process(sample);

                Vector2i cell_x0y1 = ImageSampler2iUtils.CellToX0Y1(cell);
                Vector2i cell_x1y0 = ImageSampler2iUtils.CellToX1Y0(cell);

                _editableMatrix[cell_x0y0] = processedSample.x0y0;
                _editableMatrix[cell_x0y1] = processedSample.x0y1;
                _editableMatrix[cell_x1y0] = processedSample.x1y0;
                _editableMatrix[cell_x1y1] = processedSample.x1y1;
            }
        }

        protected SampledData2h Process(SampledData2h sampleData)
        {
            switch (_direction)
            {
                case Direction.Up:
                    return ProcessUp(sampleData);

                case Direction.Down:
                default:
                    return ProcessDown(sampleData);
            }
        }
        protected SampledData2h ProcessDown(SampledData2h sampleData) => -ProcessInner(-sampleData);

        protected SampledData2h ProcessUp(SampledData2h sampleData) => ProcessInner(sampleData);

        private SampledData2h ProcessInner(SampledData2h sampleData)
        {
            var height = sampleData.Max - Height.Default;
            SampledData2h normalizedHeightData = (sampleData - height).ClipMin(-2.CreateHeight() * _scaleFactor);
            SampledData2h choosenTemplateTile = _tileMap.GetTile(normalizedHeightData);
            return choosenTemplateTile + height;
        }
    }
}