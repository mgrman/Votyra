using System.Collections.Generic;
using System.Linq;
using Votyra.Core.Images.Constraints;
using Votyra.Core.Models;

namespace Votyra.Core.Images
{
    public class EditableMatrixImage3b : IImage3bProvider, IEditableImage3b
    {
        private readonly IImageConstraint3b constraint;
        private readonly bool[,,] editableMatrix;

        private readonly List<MatrixImage3b> readonlyMatrices = new List<MatrixImage3b>();
        private Range3i? invalidatedArea;
        private MatrixImage3b preparedImage;

        public EditableMatrixImage3b(IImageConfig imageConfig, IImageConstraint3b constraint = null)
        {
            this.constraint = constraint;
            this.editableMatrix = new bool[imageConfig.ImageSize.X, imageConfig.ImageSize.Y, imageConfig.ImageSize.Z];
        }

        private MatrixImage3b PreparedImage
        {
            get => this.preparedImage;
            set
            {
                this.preparedImage?.FinishUsing();
                this.preparedImage = value;
                this.preparedImage?.StartUsing();
            }
        }

        public IEditableImageAccessor3b RequestAccess(Range3i areaRequest) => new MatrixImageAccessor(this, areaRequest);

        public IImage3b CreateImage()
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

        private MatrixImage3b GetNotUsedImage()
        {
            var image = this.readonlyMatrices.FirstOrDefault(o => !o.IsBeingUsed);
            if (image == null)
            {
                image = new MatrixImage3b(this.editableMatrix.Size());
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

        private class MatrixImageAccessor : IEditableImageAccessor3b
        {
            private readonly EditableMatrixImage3b editableImage;
            private readonly bool[,,] editableMatrix;
            private int changeCounter;

            public MatrixImageAccessor(EditableMatrixImage3b editableImage, Range3i area)
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
