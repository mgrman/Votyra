using System.Collections.Generic;
using UnityEngine;
using Votyra.Core.Models;

namespace Votyra.Core.Utils
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
            mesh.UpdateBounds(xyBounds, new Range1f(minZ, maxZ));
        }

        public static void UpdateBounds(this Mesh mesh, Rect xyBounds, Range1f rangeZ)
        {
            Vector2 center = xyBounds.center;
            Vector2 size = xyBounds.size;
            mesh.bounds = new Bounds(new Vector3(center.x, center.y, rangeZ.Center), new Vector3(size.x, size.y, rangeZ.Size));
        }

        public static void SetNormalsOrRecompute(this Mesh mesh, Vector3[] normals)
        {
            if (normals == null || normals.Length != mesh.vertices.Length)
            {
                mesh.RecalculateNormals();
            }
            else
            {
                mesh.normals = normals;
            }
        }

        public static void SetNormalsOrRecompute(this Mesh mesh, List<Vector3> normals)
        {
            if (normals == null || normals.Count != mesh.vertices.Length)
            {
                mesh.RecalculateNormals();
            }
            else
            {
                mesh.SetNormals(normals);
            }
        }
    }
}