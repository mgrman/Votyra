using System.Collections.Generic;
using System.Linq;
using Votyra.Core.Images.Constraints;
using Votyra.Core.Models;

namespace Votyra.Core.Images
{
    public class EditableMatrixImage2f : IImage2fProvider, IEditableImage2f
    {
        private readonly IImageConstraint2i _constraint;
        private readonly Matrix2<float> _editableMatrix;

        private readonly IImage2fPostProcessor _image2fPostProcessor;

        private readonly List<MatrixImage2f> _readonlyMatrices = new List<MatrixImage2f>();
        private Area1f _editableRangeZ;
        private Range2i? _invalidatedArea;
        private MatrixImage2f _preparedImage;

        public EditableMatrixImage2f(IImageConfig imageConfig, IImageConstraint2i constraint = null)
        {
            _constraint = constraint;
            _editableMatrix = new Matrix2<float>(imageConfig.ImageSize.XY);
            _editableRangeZ = new Area1f(_editableMatrix[0, 0], _editableMatrix[0, 0]);
        }

        private MatrixImage2f PreparedImage
        {
            get => _preparedImage;
            set
            {
                _preparedImage?.FinishUsing();
                _preparedImage = value;
                _preparedImage?.StartUsing();
            }
        }

        public IEditableImageAccessor2f RequestAccess(Range2i areaRequest) => new MatrixImageAccessor(this, areaRequest);

        public IImage2f CreateImage()
        {
            if (_invalidatedArea == Range2i.Zero && PreparedImage.InvalidatedArea == Range2i.Zero)
            {
            }
            else if (_invalidatedArea.HasValue || PreparedImage == null)
            {
                _invalidatedArea = _invalidatedArea ?? _editableMatrix.Size.ToRange2i();

                PreparedImage = GetNotUsedImage();
                PreparedImage.UpdateImage(_editableMatrix, _editableRangeZ);
                PreparedImage.UpdateInvalidatedArea(_invalidatedArea.Value);
                _invalidatedArea = Range2i.Zero;
            }

            return PreparedImage;
        }

        private MatrixImage2f GetNotUsedImage()
        {
            var image = _readonlyMatrices.FirstOrDefault(o => !o.IsBeingUsed);
            if (image == null)
            {
                image = new MatrixImage2f(_editableMatrix.Size);
                _readonlyMatrices.Add(image);
            }

            return image;
        }

        private void FixImage(Range2i invalidatedImageArea, Direction direction)
        {
            _invalidatedArea = _invalidatedArea?.CombineWith(invalidatedImageArea) ?? invalidatedImageArea;

            if (_constraint == null)
                return;

            var newInvalidatedImageArea = _constraint.FixImage(_editableMatrix, invalidatedImageArea, direction);
            _invalidatedArea = _invalidatedArea?.CombineWith(newInvalidatedImageArea) ?? newInvalidatedImageArea;
        }

        private class MatrixImageAccessor : IEditableImageAccessor2f
        {
            private readonly EditableMatrixImage2f _editableImage;
            private readonly float[,] _editableMatrix;
            private float _changeCounter;
            private Area1f _editableRangeZ;

            public MatrixImageAccessor(EditableMatrixImage2f editableImage, Range2i area)
            {
                _editableMatrix = editableImage._editableMatrix.NativeMatrix;
                _editableImage = editableImage;
                Area = area.IntersectWith(editableImage._editableMatrix.Size.ToRange2i());
            }

            public Range2i Area { get; }

            public float this[Vector2i pos]
            {
                get => _editableMatrix[pos.X, pos.Y];
                set
                {
                    var existingValue = _editableMatrix[pos.X, pos.Y];
                    _changeCounter += value - existingValue;
                    _editableMatrix[pos.X, pos.Y] = value;
                    _editableRangeZ = _editableRangeZ.UnionWith(value);
                }
            }

            public void Dispose()
            {
                _editableImage._editableRangeZ = _editableRangeZ;
                var direction = _changeCounter > 0 ? Direction.Up : _changeCounter < 0 ? Direction.Down : Direction.Unknown;
                _editableImage.FixImage(Area, direction);
            }
        }
    }
}