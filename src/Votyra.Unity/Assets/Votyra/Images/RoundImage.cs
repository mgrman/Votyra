using System;
using UnityEngine;
using Votyra.Common.Models;
using Votyra.Common.Utils;

namespace Votyra.Images
{
    public class RoundImage : IImage2i
    {
        public readonly IImage2 Image;

        public RoundImage(IImage2 image)
        {
            Image = image;
        }

        public Range2i RangeZ { get { return new Range2i((Image).RangeZ); } }

        public Rect2i InvalidatedArea => Image.InvalidatedArea.RoundToInt();

        public int Sample(Vector2i point)
        {
            return (int)Math.Floor(Image.Sample(point.ToVector2()));
        }
    }
}