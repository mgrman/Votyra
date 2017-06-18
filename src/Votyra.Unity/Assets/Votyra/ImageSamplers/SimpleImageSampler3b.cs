using Votyra.Models;
using Votyra.Utils;
using Votyra.Images;
using UnityEngine;
using System;

namespace Votyra.ImageSamplers
{
    public class SimpleImageSampler3b : IImageSampler3b
    {
        public Vector3 WorldToImage(Vector3 pos)
        {
            return pos;
        }
        public Vector3 ImageToWorld(Vector3i pos)
        {
            return pos.ToVector3();
        }


        public Vector3i CellToX0Y0Z0(Vector3i pos)
        {
            return new Vector3i(pos.x + 0, pos.y + 0, pos.z + 0);
        }
        public Vector3i CellToX0Y0Z1(Vector3i pos)
        {
            return new Vector3i(pos.x + 0, pos.y + 0, pos.z + 1);
        }

        public Vector3i CellToX0Y1Z0(Vector3i pos)
        {
            return new Vector3i(pos.x + 0, pos.y + 1, pos.z + 0);
        }

        public Vector3i CellToX0Y1Z1(Vector3i pos)
        {
            return new Vector3i(pos.x + 0, pos.y + 1, pos.z + 1);
        }

        public Vector3i CellToX1Y0Z0(Vector3i pos)
        {
            return new Vector3i(pos.x + 1, pos.y + 0, pos.z + 0);
        }

        public Vector3i CellToX1Y0Z1(Vector3i pos)
        {
            return new Vector3i(pos.x + 1, pos.y + 0, pos.z + 1);
        }

        public Vector3i CellToX1Y1Z0(Vector3i pos)
        {
            return new Vector3i(pos.x + 1, pos.y + 1, pos.z + 0);
        }

        public Vector3i CellToX1Y1Z1(Vector3i pos)
        {
            return new Vector3i(pos.x + 1, pos.y + 1, pos.z + 1);
        }

        public SampledData3b Sample(IImage3b image, Vector3i offset)
        {
            return new SampledData3b(image.Sample(new Vector3i(offset.x + 0, offset.y + 0, offset.z + 0)),
                image.Sample(new Vector3i(offset.x + 0, offset.y + 0, offset.z + 1)),
                image.Sample(new Vector3i(offset.x + 0, offset.y + 1, offset.z + 0)),
                image.Sample(new Vector3i(offset.x + 0, offset.y + 1, offset.z + 1)),
                image.Sample(new Vector3i(offset.x + 1, offset.y + 0, offset.z + 0)),
                image.Sample(new Vector3i(offset.x + 1, offset.y + 0, offset.z + 1)),
                image.Sample(new Vector3i(offset.x + 1, offset.y + 1, offset.z + 0)),
                image.Sample(new Vector3i(offset.x + 1, offset.y + 1, offset.z + 1)));
        }
    }
}