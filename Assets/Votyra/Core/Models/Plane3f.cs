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
            this.Normal = inNormal.Normalized;
            this.Distance = d;
        }

        public float GetDistanceToPoint(Vector3f inPt)
        {
            return Vector3f.Dot(this.Normal, inPt) + this.Distance;
        }
    }

    public static class Plane3fExtensions
    {
        public static bool TestPlanesAABB(this IEnumerable<Plane3f> planes, Area3f bounds)
        {
            Vector3f boundsCenter = bounds.Center;  // center of bounds
            Vector3f boundsExtent = bounds.Extents; // half diagonal
                                                    // do intersection test for each active frame

            // while active frames
            int planesCount = 0;
            foreach (var p in planes)
            {
                var n = new Vector3f(Math.Abs(p.Normal.X), Math.Abs(p.Normal.Y), Math.Abs(p.Normal.Z));

                float distance = p.GetDistanceToPoint(boundsCenter);
                float radius = Vector3f.Dot(boundsExtent, n);

                if (distance + radius < 0)
                {
                    // behind clip plane
                    return false;
                }

                planesCount++;
            }

            return true;
        }
    }
}