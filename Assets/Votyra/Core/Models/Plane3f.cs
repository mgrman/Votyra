using System;
using System.Collections.Generic;

namespace Votyra.Core.Models
{
    public struct Plane3f
    {
        public readonly Vector3f Normal;

        public readonly float Distance;

        public Plane3f(Vector3f inNormal, float d)
        {
            Normal = inNormal.Normalized;
            Distance = d;
        }

        public float GetDistanceToPoint(Vector3f inPt) => Vector3f.Dot(Normal, inPt) + Distance;
    }

    public static class Plane3fExtensions
    {
        public static bool TestPlanesAABB(this IReadOnlyList<Plane3f> planes, Area3f bounds)
        {
            var boundsCenter = bounds.Center; // center of bounds
            var boundsExtent = bounds.Extents; // half diagonal
            // do intersection test for each active frame

            // while active frames
            for (var i = 0; i < planes.Count; i++)
            {
                var p = planes[i];
                var n = new Vector3f(Math.Abs(p.Normal.X), Math.Abs(p.Normal.Y), Math.Abs(p.Normal.Z));

                var distance = p.GetDistanceToPoint(boundsCenter);
                var radius = Vector3f.Dot(boundsExtent, n);

                if (distance + radius < 0)
                {
                    return false;
                }
            }

            return true;
        }
    }
}