using System.Collections.Generic;
using Votyra.Core.Models;

namespace Votyra.Core.TerrainMeshes
{
    public static class TerrainMeshExtensions
    {
        public static void AddTriangle(this ITerrainMesh mesh, Triangle3 triangle)
        {
            mesh.AddTriangle(triangle.a, triangle.b, triangle.c);
        }

        public static void AddQuad(this ITerrainMesh mesh, Vector3f x0y0, Vector3f x0y1, Vector3f x1y0, Vector3f x1y1, bool flipSides)
        {
            if (flipSides)
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

        public static IEnumerable<Triangle3> GetQuadTriangles(Vector3f x0y0, Vector3f x0y1, Vector3f x1y0, Vector3f x1y1, bool flipSides)
        {
            if (flipSides)
            {
                yield return new Triangle3(x0y0, x1y0, x1y1);
                yield return new Triangle3(x1y1, x0y1, x0y0);
            }
            else
            {
                yield return new Triangle3(x0y0, x1y0, x0y1);
                yield return new Triangle3(x1y0, x1y1, x0y1);
            }
        }

        public static void AddWall(this ITerrainMesh mesh, Vector3f a, Vector3f b, Vector3f b_lower, Vector3f a_lower, bool flipSides)
        {
            if (flipSides)
            {
                mesh.AddTriangle(a, b, b_lower);
                mesh.AddTriangle(a, b_lower, a_lower);
            }
            else
            {
                mesh.AddTriangle(a, b, b_lower);
                mesh.AddTriangle(a, b_lower, a_lower);
            }
        }

        public static IEnumerable<Triangle3> GetWallTriangles(Vector3f a, Vector3f b, Vector3f b_lower, Vector3f a_lower, bool flipSides)
        {
            if (flipSides)
            {
                yield return new Triangle3(a, b, b_lower);
                yield return new Triangle3(a, b_lower, a_lower);
            }
            else
            {
                yield return new Triangle3(a, b, b_lower);
                yield return new Triangle3(a, b_lower, a_lower);
            }
        }
    }
}