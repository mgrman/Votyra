using Votyra.Common.Models;
using UnityEngine;

namespace Votyra.Common.Utils
{
    public static class BoundsUtils
    {
        public static Bounds GetBounds(this Bounds bounds, Vector2i cellCount, Vector2i cell)
        {
            var rect = RectUtils.FromMinAndSize(bounds.min.x, bounds.min.y, bounds.size.x, bounds.size.y);

            var step = rect.size.DivideBy(cellCount);
            var pos = rect.min + (step * cell);

            var center = new Vector3(pos.x + (step.x / 2), pos.y + (step.y / 2), bounds.center.z);
            var size = new Vector3(step.x, step.y, bounds.size.z);

            return new Bounds(center, size);
        }
    }
}