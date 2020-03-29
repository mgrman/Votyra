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
        private readonly Func<Vector2i, float> _getMaxValue;

        private readonly Func<Vector2i, float> _getMinValue;
        private readonly PrioritySetQueue<Vector2i, float> _queue = new PrioritySetQueue<Vector2i, float>(EqualityComparer<Vector2i>.Default);

        private IComparer<float> _comparer;
        private Func<Vector2i, float> _getValue;

        public SimpleTycoonTileConstraint2i(IConstraintConfig constraintConfig, IThreadSafeLogger logger)
            : base(constraintConfig, logger)
        {
            _getMinValue = GetMinValue;
            _getMaxValue = GetMaxValue;
        }

        protected override void Constrain()
        {
            switch (_direction)
            {
                case Direction.Up:
                    _comparer = InvertedComparer;
                    _getValue = _getMaxValue;
                    break;

                default:
                    _comparer = DefaultComparer;
                    _getValue = _getMinValue;
                    break;
            }

            base.Constrain();
        }

        private float GetMaxValue(Vector2i cell) => _editableMatrix.SampleCell(cell)
            .Max;

        private float GetMinValue(Vector2i cell) => _editableMatrix.SampleCell(cell)
            .Min;

        protected override void ConstrainCell(Vector2i seedCell)
        {
            _queue.Reset(_comparer);
            _queue.Add(seedCell, _getValue(seedCell));

            while (_queue.Count > 0)
            {
                // #if UNITY_EDITOR
                //                 if (!EditorApplication.isPlayingOrWillChangePlaymode)
                //                     return;
                // #endif
                var cell = _queue.GetFirst()
                    .Value;
                _invalidatedCellArea = _invalidatedCellArea.CombineWith(cell);

                var sample = _editableMatrix.SampleCell(cell)
                    .ToSampledData2i();
                var processedSample = Process(sample);

                var cell_x0y0 = cell;
                var cell_x0y1 = new Vector2i(cell.X, cell.Y + 1);
                var cell_x1y0 = new Vector2i(cell.X + 1, cell.Y);
                var cell_x1y1 = new Vector2i(cell.X + 1, cell.Y + 1);

                var change = 0f;
                if (_editableMatrix.ContainsIndex(cell_x0y0) && _editableMatrix.ContainsIndex(cell_x1y1))
                {
                    change += Math.Abs(_editableMatrix.Get(cell_x0y0) - processedSample.x0y0);
                    _editableMatrix.Set(cell_x0y0, processedSample.x0y0);

                    change += Math.Abs(_editableMatrix.Get(cell_x0y1) - processedSample.x0y1);
                    _editableMatrix.Set(cell_x0y1, processedSample.x0y1);

                    change += Math.Abs(_editableMatrix.Get(cell_x1y0) - processedSample.x1y0);
                    _editableMatrix.Set(cell_x1y0, processedSample.x1y0);

                    change += Math.Abs(_editableMatrix.Get(cell_x1y1) - processedSample.x1y1);
                    _editableMatrix.Set(cell_x1y1, processedSample.x1y1);
                }

                if (change > 0f)
                {
                    const int areaSize = 1;
                    for (var offsetX = -areaSize; offsetX <= areaSize; offsetX++)
                    {
                        for (var offsetY = -areaSize; offsetY <= areaSize; offsetY++)
                        {
                            if (offsetX == 0 && offsetY == 0)
                            {
                                continue;
                            }

                            var ix = cell.X + offsetX;
                            var iy = cell.Y + offsetY;
                            var newCellToCheck = new Vector2i(ix, iy);
                            var newCellToCheckValue = _getValue(cell);
                            if (_editableMatrix.ContainsIndex(newCellToCheck))
                            {
                                _queue.Add(newCellToCheck, newCellToCheckValue);
                            }
                        }
                    }
                }
            }
        }
    }
}
