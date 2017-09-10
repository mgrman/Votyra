using System;
using Votyra.Models;
using UnityEngine;
using Votyra.Utils;

namespace Votyra.Images
{
    public class UmbraImage3f : IImage3f
    {
        public IImage2f Image { get; private set; }


        public UmbraImage3f(IImage2f imageA)
        {
            Image = imageA;
        }

        public float Sample(Vector3i point)
        {
            return Image.Sample(new Vector2i(point.x, point.y)) - point.z;
        }
    }
}