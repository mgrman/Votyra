using Votyra.Core.Models;

namespace Votyra.Core.Images
{
    public class CombineImage2i : IImage2i
    {
        public CombineImage2i(IImage2i imageA, IImage2i imageB, Operations operation)
        {
            ImageA = imageA;
            ImageB = imageB;
            Operation = operation;
            switch (Operation)
            {
                case Operations.Add:
                    RangeZ = new Range1i(imageA.RangeZ.Min + imageB.RangeZ.Min, imageA.RangeZ.Max + imageB.RangeZ.Max);
                    break;

                case Operations.Subtract:
                    RangeZ = new Range1i(imageA.RangeZ.Min - imageB.RangeZ.Min, imageA.RangeZ.Max - imageB.RangeZ.Max);
                    break;

                case Operations.Multiply:
                    RangeZ = new Range1i(imageA.RangeZ.Min * imageB.RangeZ.Min, imageA.RangeZ.Max * imageB.RangeZ.Max);
                    break;

                case Operations.Divide:
                    RangeZ = new Range1i(imageA.RangeZ.Min / imageB.RangeZ.Min, imageA.RangeZ.Max / imageB.RangeZ.Max);
                    break;

                default:
                    RangeZ = new Range1i(0, 0);
                    break;
            }
        }

        public enum Operations
        {
            Add,
            Subtract,
            Multiply,
            Divide
        }

        public IImage2i ImageA { get; private set; }
        public IImage2i ImageB { get; private set; }
        public Operations Operation { get; private set; }
        public Range1i RangeZ { get; private set; }

        public bool AnyData(Range2i range)
        {
            return ImageA.AnyData(range) || ImageB.AnyData(range);
        }

        public int? Sample(Vector2i point)
        {
            int? a = ImageA.Sample(point);
            int? b = ImageB.Sample(point);
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