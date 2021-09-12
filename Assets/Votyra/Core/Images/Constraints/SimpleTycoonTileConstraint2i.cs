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

        private IComparer<float> _comparer;
        private Func<Vector2i, float> _getValue;

        public SimpleTycoonTileConstraint2i(IConstraintConfig constraintConfig, IThreadSafeLogger logger)
            : base(constraintConfig,logger)
        {
        }

        protected override void Constrain()
        {
            switch (Direction)
            {
                case Direction.Up:
                    _comparer = Comparer<float>.Create((a, b) => -DefaultComparer.Compare(a, b));
                    _getValue = cell => EditableMatrix.SampleCell(cell)
                        .Max;
                    break;

                case Direction.Down:
                default:
                    _comparer = DefaultComparer;
                    _getValue = cell => EditableMatrix.SampleCell(cell)
                        .Min;
                    break;
            }

            base.Constrain();
        }

        protected override void ConstrainCell(Vector2i seedCell)
        {
            var queue = new PrioritySetQueue<Vector2i, float>(EqualityComparer<Vector2i>.Default, _comparer);
            queue.Add(seedCell, _getValue(seedCell));

            while (queue.Count > 0)
            {
                var cell = queue.GetFirst()
                    .Value;
                InvalidatedCellArea = InvalidatedCellArea.CombineWith(cell);

                var sample = EditableMatrix.SampleCell(cell)
                    .ToSampledData2I();
                var processedSample = Process(sample);

                var cellX0Y0 = cell;
                var cellX0Y1 = new Vector2i(cell.X,cell.Y+1);
                var cellX1Y0 = new Vector2i(cell.X+1, cell.Y);
                var cellX1Y1 = new Vector2i(cell.X+1, cell.Y+1);

                var change = 0f;
                if (EditableMatrix.ContainsIndex(cellX0Y0) && EditableMatrix.ContainsIndex(cellX1Y1))
                {
                    change += Math.Abs(EditableMatrix[cellX0Y0] - processedSample.X0Y0);
                    EditableMatrix[cellX0Y0] = processedSample.X0Y0;

                    change += Math.Abs(EditableMatrix[cellX0Y1] - processedSample.X0Y1);
                    EditableMatrix[cellX0Y1] = processedSample.X0Y1;

                    change += Math.Abs(EditableMatrix[cellX1Y0] - processedSample.X1Y0);
                    EditableMatrix[cellX1Y0] = processedSample.X1Y0;

                    change += Math.Abs(EditableMatrix[cellX1Y1] - processedSample.X1Y1);
                    EditableMatrix[cellX1Y1] = processedSample.X1Y1;
                }

                if (change > 0f)
                {
                    const int areaSize = 1;
                    for (var offsetX = -areaSize; offsetX <= areaSize; offsetX++)
                    {
                        for (var offsetY = -areaSize; offsetY <= areaSize; offsetY++)
                        {
                            if (offsetX == 0 && offsetY == 0)
                                continue;
                            var ix = cell.X + offsetX;
                            var iy = cell.Y + offsetY;
                            var newCellToCheck = new Vector2i(ix, iy);
                            var newCellToCheckValue = _getValue(cell);
                            if (EditableMatrix.ContainsIndex(newCellToCheck))
                                queue.Add(newCellToCheck, newCellToCheckValue);
                        }
                    }
                }
            }
        }
    }
}