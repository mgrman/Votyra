using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Assertions;
using Votyra.Core.Models;

namespace Votyra.Core.Utils
{
    public static class Path2fUtils
    {
        public static void InvokeOnPath(Vector2f from, Vector2f to,  Func<Line2f,bool> action)
        {
            var direction =to - from;

            var lastCell = to.FloorToVector2i();

            var cell = from.FloorToVector2i();
            var position = from;
            int counter = 100;
            while (cell!= lastCell && counter>0)
            {
                var intersection = Intersect(new Ray2f(position, direction), cell);
                if (intersection == null)
                {
                    return;
                }

                var stop = action(new Line2f(position, intersection.Value.intersection));
                if (stop)
                {
                    return;
                }

                position = intersection.Value.intersection;
                cell = intersection.Value.nextCell;
                counter--;
            }

            Assert.IsTrue(counter>0,"InvokeOnPath used too many iterations");
        }


        public static (Vector2f intersection, Vector2i nextCell)? Intersect(Ray2f ray,Vector2i cell)
        {
            return IntersectLine(ray, new Line2f(new Vector2f(cell.X, cell.Y), new Vector2f(cell.X+1, cell.Y)),cell+new Vector2i(0,-1)) 
                ?? IntersectLine(ray, new Line2f(new Vector2f(cell.X+1, cell.Y), new Vector2f(cell.X+1, cell.Y+1)), cell + new Vector2i(1, 0)) 
                ?? IntersectLine(ray, new Line2f(new Vector2f(cell.X+1, cell.Y+1), new Vector2f(cell.X, cell.Y+1)), cell + new Vector2i(0, 1)) 
                ?? IntersectLine(ray, new Line2f(new Vector2f(cell.X, cell.Y+1), new Vector2f(cell.X, cell.Y)), cell + new Vector2i(-1, 0));
        }

        public static (Vector2f, Vector2i)? IntersectLine(Ray2f ray, Line2f line,Vector2i nextCell)
        {
            /* Return intersection of two line, either infinite or line segments.
            Lines are defined as end points in line segments or as any two points in infite lines.

            */
            // Calculate vectors of both lines from given points:
            Vector2f v1 = ray.ToAt1 - ray.Origin;
            Vector2f v2 = line.To - line.From;

            // If lines are perpendicular then they dont intersect:
            if (Vector2f.Determinant(v1, v2) == 0)
                return null;

            // Using solved equations for intersection of parametric lines
            // we get parameter for first line where they intersect:
            float rayParameter = (line.From.X * v2.Y + ray.Origin.Y * v2.X - line.From.Y * v2.X - ray.Origin.X * v2.Y) / (v1.X * v2.Y - v1.Y * v2.X);

            // If line 2 have zero Y component then we must calculate parameter for line 2 differently:
            float m;
            if (v2.Y != 0)
                m = (ray.Origin.Y + rayParameter * v1.Y - line.From.Y) / v2.Y;
            else
                m = (ray.Origin.X + rayParameter * v1.X - line.From.X) / v2.X;

            // If lines are both line segments we must check whether these lines intersect in the segments:

            if (rayParameter > 0 && m >= 0 && m <= 1)
                return (ray.Origin + v1 * rayParameter,nextCell);
            else
                return null;
        }
    }
}