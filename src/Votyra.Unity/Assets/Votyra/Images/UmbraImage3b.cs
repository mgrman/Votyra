using System;
using Votyra.Models;
using UnityEngine;
using Votyra.Utils;

namespace Votyra.Images
{
    public class UmbraImage3b : IImage3b
    {
        public IImage2i ImageA { get; private set; }


        public UmbraImage3b(IImage2i imageA)
        {
            ImageA = imageA;
        }

        public bool Sample(Vector3i point)
        {
            return ImageA.Sample(new Vector2i(point.x, point.y)) > point.z;
        }
    }
}