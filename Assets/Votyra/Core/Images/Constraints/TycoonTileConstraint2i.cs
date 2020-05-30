using System.Collections.Generic;
using Votyra.Core.ImageSamplers;
using Votyra.Core.Logging;
using Votyra.Core.Models;

namespace Votyra.Core.Images.Constraints
{
    public class TycoonTileConstraint2i : IImageConstraint2i
    {
        private static TileMap2i tileMap;
        private static int? tileMapScaleFactor;
        private readonly IThreadSafeLogger logger;
        private readonly int scaleFactor;
        protected Range2i invalidatedCellArea { get; set; }

        protected Direction direction { get; private set; }

        protected float[,] editableMatrix { get; private set; }

        public TycoonTileConstraint2i(IConstraintConfig constraintConfig, IThreadSafeLogger logger)
        {
            this.logger = logger;
            this.scaleFactor = constraintConfig.ScaleFactor;
            if (this.scaleFactor != tileMapScaleFactor)
            {
                tileMap = new[]
                {
                    // plane
                    new SampledData2i(0, 0, 0, 0),

                    // slope
                    new SampledData2i(-1, 0, -1, 0),

                    // slopeDiagonal
                    new SampledData2i(-2, -1, -1, 0),

                    // partialUpSlope
                    new SampledData2i(-1, -1, -1, 0),

                    // partialDownSlope
                    new SampledData2i(-1, 0, 0, 0),

                    // slopeDiagonal
                    new SampledData2i(0, -1, -1, 0),
                }.CreateExpandedTileMap2i(this.scaleFactor, this.logger);
                tileMapScaleFactor = this.scaleFactor;
            }
        }

        public IEnumerable<int> Priorities => new[]
        {
            0,
        };

        public Range2i FixImage(IEditableImage2f _, float[,] editableMatrix, Range2i invalidatedImageArea, Direction direction)
        {
            this.invalidatedCellArea = invalidatedImageArea.ExtendBothDirections(3);
            if ((direction != Direction.Up) && (direction != Direction.Down))
            {
                direction = Direction.Down;
            }

            this.direction = direction;
            this.editableMatrix = editableMatrix;
            this.Constrain();
            this.invalidatedCellArea = this.invalidatedCellArea.ExtendBothDirections(2);
            return this.invalidatedCellArea;
        }

        void IImageConstraint2i.Initialize(IEditableImage2f image)
        {
        }

        protected virtual void Constrain()
        {
            var min = this.invalidatedCellArea.Min;
            var max = this.invalidatedCellArea.Max;
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
            var cellX0Y0 = cell;
            var cellX1Y1 = new Vector2i(cell.X + 1, cell.Y + 1);
            if (!this.editableMatrix.ContainsIndex(cellX0Y0) || !this.editableMatrix.ContainsIndex(cellX1Y1))
            {
                return;
            }

            var sample = this.editableMatrix.SampleCell(cell)
                .ToSampledData2i();
            var processedSample = this.Process(sample);

            var cellX0Y1 = new Vector2i(cell.X, cell.Y + 1);
            var cellX1Y0 = new Vector2i(cell.X + 1, cell.Y);

            this.editableMatrix.Set(cellX0Y0, processedSample.X0Y0);
            this.editableMatrix.Set(cellX0Y1, processedSample.X0Y1);
            this.editableMatrix.Set(cellX1Y0, processedSample.X1Y0);
            this.editableMatrix.Set(cellX1Y1, processedSample.X1Y1);
        }

        protected SampledData2i Process(SampledData2i sampleData)
        {
            switch (this.direction)
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
            var normalizedHeightData = (sampleData - height).ClipMin(-2 * this.scaleFactor);
            var choosenTemplateTile = tileMap.GetTile(normalizedHeightData);
            return choosenTemplateTile + height;
        }
    }
}