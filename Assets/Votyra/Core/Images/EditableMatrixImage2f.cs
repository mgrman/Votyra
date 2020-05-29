using System.Collections.Generic;
using System.Linq;
using Votyra.Core.Images.Constraints;
using Votyra.Core.Models;

namespace Votyra.Core.Images
{
    public class EditableMatrixImage2f : IImage2fProvider, IEditableImage2f
    {
        private readonly IImageConstraint2i[] _constraints;
        private readonly float[,] _editableMatrix;

        private readonly IImage2fPostProcessor _image2fPostProcessor;

        private readonly List<MatrixImage2f> _readonlyMatrices = new List<MatrixImage2f>();
        private Area1f _editableRangeZ;
        private Range2i? _invalidatedArea;
        private MatrixImage2f _preparedImage;

        public EditableMatrixImage2f(IImageConfig imageConfig, List<IImageConstraint2i> constraints, float defaultValue = 0f)
        {
            this._constraints = constraints.SelectMany(o => o.Priorities.Select(p => new
                {
                    p,
                    o,
                }))
                .OrderBy(o => o.p)
                .Select(o => o.o)
                .ToArray();
            foreach (var constraint in this._constraints)
            {
                constraint.Initialize(this);
            }

            this._editableMatrix = new float[imageConfig.ImageSize.X, imageConfig.ImageSize.Y];
            if (defaultValue != 0f)
            {
                for (var ix = 0; ix < imageConfig.ImageSize.X; ix++)
                {
                    for (var iy = 0; iy < imageConfig.ImageSize.Y; iy++)
                    {
                        this._editableMatrix[ix, iy] = defaultValue;
                    }
                }
            }

            this._editableRangeZ = Area1f.FromMinAndMax(defaultValue, defaultValue);
        }

        private MatrixImage2f PreparedImage
        {
            get => this._preparedImage;
            set
            {
                this._preparedImage?.FinishUsing();
                this._preparedImage = value;
                this._preparedImage?.StartUsing();
            }
        }

        public IEditableImageAccessor2f RequestAccess(Range2i areaRequest) => new MatrixImageAccessor(this, areaRequest);

        public IImage2f CreateImage()
        {
            if ((this._invalidatedArea == Range2i.Zero) && (this.PreparedImage.InvalidatedArea == Range2i.Zero))
            {
            }
            else if (this._invalidatedArea.HasValue || (this.PreparedImage == null))
            {
                this._invalidatedArea = this._invalidatedArea ?? this._editableMatrix.Range();

                this.PreparedImage = this.GetNotUsedImage();
                this.PreparedImage.UpdateImage(this._editableMatrix, this._editableRangeZ);
                this.PreparedImage.UpdateInvalidatedArea(this._invalidatedArea.Value);
                this._invalidatedArea = Range2i.Zero;
            }

            return this.PreparedImage;
        }

        private MatrixImage2f GetNotUsedImage()
        {
            MatrixImage2f image = null;
            for (var i = 0; i < this._readonlyMatrices.Count; i++)
            {
                var o = this._readonlyMatrices[i];
                if (!o.IsBeingUsed)
                {
                    image = o;
                    break;
                }
            }

            if (image == null)
            {
                image = new MatrixImage2f(this._editableMatrix.Size());
                this._readonlyMatrices.Add(image);
            }

            return image;
        }

        private void FixImage(Range2i invalidatedImageArea, Direction direction)
        {
            if (this._invalidatedArea != null)
            {
                invalidatedImageArea = this._invalidatedArea.Value.CombineWith(invalidatedImageArea);
            }

            this._invalidatedArea = invalidatedImageArea;

            for (var i = 0; i < this._constraints.Length; i++)
            {
                var newInvalidatedImageArea = this._constraints[i]
                    .FixImage(this, this._editableMatrix, invalidatedImageArea, direction);
                invalidatedImageArea = invalidatedImageArea.CombineWith(newInvalidatedImageArea);
                this._invalidatedArea = invalidatedImageArea;
            }
        }

        private class MatrixImageAccessor : IEditableImageAccessor2f
        {
            private readonly EditableMatrixImage2f _editableImage;
            private readonly float[,] _editableMatrix;
            private bool _anyChange;
            private float _changeCounter;
            private Area1f _editableRangeZ;

            public MatrixImageAccessor(EditableMatrixImage2f editableImage, Range2i area)
            {
                this._editableMatrix = editableImage._editableMatrix;
                this._editableImage = editableImage;
                this.Area = area.IntersectWith(editableImage._editableMatrix.Range());
            }

            public Range2i Area { get; }

            public float this[Vector2i pos]
            {
                get => this._editableMatrix[pos.X, pos.Y];
                set
                {
                    var existingValue = this._editableMatrix[pos.X, pos.Y];
                    if (value == existingValue)
                    {
                        return;
                    }

                    this._anyChange = true;
                    this._changeCounter += value - existingValue;
                    this._editableMatrix[pos.X, pos.Y] = value;
                    this._editableRangeZ = this._editableRangeZ.UnionWith(value);
                }
            }

            public void Dispose()
            {
                if (this._anyChange)
                {
                    this._editableImage._editableRangeZ = this._editableRangeZ;
                    var direction = this._changeCounter > 0 ? Direction.Up : this._changeCounter < 0 ? Direction.Down : Direction.Unknown;
                    this._editableImage.FixImage(this.Area, direction);
                }
            }
        }
    }
}
