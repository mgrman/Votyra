using System.Collections.Generic;
using System.Linq;
using Votyra.Core.Images.Constraints;
using Votyra.Core.Models;

namespace Votyra.Core.Images
{
    public class EditableMatrixImage2fCopy : IEditableImage2f
    {
        private readonly IImageConstraint2i[] _constraints;
        private readonly float[,] _editableMatrix;

        private readonly IImage2fPostProcessor _image2fPostProcessor;

        public EditableMatrixImage2fCopy(IImageConfig imageConfig, List<IImageConstraint2i> constraints)
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
        }

        public IEditableImageAccessor2f RequestAccess(Range2i areaRequest) => new MatrixImageAccessor(this, areaRequest);

        private void FixImage(Range2i invalidatedImageArea, Direction direction)
        {
            for (var i = 0; i < this._constraints.Length; i++)
            {
                var newInvalidatedImageArea = this._constraints[i]
                    .FixImage(this, this._editableMatrix, invalidatedImageArea, direction);
                invalidatedImageArea = invalidatedImageArea.CombineWith(newInvalidatedImageArea);
            }
        }

        private class MatrixImageAccessor : IEditableImageAccessor2f
        {
            private readonly EditableMatrixImage2fCopy _editableImage;
            private readonly float[,] _editableMatrix;
            private bool _anyChange;
            private float _changeCounter;
            private Area1f _editableRangeZ;

            public MatrixImageAccessor(EditableMatrixImage2fCopy editableImage, Range2i area)
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
                    var direction = this._changeCounter > 0 ? Direction.Up : this._changeCounter < 0 ? Direction.Down : Direction.Unknown;
                    this._editableImage.FixImage(this.Area, direction);
                }
            }
        }
    }
}
