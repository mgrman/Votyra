using System.Collections.Generic;
using System.Linq;
using Votyra.Common.Models;
using Votyra.Images;
using Votyra.Models;
using Votyra.Common.Utils;
using UnityEngine;
using System;
using Votyra.ImageSamplers;
using Votyra.TerrainAlgorithms;

namespace Votyra.Images.EditableImages
{
    internal class EditableMatrixImage : IImage2iProvider, IEditableImage
    {
        private Matrix<int> _editableMatrix;

        private Rect2i? _invalidatedArea;

        private readonly List<LockableMatrix<int>> _readonlyMatrices = new List<LockableMatrix<int>>();

        private MatrixImage _image = null;

        private ITerrainAlgorithm _terrainAlgorithm;
        private IImageSampler _sampler;

        public EditableMatrixImage(Vector2i size, IImageSampler sampler, ITerrainAlgorithm onEditAlgorithm)
        {
            _terrainAlgorithm = onEditAlgorithm;
            _sampler = sampler;

            _editableMatrix = new Matrix<int>(size);
            FixImage(new Rect2i(0, 0, size.x, size.y), Direction.Unknown);
        }

        public EditableMatrixImage(Texture2D texture, float scale, IImageSampler sampler, ITerrainAlgorithm onEditAlgorithm)
        {
            _terrainAlgorithm = onEditAlgorithm;
            _sampler = sampler;

            int width = texture.width;
            int height = texture.height;

            var size = new Vector2i(width, height);
            _editableMatrix = new Matrix<int>(size);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    _editableMatrix[x, y] = (int)(texture.GetPixel(x, y).grayscale * scale);
                }
            }
            FixImage(new Rect2i(0, 0, size.x, size.y), Direction.Unknown);
        }

        public IImage2i CreateImage()
        {
            if (_invalidatedArea.HasValue)
            {
                // Debug.LogFormat("Update readonlyCount:{0}", _readonlyMatrices.Count);

                var readonlyMatrix = _readonlyMatrices.FirstOrDefault(o => !o.IsLocked);
                if (readonlyMatrix == null)
                {
                    readonlyMatrix = new LockableMatrix<int>(_editableMatrix.size);
                    _readonlyMatrices.Add(readonlyMatrix);
                }

                //sync
                for (int x = 0; x < _editableMatrix.size.x; x++)
                {
                    for (int y = 0; y < _editableMatrix.size.y; y++)
                    {
                        readonlyMatrix[x, y] = _editableMatrix[x, y];
                    }
                }

                // Debug.LogError($"_readonlyMatrices: {_readonlyMatrices.Count}");

                var oldImage = _image;
                oldImage?.Dispose();

                _image = new MatrixImage(readonlyMatrix, _invalidatedArea.Value);
                _invalidatedArea = null;
            }
            return _image;
        }

        public IEditableImageAccessor RequestAccess(Rect2i area)
        {
            return new MatrixImageAccessor(this, area);
        }

        private enum Direction
        {
            Unknown = 0,
            Up = 1,
            Down = -1
        }

        private void FixImage(Rect2i area, Direction direction)
        {
            _invalidatedArea = _invalidatedArea?.CombineWith(area) ?? area;

            if (_sampler == null || _terrainAlgorithm == null)
            {
                return;
            }

            if (direction != Direction.Up && direction != Direction.Down)
            {
                direction = Direction.Down;
            }

            var transformedArea = _sampler.ImageToWorld(area);
            var image = new EditableImageWrapper(_editableMatrix);

            var cellMin = transformedArea.min.FloorToVector2i();
            var cellMax = transformedArea.max.CeilToVector2i();

            // Func<Vector2i, bool> processCell = (cell) =>
            // {

            //     var sample = _sampler.Sample(image, cell);

            //     var processedSample = _terrainAlgorithm.Process(sample);

            //     Vector2i cell_x0y0 = _sampler.CellToX0Y0(cell);
            //     Vector2i cell_x0y1 = _sampler.CellToX0Y1(cell);
            //     Vector2i cell_x1y0 = _sampler.CellToX1Y0(cell);
            //     Vector2i cell_x1y1 = _sampler.CellToX1Y1(cell);

            //     if (cell_x0y0.IsAsIndexContained(_editableMatrix.size))
            //         _editableMatrix[cell_x0y0] = processedSample.data.x0y0;
            //     if (cell_x0y1.IsAsIndexContained(_editableMatrix.size))
            //         _editableMatrix[cell_x0y1] = processedSample.data.x0y1;
            //     if (cell_x1y0.IsAsIndexContained(_editableMatrix.size))
            //         _editableMatrix[cell_x1y0] = processedSample.data.x1y0;
            //     if (cell_x1y1.IsAsIndexContained(_editableMatrix.size))
            //         _editableMatrix[cell_x1y1] = processedSample.data.x1y1;

            //     return false;
            // };

            // int maxDist = Math.Max(area.width, area.height);
            // int dist = 1;
            // Vector2i center = area.center;


            // bool isChanged = processCell(center);
            // while (dist < maxDist || isChanged)
            // {
            //     isChanged = false;
            //     for (int ix = center.x - dist; ix <= center.x + dist; ix++)
            //     {
            //         isChanged = isChanged || processCell(new Vector2i(ix, center.y - dist));
            //         isChanged = isChanged || processCell(new Vector2i(ix, center.y + dist));
            //     }
            //     for (int iy = center.y - dist + 1; iy <= center.y + dist - 1; iy++)
            //     {
            //         isChanged = isChanged || processCell(new Vector2i(center.x - dist, iy));
            //         isChanged = isChanged || processCell(new Vector2i(center.x + dist, iy));
            //     }
            //     dist++;
            // }



            for (int ix = cellMin.x; ix <= cellMax.x; ix++)
            {
                for (int iy = cellMin.y; iy <= cellMax.y; iy++)
                {
                    var cell = new Vector2i(ix, iy);
                    var sample = _sampler.Sample(image, cell);

                    var processedSample = _terrainAlgorithm.Process(sample);

                    Vector2i cell_x0y0 = _sampler.CellToX0Y0(cell);
                    Vector2i cell_x0y1 = _sampler.CellToX0Y1(cell);
                    Vector2i cell_x1y0 = _sampler.CellToX1Y0(cell);
                    Vector2i cell_x1y1 = _sampler.CellToX1Y1(cell);

                    if (cell_x0y0.IsAsIndexContained(_editableMatrix.size))
                        _editableMatrix[cell_x0y0] = processedSample.data.x0y0;
                    if (cell_x0y1.IsAsIndexContained(_editableMatrix.size))
                        _editableMatrix[cell_x0y1] = processedSample.data.x0y1;
                    if (cell_x1y0.IsAsIndexContained(_editableMatrix.size))
                        _editableMatrix[cell_x1y0] = processedSample.data.x1y0;
                    if (cell_x1y1.IsAsIndexContained(_editableMatrix.size))
                        _editableMatrix[cell_x1y1] = processedSample.data.x1y1;

                }
            }

            // const int maxDiference = 1;

            // Func<int, int, int> op;
            // Func<int, int> getLimit;
            // if (direction == Direction.Up)
            // {
            //     op = Math.Max;
            //     getLimit = l => l - maxDiference;
            // }
            // else
            // {
            //     op = Math.Min;
            //     getLimit = l => l + maxDiference;
            // }

            // Queue<Vector2i> toCheck = new Queue<Vector2i>();
            // for (int ix = area.xMin.FloorTo2(); ix < area.xMax.CeilTo2(); ix += 2)
            // {
            //     for (int iy = area.yMin.FloorTo2(); iy < area.yMax.CeilTo2(); iy += 2)
            //     {
            //         int x0y0 = _editableMatrix[ix + 0, iy + 0];
            //         int x0y1 = _editableMatrix[ix + 0, iy + 1];
            //         int x1y0 = _editableMatrix[ix + 1, iy + 0];
            //         int x1y1 = _editableMatrix[ix + 1, iy + 1];

            //         int limit = getLimit(op(op(op(x0y0, x0y1), x1y0), x1y1));

            //         _editableMatrix[ix + 0, iy + 0] = op(_editableMatrix[ix + 0, iy + 0], limit);
            //         _editableMatrix[ix + 0, iy + 1] = op(_editableMatrix[ix + 0, iy + 1], limit);
            //         _editableMatrix[ix + 1, iy + 0] = op(_editableMatrix[ix + 1, iy + 0], limit);
            //         _editableMatrix[ix + 1, iy + 1] = op(_editableMatrix[ix + 1, iy + 1], limit);
            //     }
            // }
        }

        private class EditableImageWrapper : IImage2i
        {
            private readonly Matrix<int> _editableMatrix;

            public EditableImageWrapper(Matrix<int> editableMatrix)
            {
                _editableMatrix = editableMatrix;
            }

            public Rect2i InvalidatedArea => Rect2i.zero;

            public Range2i RangeZ => Range2i.Zero;

            public int Sample(Vector2i point)
            {
                if (point.IsAsIndexContained(_editableMatrix.size))
                {
                    return _editableMatrix[point];
                }
                else
                {
                    return 0;
                }
            }
        }

        private class MatrixImageAccessor : IEditableImageAccessor
        {
            public Rect2i Area { get; }

            public int this[Vector2i pos]
            {
                get { return _editableImage._editableMatrix[pos]; }
                set { _editableImage._editableMatrix[pos] = value; }
            }
            private readonly EditableMatrixImage _editableImage;
            public MatrixImageAccessor(EditableMatrixImage editableImage, Rect2i area)
            {
                _editableImage = editableImage;
                Area = area;
            }

            public void Dispose()
            {
                this._editableImage.FixImage(Area, Direction.Unknown);
            }
        }

    }
}