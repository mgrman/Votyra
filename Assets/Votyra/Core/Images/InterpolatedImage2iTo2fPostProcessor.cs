using System;
using Votyra.Core.Models;

namespace Votyra.Core.Images
{
    public class InterpolatedImage2ITo2FPostProcessor : IImage2FPostProcessor
    {
        private readonly IInterpolationConfig interpolationConfig;

        public InterpolatedImage2ITo2FPostProcessor(IInterpolationConfig interpolationConfig)
        {
            this.interpolationConfig = interpolationConfig;
        }

        public IImage2F PostProcess(IImage2F image)
        {
            if (this.interpolationConfig.ImageSubdivision == 1)
            {
                return image;
            }

            switch (this.interpolationConfig.ActiveAlgorithm)
            {
                case IntepolationAlgorithm.NearestNeighbour:
                    if (image is IImageInvalidatableImage2)
                    {
                        return new NnImage2FInvalidatableWrapper(image, this.interpolationConfig.ImageSubdivision);
                    }
                    else
                    {
                        return new NnImage2FWrapper(image, this.interpolationConfig.ImageSubdivision);
                    }
                case IntepolationAlgorithm.Linear:
                    if (image is IImageInvalidatableImage2)
                    {
                        return new LinearImage2FInvalidatableWrapper(image, this.interpolationConfig.ImageSubdivision);
                    }
                    else
                    {
                        return new LinearImage2FWrapper(image, this.interpolationConfig.ImageSubdivision);
                    }
                case IntepolationAlgorithm.Cubic:
                    if (image is IImageInvalidatableImage2)
                    {
                        return new CubicImage2FInvalidatableWrapper(image, this.interpolationConfig.ImageSubdivision);
                    }
                    else
                    {
                        return new CubicImage2FWrapper(image, this.interpolationConfig.ImageSubdivision);
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private class CubicImage2FWrapper : IImage2F, IInitializableImage
        {
            private readonly IImage2F image;

            private readonly int subdivision;
            private readonly Vector2i interpolationCell = new Vector2i(int.MinValue, int.MinValue);
            private readonly float[,] interpolationMatrix = new float[4, 4];

            public CubicImage2FWrapper(IImage2F image, int subdivision)
            {
                this.image = image;
                this.subdivision = subdivision;
                this.RangeZ = this.image.RangeZ;
            }

            public Area1f RangeZ { get; }

            public float Sample(Vector2i point)
            {
                var minPoint = point / this.subdivision;
                var fraction = (point - (minPoint * this.subdivision)) / (float)this.subdivision;
                if (minPoint != this.interpolationCell)
                {
                    this.interpolationMatrix[0, 0] = this.image.Sample(minPoint + new Vector2i(+0 - 1, +0 - 1));
                    this.interpolationMatrix[0, 1] = this.image.Sample(minPoint + new Vector2i(+0 - 1, +1 - 1));
                    this.interpolationMatrix[0, 2] = this.image.Sample(minPoint + new Vector2i(+0 - 1, +2 - 1));
                    this.interpolationMatrix[0, 3] = this.image.Sample(minPoint + new Vector2i(+0 - 1, +3 - 1));
                    this.interpolationMatrix[1, 0] = this.image.Sample(minPoint + new Vector2i(+1 - 1, +0 - 1));
                    this.interpolationMatrix[1, 1] = this.image.Sample(minPoint + new Vector2i(+1 - 1, +1 - 1));
                    this.interpolationMatrix[1, 2] = this.image.Sample(minPoint + new Vector2i(+1 - 1, +2 - 1));
                    this.interpolationMatrix[1, 3] = this.image.Sample(minPoint + new Vector2i(+1 - 1, +3 - 1));
                    this.interpolationMatrix[2, 0] = this.image.Sample(minPoint + new Vector2i(+2 - 1, +0 - 1));
                    this.interpolationMatrix[2, 1] = this.image.Sample(minPoint + new Vector2i(+2 - 1, +1 - 1));
                    this.interpolationMatrix[2, 2] = this.image.Sample(minPoint + new Vector2i(+2 - 1, +2 - 1));
                    this.interpolationMatrix[2, 3] = this.image.Sample(minPoint + new Vector2i(+2 - 1, +3 - 1));
                    this.interpolationMatrix[3, 0] = this.image.Sample(minPoint + new Vector2i(+3 - 1, +0 - 1));
                    this.interpolationMatrix[3, 1] = this.image.Sample(minPoint + new Vector2i(+3 - 1, +1 - 1));
                    this.interpolationMatrix[3, 2] = this.image.Sample(minPoint + new Vector2i(+3 - 1, +2 - 1));
                    this.interpolationMatrix[3, 3] = this.image.Sample(minPoint + new Vector2i(+3 - 1, +3 - 1));
                }

                var col0 = this.Intepolate(this.interpolationMatrix[0, 0], this.interpolationMatrix[1, 0], this.interpolationMatrix[2, 0], this.interpolationMatrix[3, 0], fraction.X);
                var col1 = this.Intepolate(this.interpolationMatrix[0, 1], this.interpolationMatrix[1, 1], this.interpolationMatrix[2, 1], this.interpolationMatrix[3, 1], fraction.X);
                var col2 = this.Intepolate(this.interpolationMatrix[0, 2], this.interpolationMatrix[1, 2], this.interpolationMatrix[2, 2], this.interpolationMatrix[3, 2], fraction.X);
                var col3 = this.Intepolate(this.interpolationMatrix[0, 3], this.interpolationMatrix[1, 3], this.interpolationMatrix[2, 3], this.interpolationMatrix[3, 3], fraction.X);
                var value = this.Intepolate(col0, col1, col2, col3, fraction.Y);
                return value;
            }

            public PoolableMatrix2<float> SampleArea(Range2i area)
            {
                var min = this.ClipMinPoint(area.Min);
                var max = this.ClipMinPoint(area.Max);

                var offset = area.Min - (min * this.subdivision);

                var matrix = PoolableMatrix2<float>.CreateDirty(area.Size);
                var rawMatrix = matrix.RawMatrix;
                var tempQualifier = Area2i.FromMinAndMax(min, max);

                var min1 = tempQualifier.Min;
                for (var ix1 = 0; ix1 <= tempQualifier.Size.X; ix1++)
                {
                    for (var iy1 = 0; iy1 <= tempQualifier.Size.Y; iy1++)
                    {
                        var minPoint = new Vector2i(ix1, iy1) + min1;
                        for (var ix = 0; ix < this.subdivision; ix++)
                        {
                            for (var iy = 0; iy < this.subdivision; iy++)
                            {
                                var imagePos = new Vector2i(ix + (minPoint.X * this.subdivision), iy + (minPoint.Y * this.subdivision));
                                var matPos = imagePos - area.Min;
                                rawMatrix.TrySet(matPos, this.Sample(imagePos));
                            }
                        }
                    }
                }

                return matrix;
            }

            public void StartUsing()
            {
                (this.image as IInitializableImage)?.StartUsing();
            }

            public void FinishUsing()
            {
                (this.image as IInitializableImage)?.FinishUsing();
            }

            private Vector2i ClipMinPoint(Vector2i point) => new Vector2i(this.ClipMinPoint(point.X), this.ClipMinPoint(point.Y));

            private int ClipMinPoint(int point)
            {
                if (point > 0)
                {
                    return point / this.subdivision;
                }

                if (point < 0)
                {
                    return (point / this.subdivision) + ((point % this.subdivision) == 0 ? 0 : -1);
                }

                return 0;
            }

            // Monotone cubic interpolation
            // https://en.wikipedia.org/wiki/Monotone_cubic_interpolation
            private float Intepolate(float y0, float y1, float y2, float y3, float x12Rel)
            {
                // Get consecutive differences and slopes
                var dys0 = y1 - y0;
                var dys1 = y2 - y1;
                var dys2 = y3 - y2;

                // Get degree-1 coefficients
                float c1S1;
                if ((dys0 * dys1) <= 0)
                {
                    c1S1 = 0;
                }
                else
                {
                    c1S1 = 6f / ((3f / dys0) + (3f / dys1));
                }

                float c1S2;
                if ((dys1 * dys2) <= 0)
                {
                    c1S2 = 0;
                }
                else
                {
                    c1S2 = 6f / ((3f / dys1) + (3f / dys2));
                }

                // Get degree-2 and degree-3 coefficients
                var c3S1 = (c1S1 + c1S2) - dys1 - dys1;
                var c2S1 = dys1 - c1S1 - c3S1;

                // Interpolate
                var diff = x12Rel;
                var diffSq = diff * diff;
                return y1 + (c1S1 * diff) + (c2S1 * diffSq) + (c3S1 * diff * diffSq);
            }
        }

        private class CubicImage2FInvalidatableWrapper : CubicImage2FWrapper, IImageInvalidatableImage2
        {
            public CubicImage2FInvalidatableWrapper(IImage2F image, int subdivision)
                : base(image, subdivision)
            {
                var invalidatedArea = (image as IImageInvalidatableImage2).InvalidatedArea;
                this.InvalidatedArea = Range2i.FromMinAndMax(invalidatedArea.Min * subdivision, invalidatedArea.Max * subdivision);
            }

            public Range2i InvalidatedArea { get; }
        }

        private class LinearImage2FWrapper : IImage2F, IInitializableImage
        {
            private readonly IImage2F image;
            private readonly int subdivision;

            public LinearImage2FWrapper(IImage2F image, int subdivision)
            {
                this.image = image;
                this.subdivision = subdivision;
                this.RangeZ = this.image.RangeZ;
            }

            public Area1f RangeZ { get; }

            public float Sample(Vector2i point)
            {
                var minPoint = point / this.subdivision;
                var fraction = (point - (minPoint * this.subdivision)) / (float)this.subdivision;

                var x0Y0 = this.image.Sample(minPoint);
                var x0Y1 = this.image.Sample(minPoint + new Vector2i(0, 1));
                var x1Y0 = this.image.Sample(minPoint + new Vector2i(1, 0));
                var x1Y1 = this.image.Sample(minPoint + new Vector2i(1, 1));

                var value = ((1f - fraction.X) * (1f - fraction.Y) * x0Y0) + (fraction.X * (1f - fraction.Y) * x1Y0) + ((1f - fraction.X) * fraction.Y * x0Y1) + (fraction.X * fraction.Y * x1Y1);

                return value;
            }

            public PoolableMatrix2<float> SampleArea(Range2i area)
            {
                var min = this.ClipMinPoint(area.Min);
                var max = this.ClipMinPoint(area.Max);

                var offset = area.Min - (min * this.subdivision);

                var matrix = PoolableMatrix2<float>.CreateDirty(area.Size);
                var rawMatrix = matrix.RawMatrix;
                var tempQualifier = Area2i.FromMinAndMax(min, max);
                var min1 = tempQualifier.Min;
                for (var ix1 = 0; ix1 <= tempQualifier.Size.X; ix1++)
                {
                    for (var iy1 = 0; iy1 <= tempQualifier.Size.Y; iy1++)
                    {
                        var minPoint = new Vector2i(ix1, iy1) + min1;
                        for (var ix = 0; ix < this.subdivision; ix++)
                        {
                            for (var iy = 0; iy < this.subdivision; iy++)
                            {
                                var imagePos = new Vector2i(ix + (minPoint.X * this.subdivision), iy + (minPoint.Y * this.subdivision));
                                var matPos = imagePos - area.Min;
                                rawMatrix.TrySet(matPos, this.Sample(imagePos));
                            }
                        }
                    }
                }

                return matrix;
            }

            public void StartUsing()
            {
                (this.image as IInitializableImage)?.StartUsing();
            }

            public void FinishUsing()
            {
                (this.image as IInitializableImage)?.FinishUsing();
            }

            private Vector2i ClipMinPoint(Vector2i point) => new Vector2i(this.ClipMinPoint(point.X), this.ClipMinPoint(point.Y));

            private int ClipMinPoint(int point)
            {
                if (point > 0)
                {
                    return point / this.subdivision;
                }

                if (point < 0)
                {
                    return (point / this.subdivision) + ((point % this.subdivision) == 0 ? 0 : -1);
                }

                return 0;
            }
        }

        private class LinearImage2FInvalidatableWrapper : LinearImage2FWrapper, IImageInvalidatableImage2
        {
            public LinearImage2FInvalidatableWrapper(IImage2F image, int subdivision)
                : base(image, subdivision)
            {
                var invalidatedArea = (image as IImageInvalidatableImage2).InvalidatedArea;
                this.InvalidatedArea = Range2i.FromMinAndMax(invalidatedArea.Min * subdivision, invalidatedArea.Max * subdivision);
            }

            public Range2i InvalidatedArea { get; }
        }

        private class NnImage2FWrapper : IImage2F, IInitializableImage
        {
            private readonly IImage2F image;
            private readonly int subdivision;

            public NnImage2FWrapper(IImage2F image, int subdivision)
            {
                this.image = image;
                this.subdivision = subdivision;
                this.RangeZ = this.image.RangeZ;
            }

            public Area1f RangeZ { get; }

            public float Sample(Vector2i point)
            {
                var minPoint = point / this.subdivision;

                var x0Y0 = this.image.Sample(minPoint);
                return x0Y0;
            }

            public PoolableMatrix2<float> SampleArea(Range2i area)
            {
                var min = this.ClipMinPoint(area.Min);
                var max = this.ClipMinPoint(area.Max);

                var offset = area.Min - (min * this.subdivision);

                var matrix = PoolableMatrix2<float>.CreateDirty(area.Size);
                var rawMatrix = matrix.RawMatrix;
                var tempQualifier = Area2i.FromMinAndMax(min, max);

                var min1 = tempQualifier.Min;
                for (var ix1 = 0; ix1 <= tempQualifier.Size.X; ix1++)
                {
                    for (var iy1 = 0; iy1 <= tempQualifier.Size.Y; iy1++)
                    {
                        var minPoint = new Vector2i(ix1, iy1) + min1;
                        for (var ix = 0; ix < this.subdivision; ix++)
                        {
                            for (var iy = 0; iy < this.subdivision; iy++)
                            {
                                var imagePos = new Vector2i(ix + (minPoint.X * this.subdivision), iy + (minPoint.Y * this.subdivision));
                                var matPos = imagePos - area.Min;
                                rawMatrix.TrySet(matPos, this.Sample(imagePos));
                            }
                        }
                    }
                }

                return matrix;
            }

            public void StartUsing()
            {
                (this.image as IInitializableImage)?.StartUsing();
            }

            public void FinishUsing()
            {
                (this.image as IInitializableImage)?.FinishUsing();
            }

            private Vector2i ClipMinPoint(Vector2i point) => new Vector2i(this.ClipMinPoint(point.X), this.ClipMinPoint(point.Y));

            private int ClipMinPoint(int point)
            {
                if (point > 0)
                {
                    return point / this.subdivision;
                }

                if (point < 0)
                {
                    return (point / this.subdivision) + ((point % this.subdivision) == 0 ? 0 : -1);
                }

                return 0;
            }
        }

        private class NnImage2FInvalidatableWrapper : NnImage2FWrapper, IImageInvalidatableImage2
        {
            public NnImage2FInvalidatableWrapper(IImage2F image, int subdivision)
                : base(image, subdivision)
            {
                var invalidatedArea = (image as IImageInvalidatableImage2).InvalidatedArea;
                this.InvalidatedArea = Range2i.FromMinAndMax(invalidatedArea.Min * subdivision, invalidatedArea.Max * subdivision);
            }

            public Range2i InvalidatedArea { get; }
        }
    }
}