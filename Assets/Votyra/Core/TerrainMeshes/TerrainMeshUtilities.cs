using System.Collections.Generic;
using Votyra.Core.Models;

namespace Votyra.Core.TerrainMeshes
{
    public static class TerrainMeshUtilities
    {
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

        //
        // public static void AddWallAlongX(this ITerrainMesh mesh, Vector2i position, SampledData2f data, SampledMask2e maskData, SampledData2f minusYres, SampledMask2e minusYresMaskData)
        // {
        //     if (maskData.x1y0.IsNotHole() && maskData.x0y0.IsNotHole() && minusYresMaskData.x0y1.IsNotHole() && minusYresMaskData.x1y1.IsNotHole())
        //     {
        //         var pos_x0y0 = new Vector2f(position.X, position.Y).ToVector3f(data.x0y0);
        //         var pos_x1y0 = new Vector2f(position.X + 1, position.Y).ToVector3f(data.x1y0);
        //         var pos_x0y0_lowerX = new Vector2f(position.X, position.Y).ToVector3f(minusYres.x0y1);
        //         var pos_x1y0_lowerX = new Vector2f(position.X + 1, position.Y).ToVector3f(minusYres.x1y1);
        //         mesh.AddWall(pos_x1y0, pos_x0y0, pos_x0y0_lowerX, pos_x1y0_lowerX);
        //     }
        //     else
        //     {
        //         mesh.AddEmptyTriangle();
        //         mesh.AddEmptyTriangle();
        //     }
        // }
        //
        // public static void AddWallAlongY(this ITerrainMesh mesh, Vector2i position, SampledData2f data, SampledMask2e maskData, SampledData2f minusXres, SampledMask2e minusXresMaskData)
        // {
        //     if (maskData.x0y0.IsNotHole() && maskData.x0y1.IsNotHole() && minusXresMaskData.x1y0.IsNotHole() && minusXresMaskData.x1y1.IsNotHole())
        //     {
        //         var pos_x0y0 = new Vector2f(position.X, position.Y).ToVector3f(data.x0y0);
        //         var pos_x0y1 = new Vector2f(position.X, position.Y + 1).ToVector3f(data.x0y1);
        //         var pos_x0y0_lowerY = new Vector2f(position.X, position.Y).ToVector3f(minusXres.x1y0);
        //         var pos_x0y1_lowerY = new Vector2f(position.X, position.Y + 1).ToVector3f(minusXres.x1y1);
        //         mesh.AddWall(pos_x0y0, pos_x0y1, pos_x0y1_lowerY, pos_x0y0_lowerY);
        //     }
        //     else
        //     {
        //         mesh.AddEmptyTriangle();
        //         mesh.AddEmptyTriangle();
        //     }
        // }
        //
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

        //
        // public static void AddWall(this ITerrainMesh mesh, Vector3f? a, Vector3f? b, Vector3f? b_lower, Vector3f? a_lower)
        // {
        //     if (a.HasValue && b.HasValue && b_lower.HasValue && a_lower.HasValue)
        //         AddWall(mesh, (Vector3f?) a.Value, b.Value, b_lower.Value, a_lower.Value);
        // }
        //
        // public static void AddWall(this ITerrainMesh mesh, Vector3f a, Vector3f b, Vector3f b_lower, Vector3f a_lower)
        // {
        //     mesh.AddTriangle(a, b, b_lower);
        //     mesh.AddTriangle(a, b_lower, a_lower);
        // }
        //
        // public static bool IsFlipped(float x0y0, float x0y1, float x1y0, float x1y1) => false;
        //
        // public static bool IsFlipped(Vector3f x0y0, Vector3f x0y1, Vector3f x1y0, Vector3f x1y1) => false;
        //
        // public static void AddEmptyTriangle(this ITerrainMesh mesh)
        // {
        //     // mesh.AddTriangle(new Vector3f(0, 0, 0), new Vector3f(1, 0, 0), new Vector3f(0, 1, 0));
        // }
    }
}
