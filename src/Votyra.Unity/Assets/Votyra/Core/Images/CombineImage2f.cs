using System;
using Votyra.Models;
using UnityEngine;
using Votyra.Utils;

namespace Votyra.Images
{
    public class CombineImage2f : IImage2f
    {
        public IImage2f ImageA { get; private set; }
        public IImage2f ImageB { get; private set; }
        public Operations Operation { get; private set; }

        public enum Operations
        {
            Add,
            Subtract,
            Multiply,
            Divide
        }

        public CombineImage2f(IImage2f imageA, IImage2f imageB, Operations operation)
        {
            ImageA = imageA;
            ImageB = imageB;
            Operation = operation;
            switch (Operation)
            {
                case Operations.Add:
                    RangeZ = new Range2(imageA.RangeZ.min + imageB.RangeZ.min, imageA.RangeZ.max + imageB.RangeZ.max);
                    break;
                case Operations.Subtract:
                    RangeZ = new Range2(imageA.RangeZ.min - imageB.RangeZ.min, imageA.RangeZ.max - imageB.RangeZ.max);
                    break;
                case Operations.Multiply:
                    RangeZ = new Range2(imageA.RangeZ.min * imageB.RangeZ.min, imageA.RangeZ.max * imageB.RangeZ.max);
                    break;
                case Operations.Divide:
                    RangeZ = new Range2(imageA.RangeZ.min / imageB.RangeZ.min, imageA.RangeZ.max / imageB.RangeZ.max);
                    break;
                default:
                    RangeZ = new Range2(0, 0);
                    break;
            }
        }
        public Range2 RangeZ { get; private set; }

        public float Sample(Vector2i point)
        {
            float a = ImageA.Sample(point);
            float b = ImageB.Sample(point);
            switch (Operation)
            {
                case Operations.Add:
                    return a + b;
                case Operations.Subtract:
                    return a - b;
                case Operations.Multiply:
                    return a * b;
                case Operations.Divide:
                    return a / b;
                default:
                    return 0;
            }
        }
    }
}