using System;
using TycoonTerrain.Common.Models;

namespace TycoonTerrain.Images
{
    public class RoundImage : IImage2i
    {
        public readonly IImage2 Image;

        public RoundImage(IImage2 image)
        {
            Image = image;
        }

        public bool IsAnimated
        {
            get
            {
                return (Image).IsAnimated;
            }
        }

        public Range2i RangeZ { get { return new Range2i((Image).RangeZ); } }

        public int Sample(Vector2i point, float time)
        {
            return (int)Math.Floor(Image.Sample(point.ToVector2(), time));
        }
    }
}