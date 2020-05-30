using System.Collections.Generic;
using System.Linq;
using Votyra.Core.Images.Constraints;
using Votyra.Core.Models;

namespace Votyra.Core.Images
{
    public class EditableMatrixImage3B : IImage3BProvider, IEditableImage3B
    {
        private readonly IImageConstraint3B constraint;
        private readonly bool[,,] editableMatrix;

        private readonly List<MatrixImage3B> readonlyMatrices = new List<MatrixImage3B>();
        private Range3i? invalidatedArea;
        private MatrixImage3B preparedImage;

        public EditableMatrixImage3B(IImageConfig imageConfig, IImageConstraint3B constraint = null)
        {
            this.constraint = constraint;
            this.editableMatrix = new bool[imageConfig.ImageSize.X, imageConfig.ImageSize.Y, imageConfig.ImageSize.Z];
        }

        private MatrixImage3B PreparedImage
        {
            get => this.preparedImage;
            set
            {
                this.preparedImage?.FinishUsing();
                this.preparedImage = value;
                this.preparedImage?.StartUsing();
            }
        }

        public IEditableImageAccessor3B RequestAccess(Range3i areaRequest) => new MatrixImageAccessor(this, areaRequest);

        public IImage3B CreateImage()
        {
            if ((this.invalidatedArea == Range3i.Zero) && (this.PreparedImage.InvalidatedArea == Range3i.Zero))
            {
            }
            else if (this.invalidatedArea.HasValue || (this.PreparedImage == null))
            {
                this.invalidatedArea = this.invalidatedArea ?? this.editableMatrix.Range();

                this.PreparedImage = this.GetNotUsedImage();
                this.PreparedImage.UpdateImage(this.editableMatrix);
                this.PreparedImage.UpdateInvalidatedArea(this.invalidatedArea.Value);
                this.invalidatedArea = Range3i.Zero;
            }

            return this.PreparedImage;
        }

        private MatrixImage3B GetNotUsedImage()
        {
            var image = this.readonlyMatrices.FirstOrDefault(o => !o.IsBeingUsed);
            if (image == null)
            {
                image = new MatrixImage3B(this.editableMatrix.Size());
                this.readonlyMatrices.Add(image);
            }

            return image;
        }

        private void FixImage(Range3i invalidatedImageArea, Direction direction)
        {
            this.invalidatedArea = this.invalidatedArea?.CombineWith(invalidatedImageArea) ?? invalidatedImageArea;

            if (this.constraint == null)
            {
                return;
            }

            var newInvalidatedImageArea = this.constraint.FixImage(this.editableMatrix, invalidatedImageArea, direction);
            this.invalidatedArea = this.invalidatedArea?.CombineWith(newInvalidatedImageArea) ?? newInvalidatedImageArea;
        }

        private class MatrixImageAccessor : IEditableImageAccessor3B
        {
            private readonly EditableMatrixImage3B editableImage;
            private readonly bool[,,] editableMatrix;
            private int changeCounter;

            public MatrixImageAccessor(EditableMatrixImage3B editableImage, Range3i area)
            {
                this.editableMatrix = editableImage.editableMatrix;
                this.editableImage = editableImage;
                this.Area = area.IntersectWith(editableImage.editableMatrix.Range());
            }

            public Range3i Area { get; }

            public bool this[Vector3i pos]
            {
                get => this.editableMatrix[pos.X, pos.Y, pos.Z];
                set
                {
                    var existingValue = this.editableMatrix[pos.X, pos.Y, pos.Z];
                    this.changeCounter += (value ? 1 : 0) - (existingValue ? 1 : 0);
                    this.editableMatrix[pos.X, pos.Y, pos.Z] = value;
                }
            }

            public void Dispose()
            {
                var direction = this.changeCounter > 0 ? Direction.Up : this.changeCounter < 0 ? Direction.Down : Direction.Unknown;
                this.editableImage.FixImage(this.Area, direction);
            }
        }
    }
}
