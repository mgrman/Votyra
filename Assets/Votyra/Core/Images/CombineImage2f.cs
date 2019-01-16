using System;
using Votyra.Core.Models;

namespace Votyra.Core.Images
{
    public class CombineImage2f : IImage2f
    {
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
                    RangeZ = new Area1f(imageA.RangeZ.Min + imageB.RangeZ.Min, imageA.RangeZ.Max + imageB.RangeZ.Max);
                    break;

                case Operations.Subtract:
                    RangeZ = new Area1f(imageA.RangeZ.Min - imageB.RangeZ.Min, imageA.RangeZ.Max - imageB.RangeZ.Max);
                    break;

                case Operations.Multiply:
                    RangeZ = new Area1f(imageA.RangeZ.Min * imageB.RangeZ.Min, imageA.RangeZ.Max * imageB.RangeZ.Max);
                    break;

                case Operations.Divide:
                    RangeZ = new Area1f(imageA.RangeZ.Min / imageB.RangeZ.Min, imageA.RangeZ.Max / imageB.RangeZ.Max);
                    break;

                default:
                    RangeZ = Area1f.Zero;
                    break;
            }
        }

        public IImage2f ImageA { get; }
        public IImage2f ImageB { get; }
        public Operations Operation { get; }
        public Area1f RangeZ { get; }

        public float Sample(Vector2i point)
        {
            var a = ImageA.Sample(point);
            var b = ImageB.Sample(point);
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

        public IPoolableMatrix2<float> SampleArea(Range2i area)
        {
            var min = area.Min;

            var matrix = PoolableMatrix<float>.CreateDirty(area.Size);
     
            for (var ix = 0; ix < matrix.Size.X; ix++)
            {
                for (var iy = 0; iy < matrix.Size.Y; iy++)
                {
                    var matPoint=new Vector2i(ix, iy);
                    matrix[matPoint] = Sample(matPoint + min);
                }
            }

            return matrix;
        }
    }
}