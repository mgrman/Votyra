using UnityEngine;

namespace Votyra.Core.Utils
{
    public static class TransformUtils
    {
        public static Bounds TransformBounds(this Transform self, Bounds bounds)
        {
            var center = self.TransformPoint(bounds.center);
            var points = bounds.GetCorners();

            var result = new Bounds(center, Vector3.zero);
            foreach (var point in points)
                result.Encapsulate(self.TransformPoint(point));
            return result;
        }

        public static Bounds InverseTransformBounds(this Transform self, Bounds bounds)
        {
            var center = self.InverseTransformPoint(bounds.center);
            var points = bounds.GetCorners();

            var result = new Bounds(center, Vector3.zero);
            foreach (var point in points)
                result.Encapsulate(self.InverseTransformPoint(point));
            return result;
        }
    }
}