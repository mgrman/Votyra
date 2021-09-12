using System.Collections.Generic;
using UnityEngine;

namespace Votyra.Core.Unity.Utils
{
    public static class BoundsUtils
    {
        public static List<Vector3> GetCorners(this Bounds obj, bool includePosition = true)
        {
            var halfSize = obj.size / 2f;
            var result = new List<Vector3>();
            for (var x = -1; x <= 1; x += 2)
            {
                for (var y = -1; y <= 1; y += 2)
                {
                    for (var z = -1; z <= 1; z += 2)
                    {
                        result.Add((includePosition ? obj.center : Vector3.zero) + new Vector3(halfSize.x * x, halfSize.y * y, halfSize.z * z));
                    }
                }
            }

            return result;
        }
    }
}