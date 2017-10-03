using System;

using UnityEngine;
using Votyra.Core.Images;
using Votyra.Core.Models;

namespace Votyra.Core.Images
{
    public class TextureImage : IImage2f
    {
        public Texture2D Texture { get; private set; }
        public Bounds Bounds { get; private set; }

        public TextureImage(Bounds bounds, Texture2D texture)
        {
            Bounds = bounds;
            RangeZ = new Range2(bounds.min.z, bounds.max.z);
            Texture = texture;
        }

        public Range2 RangeZ { get; private set; }

        public Rect2i InvalidatedArea => Rect2i.All;

        public float Sample(Vector2i point)
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
            return zNorm * Bounds.size.z + Bounds.min.z;
        }
    }
}
