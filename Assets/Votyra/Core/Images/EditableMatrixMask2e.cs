using System.Collections.Generic;
using System.Linq;
using Votyra.Core.Models;

namespace Votyra.Core.Images
{
    public class EditableMatrixMask2e : IMask2eProvider, IEditableMask2e
    {
        private readonly MaskValues[,] _editableMatrix;

        private readonly IImage2fPostProcessor _image2fPostProcessor;

        private readonly List<MatrixMask2e> _readonlyMatrices = new List<MatrixMask2e>();
        private Range2i? _invalidatedArea;
        private MatrixMask2e _preparedImage;

        public EditableMatrixMask2e(IImageConfig imageConfig)
        {
            _editableMatrix = new MaskValues[imageConfig.ImageSize.X,imageConfig.ImageSize.Y];
        }

        private MatrixMask2e PreparedImage
        {
            get => _preparedImage;
            set
            {
                _preparedImage?.FinishUsing();
                _preparedImage = value;
                _preparedImage?.StartUsing();
            }
        }

        public IEditableMaskAccessor2e RequestAccess(Range2i areaRequest) => new MatrixImageAccessor(this, areaRequest);

        public IMask2e CreateMask()
        {
            if (_invalidatedArea == Range2i.Zero && PreparedImage.InvalidatedArea == Range2i.Zero)
            {
            }
            else if (_invalidatedArea.HasValue || PreparedImage == null)
            {
                _invalidatedArea = _invalidatedArea ?? _editableMatrix.Range();

                PreparedImage = GetNotUsedImage();
                PreparedImage.UpdateImage(_editableMatrix);
                PreparedImage.UpdateInvalidatedArea(_invalidatedArea.Value);
                _invalidatedArea = Range2i.Zero;
            }

            return PreparedImage;
        }

        private MatrixMask2e GetNotUsedImage()
        {
            var image = _readonlyMatrices.FirstOrDefault(o => !o.IsBeingUsed);
            if (image == null)
            {
                image = new MatrixMask2e(_editableMatrix.Size());
                _readonlyMatrices.Add(image);
            }

            return image;
        }

        private void FixImage(Range2i invalidatedImageArea)
        {
            _invalidatedArea = _invalidatedArea?.CombineWith(invalidatedImageArea) ?? invalidatedImageArea;
        }

        private class MatrixImageAccessor : IEditableMaskAccessor2e
        {
            private readonly EditableMatrixMask2e _editableImage;
            private readonly MaskValues[,] _editableMatrix;
            private float _changeCounter;

            public MatrixImageAccessor(EditableMatrixMask2e editableImage, Range2i area)
            {
                _editableMatrix = editableImage._editableMatrix;
                _editableImage = editableImage;
                Area = area.IntersectWith(editableImage._editableMatrix.Range());
            }

            public Range2i Area { get; }

            public MaskValues this[Vector2i pos]
            {
                get => _editableMatrix[pos.X, pos.Y];
                set
                {
                    var existingValue = _editableMatrix[pos.X, pos.Y];
                    _changeCounter += value - existingValue;
                    _editableMatrix[pos.X, pos.Y] = value;
                }
            }

            public void Dispose()
            {
                _editableImage.FixImage(Area);
            }
        }
    }
}