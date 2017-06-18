using UnityEngine;

namespace Votyra.TerrainGenerators.TerrainMeshers.TerrainMeshes
{
    public static class TerrainMeshExtensions
    {

        public static void AddQuad(this ITerrainMesh mesh, Vector3 x0y0, Vector3 x0y1, Vector3 x1y0, Vector3 x1y1, bool flipSides)
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

        public static void AddWall(this ITerrainMesh mesh, Vector3 a, Vector3 b, Vector3 b_lower, Vector3 a_lower, bool flipSides)
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
    }
}