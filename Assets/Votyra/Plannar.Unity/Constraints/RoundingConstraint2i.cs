using System;
using Votyra.Core.Images;
using Votyra.Core.Images.Constraints;
using Votyra.Core.Models;

namespace Votyra.Plannar.Images.Constraints
{
    public class RoundingConstraint2i : IImageConstraint2i
    {
        public Rect2i FixImage(Matrix2<float> editableMatrix, Rect2i invalidatedImageArea, Direction direction)
        {
            invalidatedImageArea = (editableMatrix.Size - Vector2i.One).ToRect2i().IntersectWith(invalidatedImageArea);
            for (int ix = invalidatedImageArea.Min.X; ix <= invalidatedImageArea.Max.X; ix++)
            {
                for (int iy = invalidatedImageArea.Min.Y; iy <= invalidatedImageArea.Max.Y; iy++)
                {
                    var i = new Vector2i(ix, iy);
                    editableMatrix[i] = (float)Math.Round(editableMatrix[i]);
                }
            }
            return invalidatedImageArea;
        }
    }
}