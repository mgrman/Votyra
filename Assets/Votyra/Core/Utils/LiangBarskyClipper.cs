using System;
using Votyra.Core.Models;
using Votyra.Core.Pooling;

namespace Votyra.Core.Utils
{
    [Flags]
    public enum Side
    {
        None = 0,
        X0 = 1 << 0,
        X1 = 1 << 1,
        Y0 = 1 << 2,
        Y1 = 1 << 3,
        X0Y0 = X0 | Y0,
        X0Y1 = X0 | Y1,
        X1Y0 = X1 | Y0,
        X1Y1 = X1 | Y1,
    }

    public static class LiangBarskyClipper
    {
        public static bool Compute(Area2f area, Ray2f line, out Line2f res, out Side side)
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
                res = default;
                side = default;
                return false;
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

            side = Side.None;
            if (xn2.IsApproximatelyEqual(area.Min.X))
            {
                side |= Side.X0;
            }

            if (xn2.IsApproximatelyEqual(area.Max.X))
            {
                side |= Side.X1;
            }

            if (yn2.IsApproximatelyEqual(area.Min.Y))
            {
                side |= Side.Y0;
            }

            if (yn2.IsApproximatelyEqual(area.Max.Y))
            {
                side |= Side.Y1;
            }

            if (side == Side.None)
            {
                throw new InvalidOperationException();
            }

            res = new Line2f(line.Origin, new Vector2f(xn2, yn2));

            return true;
        }
    }
}