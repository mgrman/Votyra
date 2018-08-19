using System;
using System.Collections.Generic;
using Votyra.Core.Models;
using Votyra.Core.Utils;

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

        public static void AddQuad(this ITerrainMesh mesh, Vector2i position, SampledData2i data)
        {
            //var x0y0 = new Vector3f(position.X, position.Y, data.x0y0);
            //var x0y1 = new Vector3f(position.X, position.Y + 1, data.x0y1);
            //var x1y0 = new Vector3f(position.X + 1, position.Y, data.x1y0);
            //var x1y1 = new Vector3f(position.X + 1, position.Y + 1, data.x1y1);
            var holeCount = data.GetHoleCount();

            if (holeCount == 1)
            {
                if (data.x0y0.IsHole)
                {
                    var x1y0 = new Vector2f(position.X + 1, position.Y).ToVector3f(data.x1y0, 0);
                    var x1y1 = new Vector2f(position.X + 1, position.Y + 1).ToVector3f(data.x1y1, 0);
                    var x0y1 = new Vector2f(position.X, position.Y + 1).ToVector3f(data.x0y1, 0);
                    mesh.AddTriangle(x1y0, x1y1, x0y1);
                    mesh.AddEmptyTriangle();
                }
                else if (data.x0y1.IsHole)
                {
                    var x0y0 = new Vector2f(position.X, position.Y).ToVector3f(data.x0y0, 0);
                    var x1y0 = new Vector2f(position.X + 1, position.Y).ToVector3f(data.x1y0, 0);
                    var x1y1 = new Vector2f(position.X + 1, position.Y + 1).ToVector3f(data.x1y1, 0);
                    mesh.AddTriangle(x0y0, x1y0, x1y1);
                    mesh.AddEmptyTriangle();
                }
                else if (data.x1y0.IsHole)
                {
                    var x0y0 = new Vector2f(position.X, position.Y).ToVector3f(data.x0y0, 0);
                    var x0y1 = new Vector2f(position.X, position.Y + 1).ToVector3f(data.x0y1, 0);
                    var x1y1 = new Vector2f(position.X + 1, position.Y + 1).ToVector3f(data.x1y1, 0);
                    mesh.AddTriangle(x1y1, x0y1, x0y0);
                    mesh.AddEmptyTriangle();
                }
                else if (data.x1y1.IsHole)
                {
                    var x0y0 = new Vector2f(position.X, position.Y).ToVector3f(data.x0y0, 0);
                    var x1y0 = new Vector2f(position.X + 1, position.Y).ToVector3f(data.x1y0, 0);
                    var x0y1 = new Vector2f(position.X, position.Y + 1).ToVector3f(data.x0y1, 0);
                    mesh.AddTriangle(x0y0, x1y0, x0y1);
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
                var x0y0 = new Vector2f(position.X, position.Y).ToVector3f(data.x0y0, 0);
                var x0y1 = new Vector2f(position.X, position.Y + 1).ToVector3f(data.x0y1, 0);
                var x1y0 = new Vector2f(position.X + 1, position.Y).ToVector3f(data.x1y0, 0);
                var x1y1 = new Vector2f(position.X + 1, position.Y + 1).ToVector3f(data.x1y1, 0);
                if (IsFlipped(data.x0y0, data.x0y1, data.x1y0, data.x1y1))
                {
                    mesh.AddTriangle(x0y0, x1y0, x1y1);
                    mesh.AddTriangle(x1y1, x0y1, x0y0);
                }
                else
                {
                    mesh.AddTriangle(x0y0, x1y0, x0y1);
                    mesh.AddTriangle(x1y0, x1y1, x0y1);
                }
            }
            else
            {
                mesh.AddEmptyTriangle();
                mesh.AddEmptyTriangle();
            }
        }

        public static void AddWallAlongX(this ITerrainMesh mesh, Vector2i position, SampledData2i data, Height minusYres_x0y1, Height minusYres_x1y1)
        {
            if (data.x1y0.IsNotHole && data.x0y0.IsNotHole && minusYres_x0y1.IsNotHole && minusYres_x1y1.IsNotHole)
            {
                var pos_x0y0 = new Vector2f(position.X, position.Y).ToVector3f(data.x0y0, 0);
                var pos_x1y0 = new Vector2f(position.X + 1, position.Y).ToVector3f(data.x1y0, 0);
                var pos_x0y0_lowerX = new Vector2f(position.X, position.Y).ToVector3f(minusYres_x0y1, 0);
                var pos_x1y0_lowerX = new Vector2f(position.X + 1, position.Y).ToVector3f(minusYres_x1y1, 0);
                mesh.AddWall(pos_x1y0, pos_x0y0, pos_x0y0_lowerX, pos_x1y0_lowerX);
            }
            else
            {
                mesh.AddEmptyTriangle();
                mesh.AddEmptyTriangle();
            }
        }

        public static void AddWallAlongY(this ITerrainMesh mesh, Vector2i position, SampledData2i data, Height minusXres_x1y0, Height minusXres_x1y1)
        {
            if (data.x0y0.IsNotHole && data.x0y1.IsNotHole && minusXres_x1y0.IsNotHole && minusXres_x1y1.IsNotHole)
            {
                var pos_x0y0 = new Vector2f(position.X, position.Y).ToVector3f(data.x0y0, 0);
                var pos_x0y1 = new Vector2f(position.X, position.Y + 1).ToVector3f(data.x0y1, 0);
                var pos_x0y0_lowerY = new Vector2f(position.X, position.Y).ToVector3f(minusXres_x1y0, 0);
                var pos_x0y1_lowerY = new Vector2f(position.X, position.Y + 1).ToVector3f(minusXres_x1y1, 0);
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

        public static void AddEmptyTriangle(this ITerrainMesh mesh)
        {
            // mesh.AddTriangle(new Vector3f(0, 0, 0), new Vector3f(1, 0, 0), new Vector3f(0, 1, 0));
        }
    }
}