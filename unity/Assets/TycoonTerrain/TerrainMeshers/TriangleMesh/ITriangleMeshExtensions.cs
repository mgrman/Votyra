using TycoonTerrain.Common.Models;
using UnityEngine;

namespace TycoonTerrain.TerrainMeshers.TriangleMesh
{
    public static class ITriangleMeshExtensions
    {
        public static void AddQuad(this ITriangleMesh mesh, int quadIndex, Rect rect, ResultHeightData heightData, bool flipSides = false)
        {
            Vector3 x0y0 = new Vector3(rect.xMin, rect.yMin, heightData.data.x0y0);
            Vector3 x0y1 = new Vector3(rect.xMin, rect.yMax, heightData.data.x0y1);
            Vector3 x1y0 = new Vector3(rect.xMax, rect.yMin, heightData.data.x1y0);
            Vector3 x1y1 = new Vector3(rect.xMax, rect.yMax, heightData.data.x1y1);

            if (flipSides)
            {
                mesh.Add(quadIndex * 2, x0y0, x1y0, x1y1);
                mesh.Add(quadIndex * 2 + 1, x1y1, x0y1, x0y0);
            }
            else
            {
                mesh.Add(quadIndex * 2, x0y0, x1y0, x0y1);
                mesh.Add(quadIndex * 2 + 1, x1y0, x1y1, x0y1);
            }
        }

        public static void AddQuad(this ITriangleMesh mesh, int quadIndex, Vector3 x0y0, Vector3 x0y1, Vector3 x1y0, Vector3 x1y1, bool flipSides)
        {
            if (flipSides)
            {
                mesh.Add(quadIndex * 2, x0y0, x1y0, x1y1);
                mesh.Add(quadIndex * 2 + 1, x1y1, x0y1, x0y0);
            }
            else
            {
                mesh.Add(quadIndex * 2, x0y0, x1y0, x0y1);
                mesh.Add(quadIndex * 2 + 1, x1y0, x1y1, x0y1);
            }
        }

        public static void AddWall(this ITriangleMesh mesh, int quadIndex, Vector3 a, Vector3 b, Vector3 b_lower, Vector3 a_lower)
        {
            mesh.Add(quadIndex * 2, a, b, b_lower);
            mesh.Add(quadIndex * 2 + 1, a, b_lower, a_lower);
        }

        public static ITriangleMesh Clone(this ITriangleMesh mesh)
        {
            var clone = new FixedTriangleMesh(mesh.MeshBounds, mesh.Vertices.Length / 3, mesh.Vertices, mesh.Normals, mesh.UV, mesh.Indices);
            return clone;
        }
    }
}