using System;
using System.Collections.Generic;

namespace Votyra.Core.Models
{
    
    
    
    
    
        
        
    
    
    
    public partial struct Plane2f
    {
        public readonly Vector2f Normal;

        public readonly float Distance;

        public Plane2f(Vector2f inNormal, float d)
        {
            Normal = inNormal.Normalized();
            Distance = d;
        }

        public float GetDistanceToPoint(Vector2f inPt) => Vector2fUtils.Dot(Normal, inPt) + Distance;
    }

    public static class Plane2fExtensions
    {
        public static bool TestPlanesAABB(this IReadOnlyList<Plane2f> planes, Area2f bounds)
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
                var radius = Vector2fUtils.Dot(boundsExtent, n);

                if (distance + radius < 0)
                    return false;

                planesCount++;
            }

            return true;
        }
    }
}