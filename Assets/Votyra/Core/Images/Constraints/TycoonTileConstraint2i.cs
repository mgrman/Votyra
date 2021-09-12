using Votyra.Core.ImageSamplers;
using Votyra.Core.Logging;
using Votyra.Core.Models;

namespace Votyra.Core.Images.Constraints
{
    public class TycoonTileConstraint2i : IImageConstraint2i
    {
        private static TileMap2i _tileMap;
        private static int? _tileMapScaleFactor;
        private readonly int _scaleFactor;
        protected Direction Direction;
        protected Matrix2<float> EditableMatrix;
        protected Range2i InvalidatedCellArea;

        public TycoonTileConstraint2i(IConstraintConfig constraintConfig,IThreadSafeLogger logger)
        {
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
                }.CreateExpandedTileMap2i(_scaleFactor, logger);
                _tileMapScaleFactor = _scaleFactor;
            }
        }

        public Range2i FixImage(Matrix2<float> editableMatrix, Range2i invalidatedImageArea, Direction direction)
        {
            InvalidatedCellArea = invalidatedImageArea;
            if (direction != Direction.Up && direction != Direction.Down)
                direction = Direction.Down;

            Direction = direction;
            EditableMatrix = editableMatrix;
            Constrain();
            return InvalidatedCellArea;
        }

        protected virtual void Constrain()
        {
            var min = InvalidatedCellArea.Min;
            var max = InvalidatedCellArea.Max;
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
            var cellX0Y0 = cell;
            var cellX1Y1 = new Vector2i(cell.X+1, cell.Y+1);
            if (!EditableMatrix.ContainsIndex(cellX0Y0) || !EditableMatrix.ContainsIndex(cellX1Y1))
                return;

            var sample = EditableMatrix.SampleCell(cell)
                .ToSampledData2I();
            var processedSample = Process(sample);

            var cellX0Y1 = new Vector2i(cell.X,cell.Y+1);
            var cellX1Y0 = new Vector2i(cell.X+1, cell.Y);

            EditableMatrix[cellX0Y0] = processedSample.X0Y0;
            EditableMatrix[cellX0Y1] = processedSample.X0Y1;
            EditableMatrix[cellX1Y0] = processedSample.X1Y0;
            EditableMatrix[cellX1Y1] = processedSample.X1Y1;
        }

        protected SampledData2i Process(SampledData2i sampleData)
        {
            switch (Direction)
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