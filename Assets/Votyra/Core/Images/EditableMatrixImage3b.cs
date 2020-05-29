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
            this._constraint = constraint;
            this._editableMatrix = new bool[imageConfig.ImageSize.X, imageConfig.ImageSize.Y, imageConfig.ImageSize.Z];
        }

        private MatrixImage3b PreparedImage
        {
            get => this._preparedImage;
            set
            {
                this._preparedImage?.FinishUsing();
                this._preparedImage = value;
                this._preparedImage?.StartUsing();
            }
        }

        public IEditableImageAccessor3b RequestAccess(Range3i areaRequest) => new MatrixImageAccessor(this, areaRequest);

        public IImage3b CreateImage()
        {
            if ((this._invalidatedArea == Range3i.Zero) && (this.PreparedImage.InvalidatedArea == Range3i.Zero))
            {
            }
            else if (this._invalidatedArea.HasValue || (this.PreparedImage == null))
            {
                this._invalidatedArea = this._invalidatedArea ?? this._editableMatrix.Range();

                this.PreparedImage = this.GetNotUsedImage();
                this.PreparedImage.UpdateImage(this._editableMatrix);
                this.PreparedImage.UpdateInvalidatedArea(this._invalidatedArea.Value);
                this._invalidatedArea = Range3i.Zero;
            }

            return this.PreparedImage;
        }

        private MatrixImage3b GetNotUsedImage()
        {
            var image = this._readonlyMatrices.FirstOrDefault(o => !o.IsBeingUsed);
            if (image == null)
            {
                image = new MatrixImage3b(this._editableMatrix.Size());
                this._readonlyMatrices.Add(image);
            }

            return image;
        }

        private void FixImage(Range3i invalidatedImageArea, Direction direction)
        {
            this._invalidatedArea = this._invalidatedArea?.CombineWith(invalidatedImageArea) ?? invalidatedImageArea;

            if (this._constraint == null)
            {
                return;
            }

            var newInvalidatedImageArea = this._constraint.FixImage(this._editableMatrix, invalidatedImageArea, direction);
            this._invalidatedArea = this._invalidatedArea?.CombineWith(newInvalidatedImageArea) ?? newInvalidatedImageArea;
        }

        private class MatrixImageAccessor : IEditableImageAccessor3b
        {
            private readonly EditableMatrixImage3b _editableImage;
            private readonly bool[,,] _editableMatrix;
            private int _changeCounter;

            public MatrixImageAccessor(EditableMatrixImage3b editableImage, Range3i area)
            {
                this._editableMatrix = editableImage._editableMatrix;
                this._editableImage = editableImage;
                this.Area = area.IntersectWith(editableImage._editableMatrix.Range());
            }

            public Range3i Area { get; }

            public bool this[Vector3i pos]
            {
                get => this._editableMatrix[pos.X, pos.Y, pos.Z];
                set
                {
                    var existingValue = this._editableMatrix[pos.X, pos.Y, pos.Z];
                    this._changeCounter += (value ? 1 : 0) - (existingValue ? 1 : 0);
                    this._editableMatrix[pos.X, pos.Y, pos.Z] = value;
                }
            }

            public void Dispose()
            {
                var direction = this._changeCounter > 0 ? Direction.Up : this._changeCounter < 0 ? Direction.Down : Direction.Unknown;
                this._editableImage.FixImage(this.Area, direction);
            }
        }
    }
}
