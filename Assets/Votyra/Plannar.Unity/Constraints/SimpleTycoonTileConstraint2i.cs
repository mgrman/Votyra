using System;
using System.Collections.Generic;
using Votyra.Core;
using Votyra.Core.Images;
using Votyra.Core.Images.Constraints;
using Votyra.Core.ImageSamplers;
using Votyra.Core.Models;

namespace Votyra.Plannar.Images.Constraints
{
    public class SimpleTycoonTileConstraint2i : TycoonTileConstraint2i
    {
        private static readonly IComparer<Height1i> DefaultComparer = Comparer<Height1i>.Default;

        private IComparer<Height1i> _comparer;
        private Func<Vector2i, Height1i> _getValue;
        public SimpleTycoonTileConstraint2i([ConfigInject("scaleFactor")] int scaleFactor)
            : base(scaleFactor)
        {
        }

        public override void Constrain()
        {
            switch (_direction)
            {
                case Direction.Up:
                    _comparer = Comparer<Height1i>.Create((a, b) => -(DefaultComparer.Compare(a, b)));
                    _getValue = cell => _editableMatrix.SampleCell(cell).Max;
                    break;

                case Direction.Down:
                default:
                    _comparer = DefaultComparer;
                    _getValue = cell => _editableMatrix.SampleCell(cell).Min;
                    break;
            }

            base.Constrain();
        }

        protected override void ConstrainCell(Vector2i seedCell)
        {
            var queue = new PrioritySetQueue<Vector2i, Height1i>(EqualityComparer<Vector2i>.Default, _comparer);
            queue.Add(seedCell, _getValue(seedCell));

            while (queue.Count > 0)
            {
#if UNITY_EDITOR
                if (!UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
                {
                    return;
                }
#endif
                var cell = queue.GetFirst().Value;
                _invalidatedCellArea = _invalidatedCellArea.CombineWith(cell);

                var sample = _editableMatrix.SampleCell(cell);
                var processedSample = Process(sample);

                Vector2i cell_x0y0 = ImageSampler2iUtils.CellToX0Y0(cell);
                Vector2i cell_x0y1 = ImageSampler2iUtils.CellToX0Y1(cell);
                Vector2i cell_x1y0 = ImageSampler2iUtils.CellToX1Y0(cell);
                Vector2i cell_x1y1 = ImageSampler2iUtils.CellToX1Y1(cell);

                Height1i.Difference change = Height1i.Difference.Zero;
                if (_editableMatrix.ContainsIndex(cell_x0y0) && _editableMatrix.ContainsIndex(cell_x1y1))
                {
                    change += (_editableMatrix[cell_x0y0] - processedSample.x0y0).Abs;
                    _editableMatrix[cell_x0y0] = processedSample.x0y0;

                    change += (_editableMatrix[cell_x0y1] - processedSample.x0y1).Abs;
                    _editableMatrix[cell_x0y1] = processedSample.x0y1;

                    change += (_editableMatrix[cell_x1y0] - processedSample.x1y0).Abs;
                    _editableMatrix[cell_x1y0] = processedSample.x1y0;

                    change += (_editableMatrix[cell_x1y1] - processedSample.x1y1).Abs;
                    _editableMatrix[cell_x1y1] = processedSample.x1y1;
                }
                if (change > Height1i.Difference.Zero)
                {
                    const int areaSize = 1;
                    for (int offsetX = -areaSize; offsetX <= areaSize; offsetX++)
                    {
                        for (int offsetY = -areaSize; offsetY <= areaSize; offsetY++)
                        {
                            if (offsetX == 0 && offsetY == 0)
                                continue;
                            int ix = cell.X + offsetX;
                            int iy = cell.Y + offsetY;
                            Vector2i newCellToCheck = new Vector2i(ix, iy);
                            var newCellToCheckValue = _getValue(cell);
                            if (_editableMatrix.ContainsIndex(newCellToCheck))
                            {
                                queue.Add(newCellToCheck, newCellToCheckValue);
                            }
                        }
                    }
                }
            }
        }

    }
}