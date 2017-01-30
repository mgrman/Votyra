using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public static class ITriangleMeshExtensions
{

    public static void AddQuad(this ITriangleMesh mesh, int quadIndex, Rect rect, HeightData heightData, bool flipSides = false)
    {
        Vector3 x0y0 = new Vector3(rect.xMin, rect.yMin, heightData.x0y0);
        Vector3 x0y1 = new Vector3(rect.xMin, rect.yMax, heightData.x0y1);
        Vector3 x1y0 = new Vector3(rect.xMax, rect.yMin, heightData.x1y0);
        Vector3 x1y1 = new Vector3(rect.xMax, rect.yMax, heightData.x1y1);

        if (flipSides)
        {
            mesh.Add(quadIndex * 2, x0y0, x1y0, x0y1);
            mesh.Add(quadIndex * 2 + 1, x1y0, x1y1, x0y1);
        }
        else
        {
            mesh.Add(quadIndex * 2, x0y0, x1y0, x1y1);
            mesh.Add(quadIndex * 2 + 1, x0y0, x1y1, x0y1);
        }
    }

    public static void AddQuad(this ITriangleMesh mesh, int quadIndex, Vector3 a, Vector3 b, Vector3 c, Vector3 d, bool flipSides = false)
    {
        if (flipSides)
        {
            mesh.Add(quadIndex * 2, a, b, c);
            mesh.Add(quadIndex * 2 + 1, c, d, a);
        }
        else
        {
            mesh.Add(quadIndex * 2, a, b, d);
            mesh.Add(quadIndex * 2 + 1, c, d, b);
        }
    }

    public static ITriangleMesh Clone(this ITriangleMesh mesh)
    {
        var clone = new FixedTriangleMesh(mesh.MeshBounds, mesh.Vertices.Count / 3, mesh.Vertices, mesh.Normals, mesh.UV, mesh.Indices);
        return clone;
    }

}
