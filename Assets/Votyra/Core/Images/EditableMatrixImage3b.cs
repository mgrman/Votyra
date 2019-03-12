using System.Collections.Generic;
using System.Linq;
using Votyra.Core.Images.Constraints;
using Votyra.Core.Models;

namespace Votyra.Core.Images
{
    public class EditableMatrixImage3b : IImage3bProvider, IEditableImage3b
    {
        private readonly IImageConstraint3b _constraint;
        private readonly bool[,,] _editableMatrix;

        private readonly List<MatrixImage3b> _readonlyMatrices = new List<MatrixImage3b>();
        private Range3i? _invalidatedArea;
        private MatrixImage3b _preparedImage;

        public EditableMatrixImage3b(IImageConfig imageConfig, IImageConstraint3b constraint = null)
        {
            _constraint = constraint;
            _editableMatrix = new bool[imageConfig.ImageSize.X, imageConfig.ImageSize.Y, imageConfig.ImageSize.Z];
        }

        private MatrixImage3b PreparedImage
        {
            get => _preparedImage;
            set
            {
                _preparedImage?.FinishUsing();
                _preparedImage = value;
                _preparedImage?.StartUsing();
            }
        }

        public IEditableImageAccessor3b RequestAccess(Range3i areaRequest) => new MatrixImageAccessor(this, areaRequest);

        public IImage3b CreateImage()
        {
            if (_invalidatedArea == Range3i.Zero && PreparedImage.InvalidatedArea == Range3i.Zero)
            {
            }
            else if (_invalidatedArea.HasValue || PreparedImage == null)
            {
                _invalidatedArea = _invalidatedArea ?? _editableMatrix.Range();

                PreparedImage = GetNotUsedImage();
                PreparedImage.UpdateImage(_editableMatrix);
                PreparedImage.UpdateInvalidatedArea(_invalidatedArea.Value);
                _invalidatedArea = Range3i.Zero;
            }

            return PreparedImage;
        }

        private MatrixImage3b GetNotUsedImage()
        {
            var image = _readonlyMatrices.FirstOrDefault(o => !o.IsBeingUsed);
            if (image == null)
            {
                image = new MatrixImage3b(_editableMatrix.Size());
                _readonlyMatrices.Add(image);
            }

            return image;
        }

        private void FixImage(Range3i invalidatedImageArea, Direction direction)
        {
            _invalidatedArea = _invalidatedArea?.CombineWith(invalidatedImageArea) ?? invalidatedImageArea;

            if (_constraint == null)
                return;

            var newInvalidatedImageArea = _constraint.FixImage(_editableMatrix, invalidatedImageArea, direction);
            _invalidatedArea = _invalidatedArea?.CombineWith(newInvalidatedImageArea) ?? newInvalidatedImageArea;
        }

        private class MatrixImageAccessor : IEditableImageAccessor3b
        {
            private readonly EditableMatrixImage3b _editableImage;
            private readonly bool[,,] _editableMatrix;
            private int _changeCounter;

            public MatrixImageAccessor(EditableMatrixImage3b editableImage, Range3i area)
            {
                _editableMatrix = editableImage._editableMatrix;
                _editableImage = editableImage;
                Area = area.IntersectWith(editableImage._editableMatrix.Range());
            }

            public Range3i Area { get; }

            public bool this[Vector3i pos]
            {
                get => _editableMatrix[pos.X, pos.Y, pos.Z];
                set
                {
                    var existingValue = _editableMatrix[pos.X, pos.Y, pos.Z];
                    _changeCounter += (value ? 1 : 0) - (existingValue ? 1 : 0);
                    _editableMatrix[pos.X, pos.Y, pos.Z] = value;
                }
            }

            public void Dispose()
            {
                var direction = _changeCounter > 0 ? Direction.Up : _changeCounter < 0 ? Direction.Down : Direction.Unknown;
                _editableImage.FixImage(Area, direction);
            }
        }
    }
}