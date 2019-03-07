using System.Collections.Generic;

namespace Votyra.Core.Models
{
    public struct Plane1f
    {
        public readonly float Normal;

        public readonly float Distance;

        public Plane1f(float inNormal, float d)
        {
            Normal = inNormal.Normalized();
            Distance = d;
        }

        public float GetDistanceToPoint(float inPt) => Vector1fUtils.Dot(Normal, inPt) + Distance;
    }

    public static class Plane1fExtensions
    {
        public static bool TestPlanesAABB(this IReadOnlyList<Plane1f> planes, Area1f bounds)
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
                var radius = Vector1fUtils.Dot(boundsExtent, n);

                if (distance + radius < 0)
                    return false;

                planesCount++;
            }

            return true;
        }
    }
}