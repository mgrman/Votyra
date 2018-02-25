using System.Collections.Generic;
using UnityEngine;
using Votyra.Core.Models;

namespace Votyra.Core.Utils
{
    public static class Vector3Utils
    {
        public static Vector3 Half { get { return new Vector3(0.5f, 0.5f, 0.5f); } }

        public static Vector3i ToVector3i(this Vector3 vec)
        {
            return new Vector3i(vec);
        }

        public static Vector2 XY(this Vector3 a)
        {
            return new Vector2(a.x, a.y);
        }

        public static Vector3 SetX(this Vector3 a, float x)
        {
            return new Vector3(x, a.y, a.z);
        }

        public static Vector3 SetY(this Vector3 a, float y)
        {
            return new Vector3(a.x, y, a.z);
        }

        public static Vector3 SetZ(this Vector3 a, float z)
        {
            return new Vector3(a.x, a.y, z);
        }

        public static Vector3 ClipX(this Vector3 a, float minX, float maxX)
        {
            return a.SetX(a.x.Clip(minX, maxX));
        }

        public static Vector3 ClipY(this Vector3 a, float minY, float maxY)
        {
            return a.SetY(a.y.Clip(minY, maxY));
        }

        public static Vector3 ClipZ(this Vector3 a, float minZ, float maxZ)
        {
            return a.SetZ(a.z.Clip(minZ, maxZ));
        }

        public static Vector3 Normalize(this Vector3 vector, Bounds bounds)
        {
            float x = vector.x.Normalize(bounds.min.x, bounds.max.x);
            float y = vector.y.Normalize(bounds.min.y, bounds.max.y);
            float z = vector.z.Normalize(bounds.min.z, bounds.max.z);
            return new Vector3(x, y, z);
        }

        public static Vector3 Denormalize(this Vector3 vector, Bounds bounds)
        {
            float x = vector.x.Denormalize(bounds.min.x, bounds.max.x);
            float y = vector.y.Denormalize(bounds.min.y, bounds.max.y);
            float z = vector.z.Denormalize(bounds.min.z, bounds.max.z);
            return new Vector3(x, y, z);
        }

        public static Vector3 Times(this Vector3 a, Vector3 b)
        {
            return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
        }

        public static Vector3 Average(this IEnumerable<Vector3> items)
        {
            Vector3 sum = new Vector3();
            int count = 0;
            foreach (var item in items)
            {
                sum += item;
                count++;
            }
            return sum / count;
        }
    }
}