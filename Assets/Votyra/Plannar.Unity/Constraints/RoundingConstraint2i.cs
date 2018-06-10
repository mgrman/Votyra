using System;
using Votyra.Core.Images;
using Votyra.Core.Images.Constraints;
using Votyra.Core.Models;

namespace Votyra.Plannar.Images.Constraints
{
    public class RoundingConstraint2i : IImageConstraint2i
    {
        public Range2i FixImage(Matrix2<int> editableMatrix, Range2i invalidatedImageArea, Direction direction)
        {
            invalidatedImageArea = (editableMatrix.Size - Vector2i.One).ToRange2i().IntersectWith(invalidatedImageArea);
            invalidatedImageArea.ForeachPointExlusive(i =>
            {
                editableMatrix[i] = editableMatrix[i];
            });
            return invalidatedImageArea;
        }
    }
}