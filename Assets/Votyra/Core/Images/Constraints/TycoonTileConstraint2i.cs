using Votyra.Core.ImageSamplers;
using Votyra.Core.Logging;
using Votyra.Core.Models;

namespace Votyra.Core.Images.Constraints
{
    public class TycoonTileConstraint2i : IImageConstraint2i
    {
        private readonly IThreadSafeLogger _logger;
        private static TileMap2i _tileMap;
        private static int? _tileMapScaleFactor;
        private readonly int _scaleFactor;
        protected Direction _direction;
        protected Matrix2<float> _editableMatrix;
        protected Range2i _invalidatedCellArea;

        public TycoonTileConstraint2i(IConstraintConfig constraintConfig,IThreadSafeLogger logger)
        {
            _logger = logger;
            _scaleFactor = constraintConfig.SimpleSampleScaleFactor;
            if (_scaleFactor != _tileMapScaleFactor)
            {
                _tileMap = new[]
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
                    new SampledData2i(0, -1, -1, 0)
                }.CreateExpandedTileMap2i(_scaleFactor, _logger);
                _tileMapScaleFactor = _scaleFactor;
            }
        }

        public Range2i FixImage(Matrix2<float> editableMatrix, Range2i invalidatedImageArea, Direction direction)
        {
            _invalidatedCellArea = invalidatedImageArea;
            if (direction != Direction.Up && direction != Direction.Down)
                direction = Direction.Down;

            _direction = direction;
            _editableMatrix = editableMatrix;
            Constrain();
            return _invalidatedCellArea;
        }

        protected virtual void Constrain()
        {
            var min = _invalidatedCellArea.Min;
            var max = _invalidatedCellArea.Max;
            for (var ix = min.X; ix < max.X; ix++)
            {
                for (var iy = min.Y; iy <= max.Y; iy++)
                {
                    ConstrainCell(new Vector2i(ix, iy));
                }
            }
        }

        protected virtual void ConstrainCell(Vector2i cell)
        {
            var cell_x0y0 = cell;
            var cell_x1y1 = new Vector2i(cell.X+1, cell.Y+1);
            if (!_editableMatrix.ContainsIndex(cell_x0y0) || !_editableMatrix.ContainsIndex(cell_x1y1))
                return;

            var sample = _editableMatrix.SampleCell(cell)
                .ToSampledData2i();
            var processedSample = Process(sample);

            var cell_x0y1 = new Vector2i(cell.X,cell.Y+1);
            var cell_x1y0 = new Vector2i(cell.X+1, cell.Y);

            _editableMatrix[cell_x0y0] = processedSample.x0y0;
            _editableMatrix[cell_x0y1] = processedSample.x0y1;
            _editableMatrix[cell_x1y0] = processedSample.x1y0;
            _editableMatrix[cell_x1y1] = processedSample.x1y1;
        }

        protected SampledData2i Process(SampledData2i sampleData)
        {
            switch (_direction)
            {
                case Direction.Up:
                    return ProcessUp(sampleData);

                case Direction.Down:
                    return ProcessDown(sampleData);

                case Direction.Unknown:
                default:
                    return sampleData;
            }
        }

        protected SampledData2i ProcessDown(SampledData2i sampleData) => -ProcessInner(-sampleData);

        protected SampledData2i ProcessUp(SampledData2i sampleData) => ProcessInner(sampleData);

        private SampledData2i ProcessInner(SampledData2i sampleData)
        {
            var height = sampleData.Max;
            var normalizedHeightData = (sampleData - height).ClipMin(-2 * _scaleFactor);
            var choosenTemplateTile = _tileMap.GetTile(normalizedHeightData);
            return choosenTemplateTile + height;
        }
    }
}