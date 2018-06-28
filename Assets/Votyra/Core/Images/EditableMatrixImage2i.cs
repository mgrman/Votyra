using System.Collections.Generic;
using System.Linq;
using Votyra.Core.Images.Constraints;
using Votyra.Core.Models;
using Zenject;

namespace Votyra.Core.Images
{
    public class EditableMatrixImage2i : IImage2iProvider, IEditableImage2i
    {
        private readonly Matrix2<int?> _editableMatrix;

        private Range2i? _invalidatedArea;

        private readonly List<LockableMatrix2<int?>> _readonlyMatrices = new List<LockableMatrix2<int?>>();

        private MatrixImage2i _image = null;

        private IImageConstraint2i _constraint;

        public EditableMatrixImage2i([InjectOptional] IImageConstraint2i constraint, IImageConfig imageConfig)
        {
            _constraint = constraint;
            _editableMatrix = new Matrix2<int?>(imageConfig.ImageSize.XY);
        }

        public IImage2i CreateImage()
        {
            if (_invalidatedArea == Range2i.Zero)
            {
                _image?.Dispose();
                _image = new MatrixImage2i(_image.Image, Range2i.Zero);
            }
            else if (_invalidatedArea.HasValue || _image == null)
            {
                _invalidatedArea = _invalidatedArea ?? _editableMatrix.Size.ToRange2i();
                // Debug.LogFormat("Update readonlyCount:{0}", _readonlyMatrices.Count);

                var readonlyMatrix = _readonlyMatrices.FirstOrDefault(o => !o.IsLocked);
                if (readonlyMatrix == null)
                {
                    readonlyMatrix = new LockableMatrix2<int?>(_editableMatrix.Size);
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
                _image = new MatrixImage2i(readonlyMatrix, _invalidatedArea.Value);
                _invalidatedArea = Range2i.Zero;
            }
            return _image;
        }

        public IEditableImageAccessor2i RequestAccess(Range2i areaRequest)
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

        private class MatrixImageAccessor : IEditableImageAccessor2i
        {
            private readonly int?[,] _editableMatrix;
            private int _changeCounter = 0;
            public Range2i Area { get; }

            public int? this[Vector2i pos]
            {
                get
                {
                    return _editableMatrix[pos.X, pos.Y];
                }
                set
                {
                    var existingValue = _editableMatrix[pos.X, pos.Y];
                    if (value.IsNotHole() && existingValue.IsNotHole())
                    {
                        _changeCounter += value.Value - existingValue.Value;
                    }
                    _editableMatrix[pos.X, pos.Y] = value;
                }
            }

            private readonly EditableMatrixImage2i _editableImage;

            public MatrixImageAccessor(EditableMatrixImage2i editableImage, Range2i area)
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