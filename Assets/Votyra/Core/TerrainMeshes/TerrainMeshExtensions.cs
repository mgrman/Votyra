using System.Collections.Generic;
using Votyra.Core.Models;

namespace Votyra.Core.TerrainMeshes
{
    public static class TerrainMeshExtensions
    {
        public static void AddTriangle(this ITerrainMesh mesh, Triangle3f triangle)
        {
            mesh.AddTriangle(triangle.A, triangle.B, triangle.C);
        }

        public static void AddTriangle(this ITerrainMesh mesh, Vector3f a, Vector3f b, Vector3f c, bool inverted)
        {
            if (inverted)
            {
                mesh.AddTriangle(c, b, a);
            }
            else
            {
                mesh.AddTriangle(a, b, c);
            }
        }

        public static void AddTriangle(this ICollection<Triangle3f> mesh, Vector3f a, Vector3f b, Vector3f c, bool inverted)
        {
            if (inverted)
            {
                mesh.Add(new Triangle3f(c, b, a));
            }
            else
            {
                mesh.Add(new Triangle3f(a, b, c));
            }
        }

        public static void AddQuad(this ITerrainMesh mesh, Vector2i position, SampledData2h data, SampledMask2e maskData)
        {
            var x0y0 = maskData.x0y0.IsNotHole() ? new Vector2f(position.X, position.Y).ToVector3f(data.x0y0) : (Vector3f?)null;
            var x0y1 = maskData.x0y1.IsNotHole() ? new Vector2f(position.X, position.Y + 1).ToVector3f(data.x0y1) : (Vector3f?)null;
            var x1y0 = maskData.x1y0.IsNotHole() ? new Vector2f(position.X + 1, position.Y).ToVector3f(data.x1y0) : (Vector3f?)null;
            var x1y1 = maskData.x1y1.IsNotHole() ? new Vector2f(position.X + 1, position.Y + 1).ToVector3f(data.x1y1) : (Vector3f?)null;
            mesh.AddQuad(x0y0, x0y1, x1y0, x1y1);
        }

        public static void AddQuad(this ITerrainMesh mesh, Vector3f? x0y0, Vector3f? x0y1, Vector3f? x1y0, Vector3f? x1y1)
        {
            var holeCount = (x0y0.HasValue ? 0 : 1)
                + (x0y1.HasValue ? 0 : 1)
                + (x1y0.HasValue ? 0 : 1)
                + (x1y1.HasValue ? 0 : 1);

            if (holeCount == 1)
            {
                if (!x0y0.HasValue)
                {
                    mesh.AddTriangle(x1y0.Value, x1y1.Value, x0y1.Value);
                    mesh.AddEmptyTriangle();
                }
                else if (!x0y1.HasValue)
                {
                    mesh.AddTriangle(x0y0.Value, x1y0.Value, x1y1.Value);
                    mesh.AddEmptyTriangle();
                }
                else if (!x1y0.HasValue)
                {
                    mesh.AddTriangle(x1y1.Value, x0y1.Value, x0y0.Value);
                    mesh.AddEmptyTriangle();
                }
                else if (!x1y1.HasValue)
                {
                    mesh.AddTriangle(x0y0.Value, x1y0.Value, x0y1.Value);
                    mesh.AddEmptyTriangle();
                }
                else
                {
                    mesh.AddEmptyTriangle();
                    mesh.AddEmptyTriangle();
                }
            }
            else if (holeCount == 0)
            {
                if (IsFlipped(x0y0.Value, x0y1.Value, x1y0.Value, x1y1.Value))
                {
                    mesh.AddTriangle(x0y0.Value, x1y0.Value, x1y1.Value);
                    mesh.AddTriangle(x1y1.Value, x0y1.Value, x0y0.Value);
                }
                else
                {
                    mesh.AddTriangle(x0y0.Value, x1y0.Value, x0y1.Value);
                    mesh.AddTriangle(x1y0.Value, x1y1.Value, x0y1.Value);
                }
            }
            else
            {
                mesh.AddEmptyTriangle();
                mesh.AddEmptyTriangle();
            }
        }

