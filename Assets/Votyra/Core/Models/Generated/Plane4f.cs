using System.Collections.Generic;

namespace Votyra.Core.Models
{
    public struct Plane4f
    {
        public readonly Vector4f Normal;

        public readonly float Distance;

        public Plane4f(Vector4f inNormal, float d)
        {
            Normal = inNormal.Normalized();
            Distance = d;
        }

        public float GetDistanceToPoint(Vector4f inPt) => Vector4fUtils.Dot(Normal, inPt) + Distance;
    }

    public static class Plane4fExtensions
    {
        public static bool TestPlanesAABB(this IReadOnlyList<Plane4f> planes, Area4f bounds)
        {
            var boundsCenter = bounds.Center; // center of bounds
            var boundsExtent = bounds.Extents; // half diagonal
            // do intersection test for each active frame

            // while active frames
            var planesCount = 0;
            for (var i = 0; i < planes.Count; i++)
            {
                var p = planes[i];
                var n = p.Normal.Abs();

                var distance = p.GetDistanceToPoint(boundsCenter);
                var radius = Vector4fUtils.Dot(boundsExtent, n);

                if (distance + radius < 0)
                {
                    return false;
                }

                planesCount++;
            }

            return true;
        }
    }
}
