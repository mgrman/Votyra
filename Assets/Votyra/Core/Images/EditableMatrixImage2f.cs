using System.Collections.Generic;
using System.Linq;
using Votyra.Core.Images.Constraints;
using Votyra.Core.Models;
using Zenject;

namespace Votyra.Core.Images
{
    public class EditableMatrixImage2f : IImage2fProvider, IEditableImage2f
    {
        private readonly Matrix2<float> _editableMatrix;

        private Range2i? _invalidatedArea;

        private readonly List<LockableMatrix2<float>> _readonlyMatrices = new List<LockableMatrix2<float>>();

        private MatrixImage2f _image = null;

        private IImageConstraint2i _constraint;

        public EditableMatrixImage2f([InjectOptional] IImageConstraint2i constraint, IImageConfig imageConfig)
        {
            _constraint = constraint;
            _editableMatrix = new Matrix2<float>(imageConfig.ImageSize.XY);
        }

        public IImage2f CreateImage()
        {
            if (_invalidatedArea == Range2i.Zero)
            {
                _image?.Dispose();
                _image = new MatrixImage2f(_image.Image, Range2i.Zero);
            }
            else if (_invalidatedArea.HasValue || _image == null)
            {
                _invalidatedArea = _invalidatedArea ?? _editableMatrix.Size.ToRange2i();
                // Debug.LogFormat("Update readonlyCount:{0}", _readonlyMatrices.Count);

                var readonlyMatrix = _readonlyMatrices.FirstOrDefault(o => !o.IsLocked);
                if (readonlyMatrix == null)
                {
                    readonlyMatrix = new LockableMatrix2<float>(_editableMatrix.Size);
                    _readonlyMatrices.Add(readonlyMatrix);
                }

                //sync
                _editableMatrix
                    .ForeachPointExlusive(i =>
                    {
                        readonlyMatrix[i] = _editableMatrix[i];
                    });

                // Debug.LogError($"_readonlyMatrices: {_readonlyMatrices.Count}");

                _image?.Dispose();
                _image = new MatrixImage2f(readonlyMatrix, _invalidatedArea.Value);
                _invalidatedArea = Range2i.Zero;
            }
            return _image;
        }

        public IEditableImageAccessor2f RequestAccess(Range2i areaRequest)
        {
            return new MatrixImageAccessor(this, areaRequest);
        }

        private void FixImage(Range2i invalidatedImageArea, Direction direction)
        {
            _invalidatedArea = _invalidatedArea?.CombineWith(invalidatedImageArea) ?? invalidatedImageArea;

            if (_constraint == null)
            {
                return;
            }

            var newInvalidatedImageArea = _constraint.FixImage(_editableMatrix, invalidatedImageArea, direction);

            _invalidatedArea = _invalidatedArea?.CombineWith(newInvalidatedImageArea) ?? newInvalidatedImageArea;
        }

        private class EditableImageWrapper : IImage2f
        {
            private readonly Matrix2<float> _editableMatrix;

            public EditableImageWrapper(Matrix2<float> editableMatrix)
            {
                _editableMatrix = editableMatrix;
            }

            public Range2i InvalidatedArea => Range2i.Zero;

            public Range1f RangeZ => new Range1f(0, 0);

            public float Sample(Vector2i point)
            {
                if (_editableMatrix.ContainsIndex(point))
                {
                    return _editableMatrix[point];
                }
                else
                {
                    return 0;
                }
            }
        }

        private class MatrixImageAccessor : IEditableImageAccessor2f
        {
            private readonly float[,] _editableMatrix;
            private float _changeCounter = 0;
            public Range2i Area { get; }

            public float this[Vector2i pos]
            {
                get
                {
                    return _editableMatrix[pos.X, pos.Y];
                }
                set
                {
                    _changeCounter += value - _editableMatrix[pos.X, pos.Y];
                    _editableMatrix[pos.X, pos.Y] = value;
                }
            }

            private readonly EditableMatrixImage2f _editableImage;

            public MatrixImageAccessor(EditableMatrixImage2f editableImage, Range2i area)
            {
                _editableMatrix = editableImage._editableMatrix.NativeMatrix;
                _editableImage = editableImage;
                Area = area.IntersectWith(editableImage._editableMatrix.Size.ToRange2i());
            }

            public void Dispose()
            {
                this._editableImage.FixImage(Area, _changeCounter > 0 ? Direction.Up : (_changeCounter < 0 ? Direction.Down : Direction.Unknown));
            }
        }
    }
}