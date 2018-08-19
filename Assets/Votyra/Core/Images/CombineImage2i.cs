using System;
using Votyra.Core.Models;

namespace Votyra.Core.Images
{
    public class CombineImage2i : IImage2i
    {
        public IImage2i ImageA { get; private set; }
        public IImage2i ImageB { get; private set; }
        public Operations Operation { get; private set; }

        public enum Operations
        {
            // Add,
            // Subtract,
            // Multiply,
            // Divide
        }

        public CombineImage2i(IImage2i imageA, IImage2i imageB, Operations operation)
        {
            ImageA = imageA;
            ImageB = imageB;
            Operation = operation;
            switch (Operation)
            {
                // case Operations.Add:
                //     RangeZ = new Range1h(imageA.RangeZ.Min + imageB.RangeZ.Min, imageA.RangeZ.Max + imageB.RangeZ.Max);
                //     break;

                // case Operations.Subtract:
                //     RangeZ = new Range1h(imageA.RangeZ.Min - imageB.RangeZ.Min, imageA.RangeZ.Max - imageB.RangeZ.Max);
                //     break;

                // case Operations.Multiply:
                //     RangeZ = new Range1i(imageA.RangeZ.Min * imageB.RangeZ.Min, imageA.RangeZ.Max * imageB.RangeZ.Max);
                //     break;

                // case Operations.Divide:
                //     RangeZ = new Range1i(imageA.RangeZ.Min / imageB.RangeZ.Min, imageA.RangeZ.Max / imageB.RangeZ.Max);
                //     break;

                default:
                    RangeZ = Range1h.Default;
                    break;
            }
        }

        public Range1h RangeZ { get; private set; }

        public Height Sample(Vector2i point)
        {
            Height a = ImageA.Sample(point);
            Height b = ImageB.Sample(point);
            throw new NotImplementedException();
            switch (Operation)
            {
                // case Operations.Add:
                //     return a + (b - Height.Default);

                // case Operations.Subtract:
                //     return a + (Height.Default - b);

                // case Operations.Multiply:
                //     return a * b;

                // case Operations.Divide:
                //     return a / b;

                default:
                    return Height.Default;
            }
        }

        public bool AnyData(Range2i range)
        {
            return ImageA.AnyData(range) || ImageB.AnyData(range);
        }
    }
}