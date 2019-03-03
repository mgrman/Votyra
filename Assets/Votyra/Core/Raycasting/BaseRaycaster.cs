using System;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine.Assertions;
using Votyra.Core.Images;
using Votyra.Core.Models;
using Votyra.Core.TerrainGenerators.TerrainMeshers;
using Votyra.Core.Utils;

namespace Votyra.Core.Raycasting
{
    public abstract class BaseRaycaster : IRaycaster
    {
        private readonly ITerrainVertexPostProcessor _terrainVertexPostProcessor;

        private static readonly Vector2i Y1Offset = new Vector2i(0, 1);
        private static readonly Vector2i Y0Offset = new Vector2i(0, -1);
        private static readonly Vector2i X1Offset = new Vector2i(1, 0);
        private static readonly Vector2i X0Offset = new Vector2i(-1, 0);

        protected BaseRaycaster(ITerrainVertexPostProcessor terrainVertexPostProcessor)
        {
            _terrainVertexPostProcessor = terrainVertexPostProcessor;
        }

        public virtual Vector2f? Raycast(Ray3f cameraRay)
        {
            float maxDistance = 500;

            var cameraRayXY = cameraRay.XY();

            var startXY = cameraRayXY.Origin;
            var directionNonNormalizedXY = cameraRay.Direction.XY();
            var directionXYMag = directionNonNormalizedXY.Magnitude();
            var endXY = (startXY + directionNonNormalizedXY.Normalized() * maxDistance);

            float GetRayValue(Vector2f point)
            {
                var p = (point - startXY).Magnitude() / directionXYMag;
                return cameraRay.Origin.Z + cameraRay.Direction.Z * p;
            }

            Vector2f? IsHit(Line2f line, Vector2i cell)
            {
                var imageValue = GetValue(line, cell);

                var fromRayValue = GetRayValue(line.From);
                var toRayValue = GetRayValue(line.To);

                var x = (fromRayValue - imageValue.FromValue) / (imageValue.ToValue - imageValue.FromValue - toRayValue + fromRayValue);
                if (x < 0 || x > 1)
                {
                    return null;
                }

                return line.From + (line.To - line.From) * x;
            }

            Vector2f? result = InvokeOnPath(startXY, endXY, IsHit);


            return result;
        }

        protected readonly ref struct LineValues
        {
            public readonly float FromValue;
            public readonly float ToValue;

            public LineValues(float fromValue, float toValue)
            {
                FromValue = fromValue;
                ToValue = toValue;
            }
        }

        protected abstract LineValues GetValue(Line2f pos, Vector2i cell);

        private Vector2f? InvokeOnPath(Vector2f from, Vector2f to, Func<Line2f, Vector2i, Vector2f?> action)
        {
            var direction = to - from;

            var lastCell = FindCell(to);

            var cell = FindCell(from);
            var position = from;
            int counter = 100;
            var offset = Vector2i.Zero;
            var previousOffset = Vector2i.Zero;
            while (cell != lastCell && counter > 0)
            {
                counter--;
                previousOffset = offset;
                var ray = new Ray2f(position, direction);

                var area = MeshCellArea(cell);

                Vector2f? intersection;
                if (previousOffset != Y1Offset && IntersectLine(ray, area.Y0, out intersection))
                {
                    offset = Y0Offset;
                }
                else
                {
                    if (previousOffset != X0Offset && IntersectLine(ray, area.X1, out intersection))
                    {
                        offset = X1Offset;
                    }
                    else
                    {
                        if (previousOffset != X1Offset && IntersectLine(ray, area.X0, out intersection))
                        {
                            offset = X0Offset;
                        }
                        else
                        {
                            if (previousOffset != Y0Offset && IntersectLine(ray, area.Y1, out intersection))
                            {
                                offset = Y1Offset;
                            }
                            else
                            {
                                offset = Vector2i.Zero;
                                intersection = null;
                            }
                        }
                    }
                }


                if (intersection == null)
                {
                    return null;
                }

                var stop = action(new Line2f(position, intersection.Value), cell);
                if (stop.HasValue)
                {
                    return stop;
                }

                cell += offset;
                position = intersection.Value;
            }

            Assert.IsTrue(counter > 0, "InvokeOnPath used too many iterations");
            return null;
        }

        private Vector2i FindCell(Vector2f meshPoint)
        {
            var cell = meshPoint.FloorToVector2i();
            var area = MeshCellArea(cell);
            int counter = 100;
            while (!area.Contains(meshPoint) && counter > 0)
            {
                counter--;
                if (meshPoint.X > area.Max.X)
                {
                    cell += new Vector2i(1, 0);
                }
                else if (meshPoint.X < area.Min.X)
                {
                    cell -= new Vector2i(1, 0);
                }
                else if (meshPoint.Y > area.Max.Y)
                {
                    cell += new Vector2i(0, 1);
                }
                else if (meshPoint.Y < area.Min.Y)
                {
                    cell -= new Vector2i(0, 1);
                }
                else
                {
                    throw new InvalidOperationException();
                }

                area = MeshCellArea(cell);
            }

            Assert.IsTrue(counter > 0, "FindCell used too many iterations");

            return cell;
        }

        private static bool IntersectLine(Ray2f ray, Line2f line, out Vector2f? res)
        {
            res = IntersectLine(ray, line);
            return res != null;
        }

        private static Vector2f? IntersectLine(Ray2f ray, Line2f line)
        {
            /* Return intersection of two line, either infinite or line segments.
            Lines are defined as end points in line segments or as any two points in infite lines.

            */
            // Calculate vectors of both lines from given points:
            Vector2f v1 = ray.ToAt1 - ray.Origin;
            Vector2f v2 = line.To - line.From;

            // If lines are perpendicular then they dont intersect:
            if (Vector2fUtils.Determinant(v1, v2) == 0)
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

            if (rayParameter >= 0 && m >= 0 && m <= 1)
                return (ray.Origin + v1 * rayParameter);
            else
                return null;
        }

        private float GetLinearInterpolatedValue(IImage2f image, Vector2f pos)
        {
            var pos_x0y0 = pos.FloorToVector2i();
            var fraction = pos - pos_x0y0;

            var pos_x0y1 = pos_x0y0 + new Vector2i(0, 1);
            var pos_x1y0 = pos_x0y0 + new Vector2i(1, 0);
            var pos_x1y1 = pos_x0y0 + new Vector2i(1, 1);

            var x0y0 = image.Sample(pos_x0y0);
            var x0y1 = image.Sample(pos_x0y1);
            var x1y0 = image.Sample(pos_x1y0);
            var x1y1 = image.Sample(pos_x1y1);

            return (1f - fraction.X) * (1f - fraction.Y) * x0y0 + fraction.X * (1f - fraction.Y) * x1y0 + (1f - fraction.X) * fraction.Y * x0y1 + fraction.X * fraction.Y * x1y1;
        }

        protected Area2f MeshCellArea(Vector2i cell) => Area2f.FromMinAndMax(ProcessVertex(new Vector2f(cell.X, cell.Y)), ProcessVertex(new Vector2f(cell.X + 1, cell.Y + 1)));

        private Vector2f ProcessVertex(Vector2f point)
        {
            if (_terrainVertexPostProcessor == null)
            {
                return point;
            }
            else
            {
                return _terrainVertexPostProcessor.PostProcessVertex(point.ToVector3f(0))
                    .XY();
            }
        }
    }
}