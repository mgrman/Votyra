﻿using Votyra.Common.Models;
using UnityEngine;

namespace Votyra.Common.Utils
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

        public static Rect? TryCombineWith(this Rect? a, Rect? b)
        {
            if (a == null)
            {
                return b;
            }
            if (b == null)
            {
                return a;
            }
            return a.Value.CombineWith(b.Value);
        }

        public static Rect? TryCombineWith(this Rect? a, Rect b)
        {
            if (a == null)
            {
                return b;
            }
            return a.Value.CombineWith(b);
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