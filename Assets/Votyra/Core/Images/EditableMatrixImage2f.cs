using System.Collections.Generic;
using System.Linq;
using Votyra.Core.Images.Constraints;
using Votyra.Core.Models;

namespace Votyra.Core.Images
{
    public class EditableMatrixImage2F : IImage2FProvider, IEditableImage2F
    {
        private readonly IImageConstraint2I[] constraints;
        private readonly float[,] editableMatrix;
        private readonly List<MatrixImage2F> readonlyMatrices = new List<MatrixImage2F>();
        private Area1f editableRangeZ;
        private Range2i? invalidatedArea;
        private MatrixImage2F preparedImage;

        public EditableMatrixImage2F(IImageConfig imageConfig, List<IImageConstraint2I> constraints, float defaultValue = 0f)
        {
            this.constraints = constraints.SelectMany(o => o.Priorities.Select(p => new
            {
                p,
                o,
            }))
                .OrderBy(o => o.p)
                .Select(o => o.o)
                .ToArray();
            foreach (var constraint in this.constraints)
            {
                constraint.Initialize(this);
            }

            this.editableMatrix = new float[imageConfig.ImageSize.X, imageConfig.ImageSize.Y];
            if (defaultValue != 0f)
            {
                for (var ix = 0; ix < imageConfig.ImageSize.X; ix++)
                {
                    for (var iy = 0; iy < imageConfig.ImageSize.Y; iy++)
                    {
                        this.editableMatrix[ix, iy] = defaultValue;
                    }
                }
            }

            this.editableRangeZ = Area1f.FromMinAndMax(defaultValue, defaultValue);
        }

        private MatrixImage2F PreparedImage
        {
            get => this.preparedImage;
            set
            {
                this.preparedImage?.FinishUsing();
                this.preparedImage = value;
                this.preparedImage?.StartUsing();
            }
        }

        public IEditableImageAccessor2F RequestAccess(Range2i areaRequest) => new MatrixImageAccessor(this, areaRequest);

        public IImage2F CreateImage()
        {
            if ((this.invalidatedArea == Range2i.Zero) && (this.PreparedImage.InvalidatedArea == Range2i.Zero))
            {
            }
            else if (this.invalidatedArea.HasValue || (this.PreparedImage == null))
            {
                this.invalidatedArea = this.invalidatedArea ?? this.editableMatrix.Range();

                this.PreparedImage = this.GetNotUsedImage();
                this.PreparedImage.UpdateImage(this.editableMatrix, this.editableRangeZ);
                this.PreparedImage.UpdateInvalidatedArea(this.invalidatedArea.Value);
                this.invalidatedArea = Range2i.Zero;
            }

            return this.PreparedImage;
        }

        private MatrixImage2F GetNotUsedImage()
        {
            MatrixImage2F image = null;
            for (var i = 0; i < this.readonlyMatrices.Count; i++)
            {
                var o = this.readonlyMatrices[i];
                if (!o.IsBeingUsed)
                {
                    image = o;
                    break;
                }
            }

            if (image == null)
            {
                image = new MatrixImage2F(this.editableMatrix.Size());
                this.readonlyMatrices.Add(image);
            }

            return image;
        }

        private void FixImage(Range2i invalidatedImageArea, Direction direction)
        {
            if (this.invalidatedArea != null)
            {
                invalidatedImageArea = this.invalidatedArea.Value.CombineWith(invalidatedImageArea);
            }

            this.invalidatedArea = invalidatedImageArea;

            for (var i = 0; i < this.constraints.Length; i++)
            {
                var newInvalidatedImageArea = this.constraints[i]
                    .FixImage(this, this.editableMatrix, invalidatedImageArea, direction);
                invalidatedImageArea = invalidatedImageArea.CombineWith(newInvalidatedImageArea);
                this.invalidatedArea = invalidatedImageArea;
            }
        }

        private class MatrixImageAccessor : IEditableImageAccessor2F
        {
            private readonly EditableMatrixImage2F editableImage;
            private readonly float[,] editableMatrix;
            private bool anyChange;
            private float changeCounter;
            private Area1f editableRangeZ;

            public MatrixImageAccessor(EditableMatrixImage2F editableImage, Range2i area)
            {
                this.editableMatrix = editableImage.editableMatrix;
                this.editableImage = editableImage;
                this.Area = area.IntersectWith(editableImage.editableMatrix.Range());
            }

            public Range2i Area { get; }

            public float this[Vector2i pos]
            {
                get => this.editableMatrix[pos.X, pos.Y];
                set
                {
                    var existingValue = this.editableMatrix[pos.X, pos.Y];
                    if (value == existingValue)
                    {
                        return;
                    }

                    this.anyChange = true;
                    this.changeCounter += value - existingValue;
                    this.editableMatrix[pos.X, pos.Y] = value;
                    this.editableRangeZ = this.editableRangeZ.UnionWith(value);
                }
            }

            public void Dispose()
            {
                if (this.anyChange)
                {
                    this.editableImage.editableRangeZ = this.editableRangeZ;
                    var direction = this.changeCounter > 0 ? Direction.Up : this.changeCounter < 0 ? Direction.Down : Direction.Unknown;
                    this.editableImage.FixImage(this.Area, direction);
                }
            }
        }
    }
}