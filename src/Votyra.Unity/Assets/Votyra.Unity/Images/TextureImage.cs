using System;
using Votyra.Common.Models;
using UnityEngine;
using Votyra.Common.Utils;

namespace Votyra.Images
{
    public class TextureImage : IImage2i
    {
        public Texture2D Texture { get; private set; }
        public Bounds Bounds { get; private set; }

        public TextureImage(Bounds bounds, Texture2D texture)
        {
            Bounds = bounds;
            RangeZ = new Range2i((int)bounds.min.z, (int)bounds.max.z);
            Texture = texture;
        }

        public Range2i RangeZ { get; private set; }

        public Rect InvalidatedArea => RectUtils.All;

        public int Sample(Vector2i point)
        {
            //float xNorm = (float)point.x / Texture.width;
            //float x = xNorm * Bounds.extents.x - Bounds.min.x;
            //float yNorm = (float)point.y / Texture.height;
            //float y = yNorm * Bounds.extents.y - Bounds.min.y;
            float xNorm = (point.x - Bounds.min.x) / Bounds.size.x;
            float x = xNorm;
            float yNorm = (point.y - Bounds.min.y) / Bounds.size.y;
            float y = yNorm;

            float zNorm = Texture.GetPixelBilinear(x, y).grayscale;
            return (int)(zNorm * Bounds.size.z + Bounds.min.z);
        }
    }
}