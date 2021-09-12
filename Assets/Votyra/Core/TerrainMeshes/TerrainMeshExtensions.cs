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
                mesh.AddTriangle(c, b, a);
            else
                mesh.AddTriangle(a, b, c);
        }

        public static void AddTriangle(this ICollection<Triangle3f> mesh, Vector3f a, Vector3f b, Vector3f c, bool inverted)
        {
            if (inverted)
                mesh.Add(new Triangle3f(c, b, a));
            else
                mesh.Add(new Triangle3f(a, b, c));
        }

        public static void AddQuad(this ITerrainMesh mesh, Vector2f position, SampledData2f data)
        {
            var x0y0 = new Vector2f(position.X, position.Y).ToVector3f(data.x0y0);
            var x0y1 = new Vector2f(position.X, position.Y + 1).ToVector3f(data.x0y1);
            var x1y0 = new Vector2f(position.X + 1, position.Y).ToVector3f(data.x1y0);
            var x1y1 = new Vector2f(position.X + 1, position.Y + 1).ToVector3f(data.x1y1);

            mesh.AddQuad(x0y0, x0y1, x1y0, x1y1);
        }

        public static void AddQuad(this ITerrainMesh mesh, Vector3f? x0y0, Vector3f? x0y1, Vector3f? x1y0, Vector3f? x1y1)
        {
            var holeCount = (x0y0.HasValue ? 0 : 1) + (x0y1.HasValue ? 0 : 1) + (x1y0.HasValue ? 0 : 1) + (x1y1.HasValue ? 0 : 1);

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
                mesh.AddTriangle(x0y0.Value, x1y0.Value, x0y1.Value);
                mesh.AddTriangle(x1y0.Value, x1y1.Value, x0y1.Value);
            }
            else
            {
                mesh.AddEmptyTriangle();
                mesh.AddEmptyTriangle();
            }
        }

        public static void AddEmptyTriangle(this ITerrainMesh mesh)
        {
            // mesh.AddTriangle(new Vector3f(0, 0, 0), new Vector3f(1, 0, 0), new Vector3f(0, 1, 0));
        }
    }
}