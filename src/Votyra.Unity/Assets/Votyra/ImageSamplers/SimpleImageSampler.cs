using System;
using Votyra.Models;
using Votyra.Utils;
using Votyra.Images;
using UnityEngine;

namespace Votyra.ImageSamplers
{
    public class SimpleImageSampler : IImageSampler
    {
        public Vector2 WorldToImage(Vector2 pos)
        {
            return pos;
        }
        public Vector2 ImageToWorld(Vector2i pos)
        {
            return pos.ToVector2();
        }


        public Vector2i CellToX0Y0(Vector2i pos)
        {
            return new Vector2i(pos.x + 0, pos.y + 0);
        }

        public Vector2i CellToX0Y1(Vector2i pos)
        {
            return new Vector2i(pos.x + 0, pos.y + 2);
        }

        public Vector2i CellToX1Y0(Vector2i pos)
        {
            return new Vector2i(pos.x + 2, pos.y + 0);
        }

        public Vector2i CellToX1Y1(Vector2i pos)
        {
            return new Vector2i(pos.x + 2, pos.y + 2);
        }
        public HeightData Sample(IImage2i image, Vector2i offset)
        {
            //offset = offset + offset;

            int x0y0 = image.Sample(offset);
            int x0y1 = image.Sample(new Vector2i(offset.x, offset.y + 2));
            int x1y0 = image.Sample(new Vector2i(offset.x + 2, offset.y));
            int x1y1 = image.Sample(new Vector2i(offset.x + 2, offset.y + 2));

            return new HeightData(x0y0, x0y1, x1y0, x1y1);
        }
    }
}