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
                    RangeZ = new Range1hi(imageA.RangeZ.Min + (imageB.RangeZ.Min - Height1i.Default), imageA.RangeZ.Max + (imageB.RangeZ.Max - Height1i.Default));
                    break;

                case Operations.Subtract:
                    RangeZ = new Range1hi(imageA.RangeZ.Min - (imageB.RangeZ.Min - Height1i.Default), imageA.RangeZ.Max - (imageB.RangeZ.Max - Height1i.Default));
                    break;

                // case Operations.Multiply:
                //     RangeZ = new Range1i(imageA.RangeZ.Min * imageB.RangeZ.Min, imageA.RangeZ.Max * imageB.RangeZ.Max);
                //     break;

                // case Operations.Divide:
                //     RangeZ = new Range1i(imageA.RangeZ.Min / imageB.RangeZ.Min, imageA.RangeZ.Max / imageB.RangeZ.Max);
                //     break;

                default:
                    RangeZ = Range1hi.Default;
                    break;
            }
        }

        public enum Operations
        {
            Add,
            Subtract,
            // Multiply,
            // Divide
        }

        public IImage2i ImageA { get; private set; }
        public IImage2i ImageB { get; private set; }
        public Operations Operation { get; private set; }
        public Range1hi RangeZ { get; private set; }

        public Height1i Sample(Vector2i point)
        {
            Height1i a = ImageA.Sample(point);
            Height1i b = ImageB.Sample(point);
            switch (Operation)
            {
                case Operations.Add:
                    return a + (b - Height1i.Default);

                case Operations.Subtract:
                    return a + (Height1i.Default - b);

                // case Operations.Multiply:
                //     return a * b;

                // case Operations.Divide:
                //     return a / b;

                default:
                    return Height1i.Default;
            }
        }
    }
}