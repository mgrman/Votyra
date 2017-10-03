
using UnityEngine;
using Votyra.Core.Models;

namespace Votyra.Core.Utils
{
    public static class RectUtils
    {
        public static readonly Rect All = new Rect(float.MinValue / 2, float.MinValue / 2, float.MaxValue, float.MaxValue);
        public static Rect GetRectangle(this Rect rect, Vector2i cellCount, Vector2i cell)
        {
            var step = rect.size.DivideBy(cellCount);
            var pos = rect.min + step * cell;

            return new Rect(pos, step);
        }

        public static Rect FromCenterAndSize(Vector2 center, Vector2 size)
        {
            return new Rect(center - size / 2, size);
        }

        public static Rect FromCenterAndSize(float centerX, float centerY, float sizeX, float sizeY)
        {
            return new Rect(new Vector2(centerX - sizeX / 2, centerY - sizeY / 2), new Vector2(sizeX, sizeY));
        }

        public static Rect FromMinAndSize(float minX, float minY, float sizeX, float sizeY)
        {
            return new Rect(new Vector2(minX, minY), new Vector2(sizeX, sizeY));
        }

        public static Rect2i RoundToInt(this Rect rec)
        {
            return new Rect2i(Mathf.RoundToInt(rec.x), Mathf.RoundToInt(rec.y), Mathf.RoundToInt(rec.width), Mathf.RoundToInt(rec.height));
        }

        public static Rect2i RoundToContain(this Rect rec)
        {
            return Rect2i.MinMaxRect(Mathf.FloorToInt(rec.xMin), Mathf.FloorToInt(rec.yMin), Mathf.CeilToInt(rec.xMax), Mathf.CeilToInt(rec.yMax));
        }

        public static Rect3i RoundToContain(this Bounds rec)
        {
            return Rect3i.MinMaxRect(Mathf.FloorToInt(rec.min.x), Mathf.FloorToInt(rec.min.y), Mathf.FloorToInt(rec.min.z), Mathf.CeilToInt(rec.max.x), Mathf.CeilToInt(rec.max.y), Mathf.CeilToInt(rec.max.z));
        }


        public static Rect CombineWith(this Rect a, Rect b)
        {
            var aMin = a.min;
            var aMax = a.max;
            var bMin = b.min;
            var bMax = b.max;
            return Rect
                     .MinMaxRect(
                         Mathf.Min(aMin.x, bMin.x),
                         Mathf.Min(aMin.y, bMin.y),
                         Mathf.Max(aMax.x, bMax.x),
                         Mathf.Max(aMax.y, bMax.y));
        }
    }
}
