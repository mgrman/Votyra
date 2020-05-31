using System;
using System.Collections.Generic;
using Votyra.Core.ImageSamplers;
using Votyra.Core.Logging;
using Votyra.Core.Models;

namespace Votyra.Core.Images.Constraints
{
    public class SimpleTycoonTileConstraint2i : TycoonTileConstraint2i
    {
        private static readonly IComparer<float> DefaultComparer = Comparer<float>.Default;
        private static readonly IComparer<float> InvertedComparer = Comparer<float>.Create((a, b) => -DefaultComparer.Compare(a, b));
        private readonly Func<Vector2i, float> getMaxValue;

        private readonly Func<Vector2i, float> getMinValue;
        private readonly PrioritySetQueue<Vector2i, float> queue = new PrioritySetQueue<Vector2i, float>(EqualityComparer<Vector2i>.Default);

        private IComparer<float> comparer;
        private Func<Vector2i, float> getValue;

        public SimpleTycoonTileConstraint2i(IConstraintConfig constraintConfig, IThreadSafeLogger logger)
            : base(constraintConfig, logger)
        {
            this.getMinValue = this.GetMinValue;
            this.getMaxValue = this.GetMaxValue;
        }

        protected override void Constrain()
        {
            switch (this.Direction)
            {
                case Direction.Up:
                    this.comparer = InvertedComparer;
                    this.getValue = this.getMaxValue;
                    break;

                default:
                    this.comparer = DefaultComparer;
                    this.getValue = this.getMinValue;
                    break;
            }

            base.Constrain();
        }

        private float GetMaxValue(Vector2i cell) => this.EditableMatrix.SampleCell(cell)
            .Max;

        private float GetMinValue(Vector2i cell) => this.EditableMatrix.SampleCell(cell)
            .Min;

        protected override void ConstrainCell(Vector2i seedCell)
        {
            this.queue.Reset(this.comparer);
            this.queue.Add(seedCell, this.getValue(seedCell));

            while (this.queue.Count > 0)
            {
                // #if UNITY_EDITOR
                //                 if (!EditorApplication.isPlayingOrWillChangePlaymode)
                //                     return;
                // #endif
                var cell = this.queue.GetFirst()
                    .Value;

                this.InvalidatedCellArea = this.InvalidatedCellArea.CombineWith(cell);

                var sample = this.EditableMatrix.SampleCell(cell)
                    .ToSampledData2i();

                var processedSample = this.Process(sample);

                var cellX0Y0 = cell;
                var cellX0Y1 = new Vector2i(cell.X, cell.Y + 1);
                var cellX1Y0 = new Vector2i(cell.X + 1, cell.Y);
                var cellX1Y1 = new Vector2i(cell.X + 1, cell.Y + 1);

                var change = 0f;
                if (this.EditableMatrix.ContainsIndex(cellX0Y0) && this.EditableMatrix.ContainsIndex(cellX1Y1))
                {
                    change += Math.Abs(this.EditableMatrix.Get(cellX0Y0) - processedSample.X0Y0);
                    this.EditableMatrix.Set(cellX0Y0, processedSample.X0Y0);

                    change += Math.Abs(this.EditableMatrix.Get(cellX0Y1) - processedSample.X0Y1);
                    this.EditableMatrix.Set(cellX0Y1, processedSample.X0Y1);

                    change += Math.Abs(this.EditableMatrix.Get(cellX1Y0) - processedSample.X1Y0);
                    this.EditableMatrix.Set(cellX1Y0, processedSample.X1Y0);

                    change += Math.Abs(this.EditableMatrix.Get(cellX1Y1) - processedSample.X1Y1);
                    this.EditableMatrix.Set(cellX1Y1, processedSample.X1Y1);
                }

                if (change > 0f)
                {
                    const int areaSize = 1;
                    for (var offsetX = -areaSize; offsetX <= areaSize; offsetX++)
                    {
                        for (var offsetY = -areaSize; offsetY <= areaSize; offsetY++)
                        {
                            if ((offsetX == 0) && (offsetY == 0))
                            {
                                continue;
                            }

                            var ix = cell.X + offsetX;
                            var iy = cell.Y + offsetY;
                            var newCellToCheck = new Vector2i(ix, iy);
                            var newCellToCheckValue = this.getValue(cell);
                            if (this.EditableMatrix.ContainsIndex(newCellToCheck))
                            {
                                this.queue.Add(newCellToCheck, newCellToCheckValue);
                            }
                        }
                    }
                }
            }
        }
    }
}
