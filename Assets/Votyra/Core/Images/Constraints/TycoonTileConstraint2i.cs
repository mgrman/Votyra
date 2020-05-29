using System.Collections.Generic;
using Votyra.Core.ImageSamplers;
using Votyra.Core.Logging;
using Votyra.Core.Models;

namespace Votyra.Core.Images.Constraints
{
    public class TycoonTileConstraint2i : IImageConstraint2i
    {
        private static TileMap2i _tileMap;
        private static int? _tileMapScaleFactor;
        private readonly IThreadSafeLogger _logger;
        private readonly int _scaleFactor;
        protected Direction _direction;
        protected float[,] _editableMatrix;
        protected Range2i _invalidatedCellArea;

        public TycoonTileConstraint2i(IConstraintConfig constraintConfig, IThreadSafeLogger logger)
        {
            this._logger = logger;
            this._scaleFactor = constraintConfig.ScaleFactor;
            if (this._scaleFactor != _tileMapScaleFactor)
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
                    new SampledData2i(0, -1, -1, 0),
                }.CreateExpandedTileMap2i(this._scaleFactor, this._logger);
                _tileMapScaleFactor = this._scaleFactor;
            }
        }

        public IEnumerable<int> Priorities => new[]
        {
            0,
        };

        void IImageConstraint2i.Initialize(IEditableImage2f image)
        {
        }

        public Range2i FixImage(IEditableImage2f _, float[,] editableMatrix, Range2i invalidatedImageArea, Direction direction)
        {
            this._invalidatedCellArea = invalidatedImageArea.ExtendBothDirections(3);
            if ((direction != Direction.Up) && (direction != Direction.Down))
            {
                direction = Direction.Down;
            }

            this._direction = direction;
            this._editableMatrix = editableMatrix;
            this.Constrain();
            this._invalidatedCellArea = this._invalidatedCellArea.ExtendBothDirections(2);
            return this._invalidatedCellArea;
        }

        protected virtual void Constrain()
        {
            var min = this._invalidatedCellArea.Min;
            var max = this._invalidatedCellArea.Max;
            for (var ix = min.X; ix < max.X; ix++)
            {
                for (var iy = min.Y; iy <= max.Y; iy++)
                {
                    this.ConstrainCell(new Vector2i(ix, iy));
                }
            }
        }

        protected virtual void ConstrainCell(Vector2i cell)
        {
            var cell_x0y0 = cell;
            var cell_x1y1 = new Vector2i(cell.X + 1, cell.Y + 1);
            if (!this._editableMatrix.ContainsIndex(cell_x0y0) || !this._editableMatrix.ContainsIndex(cell_x1y1))
            {
                return;
            }

            var sample = this._editableMatrix.SampleCell(cell)
                .ToSampledData2i();
            var processedSample = this.Process(sample);

            var cell_x0y1 = new Vector2i(cell.X, cell.Y + 1);
            var cell_x1y0 = new Vector2i(cell.X + 1, cell.Y);

            this._editableMatrix.Set(cell_x0y0, processedSample.x0y0);
            this._editableMatrix.Set(cell_x0y1, processedSample.x0y1);
            this._editableMatrix.Set(cell_x1y0, processedSample.x1y0);
            this._editableMatrix.Set(cell_x1y1, processedSample.x1y1);
        }

        protected SampledData2i Process(SampledData2i sampleData)
        {
            switch (this._direction)
            {
                case Direction.Up:
                    return this.ProcessUp(sampleData);

                case Direction.Down:
                    return this.ProcessDown(sampleData);

                case Direction.Unknown:
                default:
                    return sampleData;
            }
        }

        protected SampledData2i ProcessDown(SampledData2i sampleData) => -this.ProcessInner(-sampleData);

        protected SampledData2i ProcessUp(SampledData2i sampleData) => this.ProcessInner(sampleData);

        private SampledData2i ProcessInner(SampledData2i sampleData)
        {
            var height = sampleData.Max;
            var normalizedHeightData = (sampleData - height).ClipMin(-2 * this._scaleFactor);
            var choosenTemplateTile = _tileMap.GetTile(normalizedHeightData);
            return choosenTemplateTile + height;
        }
    }
}
