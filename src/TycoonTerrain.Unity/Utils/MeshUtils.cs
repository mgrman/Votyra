using System.Collections.Generic;
using UnityEngine;

namespace TycoonTerrain.Unity.Utils
{
    public static class MeshUtils
    {
        public static void UpdateBounds(this Mesh mesh, Rect xyBounds, IList<Vector3> points)
        {
            float minZ = float.MaxValue;
            float maxZ = float.MinValue;
            for (int i = 0; i < points.Count; i++)
            {
                float z = points[i].z;
                minZ = Mathf.Min(minZ, z);
                maxZ = Mathf.Max(maxZ, z);
            }
            mesh.UpdateBounds(xyBounds, new Common.Models.Range2(minZ, maxZ));
        }

        public static void UpdateBounds(this Mesh mesh, Rect xyBounds, Common.Models.Range2 rangeZ)
        {
            Vector2 center = xyBounds.center;
            Vector2 size = xyBounds.size;
            mesh.bounds = new Bounds(new Vector3(center.x, center.y, rangeZ.Center), new Vector3(size.x, size.y, rangeZ.Size));
        }
    }
}