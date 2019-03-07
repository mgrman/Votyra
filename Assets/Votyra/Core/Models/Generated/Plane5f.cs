using System.Collections.Generic;

namespace Votyra.Core.Models
{
    public struct Plane5f
    {
        public readonly Vector5f Normal;

        public readonly float Distance;

        public Plane5f(Vector5f inNormal, float d)
        {
            Normal = inNormal.Normalized();
            Distance = d;
        }

        public float GetDistanceToPoint(Vector5f inPt) => Vector5fUtils.Dot(Normal, inPt) + Distance;
    }

    public static class Plane5fExtensions
    {
        public static bool TestPlanesAABB(this IReadOnlyList<Plane5f> planes, Area5f bounds)
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
                var radius = Vector5fUtils.Dot(boundsExtent, n);

                if (distance + radius < 0)
                    return false;

                planesCount++;
            }

            return true;
        }
    }
}