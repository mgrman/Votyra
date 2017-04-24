using System;
using TycoonTerrain.Common.Models;
using UnityEngine;

namespace TycoonTerrain.Images
{
    public class CombineImage : IImage2i
    {
        public IImage2i ImageA { get; private set; }
        public IImage2i ImageB { get; private set; }
        public Operations Operation { get; private set; }
        
        public enum Operations
        {
            Add,
            Subtract,
            Multiply,
            Divide
        }

        public CombineImage(IImage2i imageA, IImage2i imageB, Operations operation)
        {
            ImageA = imageA;
            ImageB = imageB;
            Operation = operation;
            RangeZ = ImageA.RangeZ + ImageB.RangeZ;
        }

        public bool IsAnimated
        {
            get
            {
                return ImageA.IsAnimated || ImageB.IsAnimated;
            }
        }

        public Range2i RangeZ { get; private set; }

        public int Sample(Vector2i point, float time)
        {
            int a = ImageA.Sample(point, time);
            int b=ImageB.Sample(point, time);
            switch (Operation)
            {
                case Operations.Add:
                    return a + b;
                case Operations.Subtract:
                    return a - b;
                case Operations.Multiply:
                    return a * b;
                case Operations.Divide:
                    return a/ b;
                default:
                    return 0;
            }
        }
    }
}