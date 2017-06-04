using Votyra.Common.Models;
using Votyra.Images;
using UnityEngine;

namespace Votyra.ImageSamplers
{
    public class DualImageSampler : IImageSampler
    {
        public Vector2 Transform(Vector2 pos)
        {
            return pos * 2;
        }
        public Vector2 InverseTransform(Vector2 pos)
        {
            return pos / 2;
        }

        public HeightData Sample(IImage2i image, Vector2i offset)
        {
            offset = offset + offset;

            int x0y0 = image.Sample(offset);
            int x0y1 = image.Sample(new Vector2i(offset.x, offset.y + 1));
            int x1y0 = image.Sample(new Vector2i(offset.x + 1, offset.y));
            int x1y1 = image.Sample(new Vector2i(offset.x + 1, offset.y + 1));

            return new HeightData(x0y0, x0y1, x1y0, x1y1);
        }
    }
}