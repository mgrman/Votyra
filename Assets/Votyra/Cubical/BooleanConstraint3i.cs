using Votyra.Core.Images;
using Votyra.Core.Models;
using Votyra.Core.ImageSamplers;
using System;
using Votyra.Core.Utils;

namespace Votyra.Core.Images.Constraints
{
    public class BooleanConstraint3i : IImageConstraint3i
    {
        private int counter = 0;
        public Rect3i FixImage(Matrix3<float> editableMatrix, Rect3i invalidatedImageArea, Direction direction)
        {
            invalidatedImageArea = (editableMatrix.size - Vector3i.One).ToRect3i().IntersectWith(invalidatedImageArea);
            try
            {
                if (counter == 0)
                {
                    SetBooleans(invalidatedImageArea, editableMatrix);
                    return invalidatedImageArea;
                }
                return Constrain(invalidatedImageArea, editableMatrix);
            }
            finally
            {
                counter++;
            }
        }

        public void SetBooleans(Rect3i invalidatedCellArea, Matrix3<float> editableMatrix)
        {
            invalidatedCellArea.ForeachPoint((point) =>
           {
               editableMatrix[point] = GetRoundedValue(editableMatrix[point]);
           });
        }

        private float GetRoundedValue(Matrix3<float> editableMatrix, Vector3i point)
        {
            return GetRoundedValue(editableMatrix[point]);
        }
        private float GetRoundedValue(float value)
        {
            return value > 0 ? 1 : 0;
        }

        public Rect3i Constrain(Rect3i invalidatedCellArea, Matrix3<float> editableMatrix)
        {
            var newInvalidatedCellArea = invalidatedCellArea;
            invalidatedCellArea.ForeachPoint((point) =>
            {
                var value = editableMatrix[point];
                if (value >= 0 && value <= 1)
                {
                    editableMatrix[point] = value > 0 ? 1 : 0;
                    return;
                }

                var dist = CalculateDistance(value);
                var valueToSet = value > 0 ? 1 : 0;
                if (dist != 0)
                {
                    var areaToChange = Rect3i.CenterAndExtents(point, new Vector3i(dist, dist, dist));
                    areaToChange = (editableMatrix.size - Vector3i.One).ToRect3i().IntersectWith(areaToChange);
                    newInvalidatedCellArea = newInvalidatedCellArea.CombineWith(areaToChange);

                    areaToChange
                        .ForeachPoint(pointToChange =>
                        {
                            if (GetRoundedValue(editableMatrix[pointToChange]) != valueToSet)
                            {
                                editableMatrix[pointToChange] = valueToSet;
                            }
                        });
                }
                editableMatrix[point] = valueToSet;
            });

            return invalidatedCellArea;
        }

        private int CalculateDistance(float value)
        {
            if (value > 0)
            {
                return Math.Max(MathUtils.RoundToInt(value) - 1, 0);
            }
            else
            {
                return Math.Min(MathUtils.RoundToInt(value), 0);
            }

        }
    }
}