using System.Collections.Generic;
using UnityEngine;
using Votyra.Core.Models;

namespace Votyra.Core.Utils
{
    public static class MeshUtils
    {
        public static void UpdateBounds(this Mesh mesh, Rect xyBounds, IList<Vector3> points)
        {
            var minZ = float.MaxValue;
            var maxZ = float.MinValue;
            for (var i = 0; i < points.Count; i++)
            {
                var z = points[i]
                    .z;
                minZ = Mathf.Min(minZ, z);
                maxZ = Mathf.Max(maxZ, z);
            }

            mesh.UpdateBounds(xyBounds, Area1f.FromMinAndMax(minZ, maxZ));
        }

        public static void UpdateBounds(this Mesh mesh, Rect xyBounds, Area1f rangeZ)
        {
            var center = xyBounds.center;
            var size = xyBounds.size;
            mesh.bounds = new Bounds(new Vector3(center.x, center.y, rangeZ.Center), new Vector3(size.x, size.y, rangeZ.Size));
        }

        public static void SetNormalsOrRecompute(this Mesh mesh, Vector3[] normals)
        {
            if (normals == null || normals.Length != mesh.vertices.Length)
                mesh.RecalculateNormals();
            else
                mesh.normals = normals;
        }

        public static void SetNormalsOrRecompute(this Mesh mesh, List<Vector3> normals)
        {
            if (normals == null || normals.Count != mesh.vertices.Length)
                mesh.RecalculateNormals();
            else
                mesh.SetNormals(normals);
        }
    }
}