using System;
using Votyra.Core.Models;
using Votyra.Core.Pooling;

namespace Votyra.Core.Utils
{
    public static class LineSegmentClipper
    {
        public static bool Compute(Area2f area, Ray2f ray, Side previousSide, out Line2f res, out Side side)
        {
            if (previousSide != Side.Y1 && IntersectLine(ray, area.Y0, out res))
            {
                side = Side.Y0;
                return true;
            }
            else
            {
                if (previousSide != Side.Y0 && IntersectLine(ray, area.Y1, out res))
                {
                    side = Side.Y1;
                    return true;
                }
                else
                {
                    if (previousSide != Side.X1 && IntersectLine(ray, area.X0, out res))
                    {
                        side = Side.X0;
                        return true;
                    }
                    else
                    {
                        if (previousSide != Side.X0 && IntersectLine(ray, area.X1, out res))
                        {
                            side = Side.X1;
                            return true;
                        }
                        else
                        {
                            side = default;
                            res = default;
                            return false;
                        }
                    }
                }
            }
        }

        private static bool IntersectLine(Ray2f ray, Line2f line, out Line2f res)
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