        public static void AddWallAlongX(this ITerrainMesh mesh, Vector2i position, SampledData2h data, SampledMask2e maskData, SampledData2h minusYres, SampledMask2e minusYresMaskData)
        {
            if (maskData.x1y0.IsNotHole() && maskData.x0y0.IsNotHole() && minusYresMaskData.x0y1.IsNotHole() && minusYresMaskData.x1y1.IsNotHole())
            {
                var pos_x0y0 = new Vector2f(position.X, position.Y).ToVector3f(data.x0y0);
                var pos_x1y0 = new Vector2f(position.X + 1, position.Y).ToVector3f(data.x1y0);
                var pos_x0y0_lowerX = new Vector2f(position.X, position.Y).ToVector3f(minusYres.x0y1);
                var pos_x1y0_lowerX = new Vector2f(position.X + 1, position.Y).ToVector3f(minusYres.x1y1);
                mesh.AddWall(pos_x1y0, pos_x0y0, pos_x0y0_lowerX, pos_x1y0_lowerX);
            }
            else
            {
                mesh.AddEmptyTriangle();
                mesh.AddEmptyTriangle();
            }
        }

        public static void AddWallAlongY(this ITerrainMesh mesh, Vector2i position, SampledData2h data, SampledMask2e maskData, SampledData2h minusXres, SampledMask2e minusXresMaskData)
        {
            if (maskData.x0y0.IsNotHole() && maskData.x0y1.IsNotHole() && minusXresMaskData.x1y0.IsNotHole() && minusXresMaskData.x1y1.IsNotHole())
            {
                var pos_x0y0 = new Vector2f(position.X, position.Y).ToVector3f(data.x0y0);
                var pos_x0y1 = new Vector2f(position.X, position.Y + 1).ToVector3f(data.x0y1);
                var pos_x0y0_lowerY = new Vector2f(position.X, position.Y).ToVector3f(minusXres.x1y0);
                var pos_x0y1_lowerY = new Vector2f(position.X, position.Y + 1).ToVector3f(minusXres.x1y1);
                mesh.AddWall(pos_x0y0, pos_x0y1, pos_x0y1_lowerY, pos_x0y0_lowerY);
            }
            else
            {
                mesh.AddEmptyTriangle();
                mesh.AddEmptyTriangle();
            }
        }

        public static IEnumerable<Triangle3f> GetQuadTriangles(Vector3f x0y0, Vector3f x0y1, Vector3f x1y0, Vector3f x1y1, bool isFlipped)
        {
            if (isFlipped)
            {
                yield return new Triangle3f(x0y0, x1y0, x1y1);
                yield return new Triangle3f(x1y1, x0y1, x0y0);
            }
            else
            {
                yield return new Triangle3f(x0y0, x1y0, x0y1);
                yield return new Triangle3f(x1y0, x1y1, x0y1);
            }
        }

        public static void AddWall(this ITerrainMesh mesh, Vector3f? a, Vector3f? b, Vector3f? b_lower, Vector3f? a_lower)
        {
            if (a.HasValue && b.HasValue && b_lower.HasValue && a_lower.HasValue)
            {
                mesh.AddWall(a.Value, b.Value, b_lower.Value, a_lower.Value);
            }
        }

        public static void AddWall(this ITerrainMesh mesh, Vector3f a, Vector3f b, Vector3f b_lower, Vector3f a_lower)
        {
            mesh.AddTriangle(a, b, b_lower);
            mesh.AddTriangle(a, b_lower, a_lower);
        }

        public static bool IsFlipped(Height x0y0, Height x0y1, Height x1y0, Height x1y1)
        {
            //TODO
            return false;
            // var difMain = MathUtils.Abs(x0y0 - x1y1);
            // var difMinor = MathUtils.Abs(x1y0 - x0y1);
            // bool flip;
            // if (difMain == difMinor)
            // {
            //     var sumMain = x0y0 + x1y1;
            //     var sumMinor = x1y0 + x0y1;
            //     flip = sumMain < sumMinor;
            // }
            // else
            // {
            //     flip = difMain < difMinor;
            // }
            // return flip;
        }

        public static bool IsFlipped(Vector3f x0y0, Vector3f x0y1, Vector3f x1y0, Vector3f x1y1)
        {
            //TODO
            return false;
            // var difMain = MathUtils.Abs(x0y0 - x1y1);
            // var difMinor = MathUtils.Abs(x1y0 - x0y1);
            // bool flip;
            // if (difMain == difMinor)
            // {
            //     var sumMain = x0y0 + x1y1;
            //     var sumMinor = x1y0 + x0y1;
            //     flip = sumMain < sumMinor;
            // }
            // else
            // {
            //     flip = difMain < difMinor;
            // }
            // return flip;
        }

        public static void AddEmptyTriangle(this ITerrainMesh mesh)
        {
            // mesh.AddTriangle(new Vector3f(0, 0, 0), new Vector3f(1, 0, 0), new Vector3f(0, 1, 0));
        }
    }
}