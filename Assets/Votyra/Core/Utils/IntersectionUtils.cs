using System;
using Votyra.Core.Models;
using Votyra.Core.Pooling;

namespace Votyra.Core.Utils
{
    public static class IntersectionUtils
    {
        public static RectangleSegment GetRectangleSegment(Area2f area, Vector2f point)
        {
            var rectangleSide = RectangleSegment.None;
            if (point.X.IsApproximatelyLessOrEqual(area.Min.X))
            {
                rectangleSide |= RectangleSegment.X0;
            }

            if (point.X.IsApproximatelyGreaterOrEqual(area.Max.X))
            {
                rectangleSide |= RectangleSegment.X1;
            }

            if (point.Y.IsApproximatelyLessOrEqual(area.Min.Y))
            {
                rectangleSide |= RectangleSegment.Y0;
            }

            if (point.Y.IsApproximatelyGreaterOrEqual(area.Max.Y))
            {
                rectangleSide |= RectangleSegment.Y1;
            }

            return rectangleSide;
        }

        public static Vector2f LiangBarskyClipper(Area2f area, Ray2f line)
        {
            var lineFrom = line.Origin;
            var lineTo = line.Origin + line.Direction;

            // defining variables
            float p1 = -(lineTo.X - lineFrom.X);
            float p2 = -p1;
            float p3 = -(lineTo.Y - lineFrom.Y);
            float p4 = -p3;

            float q1 = lineFrom.X - area.Min.X;
            float q2 = area.Max.X - lineFrom.X;
            float q3 = lineFrom.Y - area.Min.Y;
            float q4 = area.Max.Y - lineFrom.Y;

            if ((p1 == 0f && q1 < 0) || (p3 == 0f && q3 < 0))
            {
                return Vector2f.NaN;
            }

            float rn2; //= mini(posarr, posind); // minimum of positive array

            if (p1 == 0f)
            {
                if (p3 == 0f)
                {
                    rn2 = 1;
                }
                else if (p3 < 0)
                {
                    rn2 = q4 / p4;
                }
                else
                {
                    rn2 = q3 / p3;
                }
            }
            else if (p1 < 0)
            {
                if (p3 == 0f)
                {
                    rn2 = q2 / p2;
                }
                else if (p3 < 0)
                {
                    rn2 = Math.Min(q2 / p2, q4 / p4);
                }
                else
                {
                    rn2 = Math.Min(q2 / p2, q3 / p3);
                }
            }
            else
            {
                if (p3 == 0f)
                {
                    rn2 = q1 / p1;
                }
                else if (p3 < 0)
                {
                    rn2 = Math.Min(q1 / p1, q4 / p4);
                }
                else
                {
                    rn2 = Math.Min(q1 / p1, q3 / p3);
                }
            }

            float xn2 = lineFrom.X + p2 * rn2;
            float yn2 = lineFrom.Y + p4 * rn2;

            return new Vector2f(xn2, yn2);
        }

        public static bool IntersectLine(Ray2f ray, Line2f line, out Line2f res)
        {
            /* Return intersection of two line, either infinite or line segments.
            Lines are defined as end points in line segments or as any two points in infite lines.

            */
            // Calculate vectors of both lines from given points:
            var v1 = ray.ToAt1 - ray.Origin;
            var v2 = line.To - line.From;

            // If lines are perpendicular then they dont intersect:
            if (Vector2fUtils.Determinant(v1, v2) == 0)
            {
                res = default;
                return false;
            }

            // Using solved equations for intersection of parametric lines
            // we get parameter for first line where they intersect:
            var rayParameter = (line.From.X * v2.Y + ray.Origin.Y * v2.X - line.From.Y * v2.X - ray.Origin.X * v2.Y) / (v1.X * v2.Y - v1.Y * v2.X);

            // If line 2 have zero Y component then we must calculate parameter for line 2 differently:
            float m;
            if (v2.Y != 0)
                m = (ray.Origin.Y + rayParameter * v1.Y - line.From.Y) / v2.Y;
            else
                m = (ray.Origin.X + rayParameter * v1.X - line.From.X) / v2.X;

            // If lines are both line segments we must check whether these lines intersect in the segments:

            if (rayParameter >= 0 && m >= 0 && m <= 1)
            {
                res = new Line2f(ray.Origin, ray.Origin + v1 * rayParameter);
                return true;
            }

            res = default;
            return false;
        }
    }
}