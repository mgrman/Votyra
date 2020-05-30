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
            Divide,
        }

        public CombineImage2f(IImage2f imageA, IImage2f imageB, Operations operation)
        {
            this.ImageA = imageA;
            this.ImageB = imageB;
            this.Operation = operation;
            switch (this.Operation)
            {
                case Operations.Add:
                    this.RangeZ = Area1f.FromMinAndMax(imageA.RangeZ.Min + imageB.RangeZ.Min, imageA.RangeZ.Max + imageB.RangeZ.Max);
                    break;

                case Operations.Subtract:
                    this.RangeZ = Area1f.FromMinAndMax(imageA.RangeZ.Min - imageB.RangeZ.Min, imageA.RangeZ.Max - imageB.RangeZ.Max);
                    break;

                case Operations.Multiply:
                    this.RangeZ = Area1f.FromMinAndMax(imageA.RangeZ.Min * imageB.RangeZ.Min, imageA.RangeZ.Max * imageB.RangeZ.Max);
                    break;

                case Operations.Divide:
                    this.RangeZ = Area1f.FromMinAndMax(imageA.RangeZ.Min / imageB.RangeZ.Min, imageA.RangeZ.Max / imageB.RangeZ.Max);
                    break;

                default:
                    this.RangeZ = Area1f.Zero;
                    break;
            }
        }

        public IImage2f ImageA { get; }

        public IImage2f ImageB { get; }

        public Operations Operation { get; }

        public Area1f RangeZ { get; }

        public float Sample(Vector2i point)
        {
            var a = this.ImageA.Sample(point);
            var b = this.ImageB.Sample(point);
            switch (this.Operation)
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

        public PoolableMatrix2<float> SampleArea(Range2i area)
        {
            var min = area.Min;

            var matrix = PoolableMatrix2<float>.CreateDirty(area.Size);
            var rawMatrix = matrix.RawMatrix;

            for (var ix = 0; ix < rawMatrix.SizeX(); ix++)
            {
                for (var iy = 0; iy < rawMatrix.SizeY(); iy++)
                {
                    var matPoint = new Vector2i(ix, iy);
                    rawMatrix.Set(matPoint, this.Sample(matPoint + min));
                }
            }

            return matrix;
        }
    }
}
