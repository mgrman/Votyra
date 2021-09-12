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
            var x0Y0 = new Vector2f(position.X, position.Y).ToVector3f(data.X0Y0);
            var x0Y1 = new Vector2f(position.X, position.Y + 1).ToVector3f(data.X0Y1);
            var x1Y0 = new Vector2f(position.X + 1, position.Y).ToVector3f(data.X1Y0);
            var x1Y1 = new Vector2f(position.X + 1, position.Y + 1).ToVector3f(data.X1Y1);

            mesh.AddQuad(x0Y0, x0Y1, x1Y0, x1Y1);
        }

        public static void AddQuad(this ITerrainMesh mesh, Vector3f? x0Y0, Vector3f? x0Y1, Vector3f? x1Y0, Vector3f? x1Y1)
        {
            var holeCount = (x0Y0.HasValue ? 0 : 1) + (x0Y1.HasValue ? 0 : 1) + (x1Y0.HasValue ? 0 : 1) + (x1Y1.HasValue ? 0 : 1);

            if (holeCount == 1)
            {
                if (!x0Y0.HasValue)
                {
                    mesh.AddTriangle(x1Y0.Value, x1Y1.Value, x0Y1.Value);
                    mesh.AddEmptyTriangle();
                }
                else if (!x0Y1.HasValue)
                {
                    mesh.AddTriangle(x0Y0.Value, x1Y0.Value, x1Y1.Value);
                    mesh.AddEmptyTriangle();
                }
                else if (!x1Y0.HasValue)
                {
                    mesh.AddTriangle(x1Y1.Value, x0Y1.Value, x0Y0.Value);
                    mesh.AddEmptyTriangle();
                }
                else if (!x1Y1.HasValue)
                {
                    mesh.AddTriangle(x0Y0.Value, x1Y0.Value, x0Y1.Value);
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
                mesh.AddTriangle(x0Y0.Value, x1Y0.Value, x0Y1.Value);
                mesh.AddTriangle(x1Y0.Value, x1Y1.Value, x0Y1.Value);
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