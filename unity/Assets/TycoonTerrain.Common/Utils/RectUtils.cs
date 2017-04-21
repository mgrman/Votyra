using System;
using TycoonTerrain.Common.Models;
using UnityEngine;

namespace TycoonTerrain.Common.Utils
{
    public static class RectUtils
    {
        public static Rect GetRectangle(this Rect rect,Vector2i cellCount, Vector2i cell)
        {
            var step = rect.size.DivideBy(cellCount);
            var pos = rect.min + step * cell;

            return new Rect(pos, step);
        }

        public static Rect FromCenterAndSize(Vector2 center, Vector2 size)
        {
            return new Rect(center-size/2, size);
        }

        public static Rect FromCenterAndSize(float centerX,float centerY, float sizeX,float sizeY)
        {
            return new Rect(new Vector2(centerX - sizeX / 2, centerY - sizeY / 2), new Vector2(sizeX, sizeY));
        }

        public static Rect FromMinAndSize(float minX, float minY, float sizeX, float sizeY)
        {
            return new Rect(new Vector2(minX, minY), new Vector2(sizeX, sizeY));
        }
    }
}