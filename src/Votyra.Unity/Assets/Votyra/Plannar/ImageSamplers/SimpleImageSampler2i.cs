using UnityEngine;
using Votyra.Core.Models;

namespace Votyra.Plannar.ImageSamplers
{
    public class SimpleImageSampler2i : IImageSampler2i
    {
        private const int StepSize = 1;
        private const float StepSizeFloat = StepSize;

        public Vector2 WorldToImage(Vector2 pos)
        {
            return pos * StepSize;
        }

        public Vector2 ImageToWorld(Vector2i pos)
        {
            return pos / StepSizeFloat;
        }

        public Vector2i CellToX0Y0(Vector2i pos)
        {
            return new Vector2i(pos.x, pos.y) * StepSize;
        }

        public Vector2i CellToX0Y1(Vector2i pos)
        {
            return new Vector2i(pos.x, pos.y + 1) * StepSize;
        }

        public Vector2i CellToX1Y0(Vector2i pos)
        {
            return new Vector2i(pos.x + 1, pos.y) * StepSize;
        }

        public Vector2i CellToX1Y1(Vector2i pos)
        {
            return new Vector2i(pos.x + 1, pos.y + 1) * StepSize;
        }
    }
}