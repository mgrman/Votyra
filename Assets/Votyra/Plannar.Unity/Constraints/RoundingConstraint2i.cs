using System;
using System.Collections.Generic;
using System.Linq;
using Votyra.Core.Images;
using Votyra.Core.Models;
using Votyra.Core.Utils;
using Votyra.Core.ImageSamplers;
using Votyra.Core.Images.Constraints;

namespace Votyra.Plannar.Images.Constraints
{
    public class RoundingConstraint2i : IImageConstraint2i
    {

        public Rect2i FixImage(Matrix2<float> editableMatrix, Rect2i invalidatedImageArea, Direction direction)
        {
            invalidatedImageArea = (editableMatrix.size - Vector2i.One).ToRect2i().IntersectWith(invalidatedImageArea);
            for (int cell_x = invalidatedImageArea.Min.X; cell_x <= invalidatedImageArea.Max.X; cell_x++)
            {
                for (int cell_y = invalidatedImageArea.Min.Y; cell_y <= invalidatedImageArea.Max.Y; cell_y++)
                {
                    editableMatrix[cell_x, cell_y] = (float)Math.Round(editableMatrix[cell_x, cell_y]);
                }
            }
            return invalidatedImageArea;
        }
    }
}