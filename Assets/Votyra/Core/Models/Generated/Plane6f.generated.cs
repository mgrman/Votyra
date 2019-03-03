using System;
using System.Collections.Generic;

namespace Votyra.Core.Models
{
    
    
    
    
    
        
        
    
    
    
    public partial struct Plane6f
    {
        public readonly Vector6f Normal;

        public readonly float Distance;

        public Plane6f(Vector6f inNormal, float d)
        {
            Normal = inNormal.Normalized();
            Distance = d;
        }

        public float GetDistanceToPoint(Vector6f inPt) => Vector6fUtils.Dot(Normal, inPt) + Distance;
    }

    public static class Plane6fExtensions
    {
        public static bool TestPlanesAABB(this IReadOnlyList<Plane6f> planes, Area6f bounds)
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
                var radius = Vector6fUtils.Dot(boundsExtent, n);

                if (distance + radius < 0)
                    return false;

                planesCount++;
            }

            return true;
        }
    }
}