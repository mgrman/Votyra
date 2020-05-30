using System.Collections.Generic;
using System.Linq;
using Votyra.Core.Images.Constraints;
using Votyra.Core.Models;

namespace Votyra.Core.Images
{
    public class EditableMatrixImage2FCopy : IEditableImage2F
    {
        private readonly IImageConstraint2I[] constraints;
        private readonly float[,] editableMatrix;

        public EditableMatrixImage2FCopy(IImageConfig imageConfig, List<IImageConstraint2I> constraints)
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
        }

        public IEditableImageAccessor2F RequestAccess(Range2i areaRequest) => new MatrixImageAccessor(this, areaRequest);

        private void FixImage(Range2i invalidatedImageArea, Direction direction)
        {
            for (var i = 0; i < this.constraints.Length; i++)
            {
                var newInvalidatedImageArea = this.constraints[i]
                    .FixImage(this, this.editableMatrix, invalidatedImageArea, direction);
                invalidatedImageArea = invalidatedImageArea.CombineWith(newInvalidatedImageArea);
            }
        }

        private class MatrixImageAccessor : IEditableImageAccessor2F
        {
            private readonly EditableMatrixImage2FCopy editableImage;
            private readonly float[,] editableMatrix;
            private bool anyChange;
            private float changeCounter;
            private Area1f editableRangeZ;

            public MatrixImageAccessor(EditableMatrixImage2FCopy editableImage, Range2i area)
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
                    var direction = this.changeCounter > 0 ? Direction.Up : this.changeCounter < 0 ? Direction.Down : Direction.Unknown;
                    this.editableImage.FixImage(this.Area, direction);
                }
            }
        }
    }
}