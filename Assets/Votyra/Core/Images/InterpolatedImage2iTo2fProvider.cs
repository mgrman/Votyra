using System;
using System.Collections.Generic;
using System.Linq;
using Votyra.Core.Images.Constraints;
using Votyra.Core.Models;
using Zenject;

namespace Votyra.Core.Images
{
    public class InterpolatedImage2iTo2fProvider : IImage2fProvider
    {
        private readonly IImage2iProvider _provider;
        private readonly IInterpolationConfig _interpolationConfig;

        public InterpolatedImage2iTo2fProvider(IImage2iProvider provider, IInterpolationConfig interpolationConfig)
        {
            _provider = provider;
            _interpolationConfig = interpolationConfig;
        }

        public IImage2f CreateImage()
        {
            var image = _provider.CreateImage();

            if (_interpolationConfig.ImageSubdivision == 1)
            {
                if (image is IImageInvalidatableImage2i)
                {
                    return new Image2fInvalidatableWrapper(image);
                }
                else
                {
                    return new Image2fWrapper(image);
                }
            }

            switch (_interpolationConfig.ActiveAlgorithm)
            {
                case IntepolationAlgorithm.NearestNeighbour:
                    if (image is IImageInvalidatableImage2i)
                    {
                        return new NNImage2fInvalidatableWrapper(image, _interpolationConfig.ImageSubdivision);
                    }
                    else
                    {
                        return new NNImage2fWrapper(image, _interpolationConfig.ImageSubdivision);
                    }
                case IntepolationAlgorithm.Linear:
                    if (image is IImageInvalidatableImage2i)
                    {
                        return new LinearImage2fInvalidatableWrapper(image, _interpolationConfig.ImageSubdivision);
                    }
                    else
                    {
                        return new LinearImage2fWrapper(image, _interpolationConfig.ImageSubdivision);
                    }
                case IntepolationAlgorithm.Cubic:
                    if (image is IImageInvalidatableImage2i)
                    {
                        return new CubicImage2fInvalidatableWrapper(image, _interpolationConfig.ImageSubdivision);
                    }
                    else
                    {
                        return new CubicImage2fWrapper(image, _interpolationConfig.ImageSubdivision);
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private class Image2fWrapper : IImage2f
        {
            public Image2fWrapper(IImage2i image)
            {
                _image = image;
            }

            protected readonly IImage2i _image;

            public Range1hf RangeZ => _image.RangeZ.ToRange1hf();

            public Height1f Sample(Vector2i point)
            {
                return _image.Sample(point).ToHeight1f();
            }

            public IPoolableMatrix2<Height1f> SampleArea(Range2i area)
            {
                var min = area.Min;

                var matrix = PoolableMatrix<Height1f>.CreateDirty(area.Size);
                matrix.Size.ForeachPointExlusive(matPoint => { matrix[matPoint] = Sample(matPoint + min); });
                return matrix;
            }
        }

        private class Image2fInvalidatableWrapper : Image2fWrapper, IImageInvalidatableImage2i
        {
            public Image2fInvalidatableWrapper(IImage2i image) : base(image)
            {
            }


            public Range2i InvalidatedArea => (_image as IImageInvalidatableImage2i).InvalidatedArea;
        }

        private class CubicImage2fWrapper : IImage2f
        {
            protected readonly IImage2i _image;
            protected readonly int _subdivision;

            public CubicImage2fWrapper(IImage2i image, int subdivision)
            {
                _image = image;
                _subdivision = subdivision;
                RangeZ = _image.RangeZ.ToRange1hf();
            }

            public Range1hf RangeZ { get; }

            private readonly Vector2i _interpolationCell = new Vector2i(int.MinValue, int.MinValue);
            private readonly float[,] _interpolationMatrix = new float[4, 4];

            public Height1f Sample(Vector2i point)
            {
                var minPoint = point / _subdivision;
                var fraction = (point - minPoint * _subdivision) / (float) _subdivision;
                if (minPoint != _interpolationCell)
                {
                    _interpolationMatrix[0, 0] = _image.Sample(minPoint + new Vector2i(+0 - 1, +0 - 1)).RawValue;
                    _interpolationMatrix[0, 1] = _image.Sample(minPoint + new Vector2i(+0 - 1, +1 - 1)).RawValue;
                    _interpolationMatrix[0, 2] = _image.Sample(minPoint + new Vector2i(+0 - 1, +2 - 1)).RawValue;
                    _interpolationMatrix[0, 3] = _image.Sample(minPoint + new Vector2i(+0 - 1, +3 - 1)).RawValue;
                    _interpolationMatrix[1, 0] = _image.Sample(minPoint + new Vector2i(+1 - 1, +0 - 1)).RawValue;
                    _interpolationMatrix[1, 1] = _image.Sample(minPoint + new Vector2i(+1 - 1, +1 - 1)).RawValue;
                    _interpolationMatrix[1, 2] = _image.Sample(minPoint + new Vector2i(+1 - 1, +2 - 1)).RawValue;
                    _interpolationMatrix[1, 3] = _image.Sample(minPoint + new Vector2i(+1 - 1, +3 - 1)).RawValue;
                    _interpolationMatrix[2, 0] = _image.Sample(minPoint + new Vector2i(+2 - 1, +0 - 1)).RawValue;
                    _interpolationMatrix[2, 1] = _image.Sample(minPoint + new Vector2i(+2 - 1, +1 - 1)).RawValue;
                    _interpolationMatrix[2, 2] = _image.Sample(minPoint + new Vector2i(+2 - 1, +2 - 1)).RawValue;
                    _interpolationMatrix[2, 3] = _image.Sample(minPoint + new Vector2i(+2 - 1, +3 - 1)).RawValue;
                    _interpolationMatrix[3, 0] = _image.Sample(minPoint + new Vector2i(+3 - 1, +0 - 1)).RawValue;
                    _interpolationMatrix[3, 1] = _image.Sample(minPoint + new Vector2i(+3 - 1, +1 - 1)).RawValue;
                    _interpolationMatrix[3, 2] = _image.Sample(minPoint + new Vector2i(+3 - 1, +2 - 1)).RawValue;
                    _interpolationMatrix[3, 3] = _image.Sample(minPoint + new Vector2i(+3 - 1, +3 - 1)).RawValue;
                }

                float col0 = Intepolate(_interpolationMatrix[0, 0], _interpolationMatrix[1, 0], _interpolationMatrix[2, 0], _interpolationMatrix[3, 0], fraction.X);
                float col1 = Intepolate(_interpolationMatrix[0, 1], _interpolationMatrix[1, 1], _interpolationMatrix[2, 1], _interpolationMatrix[3, 1], fraction.X);
                float col2 = Intepolate(_interpolationMatrix[0, 2], _interpolationMatrix[1, 2], _interpolationMatrix[2, 2], _interpolationMatrix[3, 2], fraction.X);
                float col3 = Intepolate(_interpolationMatrix[0, 3], _interpolationMatrix[1, 3], _interpolationMatrix[2, 3], _interpolationMatrix[3, 3], fraction.X);
                float value = Intepolate(col0, col1, col2, col3, fraction.Y);
                return new Height1f(value);
            }

            public IPoolableMatrix2<Height1f> SampleArea(Range2i area)
            {
                var min = ClipMinPoint(area.Min);
                var max = ClipMinPoint(area.Max);

                var offset = area.Min - min * _subdivision;

                var matrix = PoolableMatrix<Height1f>.CreateDirty(area.Size);
                Area2i.FromMinAndMax(min, max).ForeachPointInclusive(minPoint =>
                {
                    for (int ix = 0; ix < _subdivision; ix++)
                    {
                        for (int iy = 0; iy < _subdivision; iy++)
                        {
                            var imagePos = new Vector2i(ix + minPoint.X * _subdivision, iy + minPoint.Y * _subdivision);
                            var matPos = imagePos - area.Min;
                            if (matrix.ContainsIndex(matPos))
                            {
                                matrix[matPos] = Sample(imagePos);
                            }
                        }
                    }
                });
                return matrix;
            }

            private Vector2i ClipMinPoint(Vector2i point)
            {
                return new Vector2i(ClipMinPoint(point.X), ClipMinPoint(point.Y));
            }

            private int ClipMinPoint(int point)
            {
                if (point > 0)
                    return point / _subdivision;
                else if (point < 0)
                    return point / _subdivision + (point % _subdivision == 0 ? 0 : -1);
                else
                    return 0;
            }

            // Monotone cubic interpolation
            // https://en.wikipedia.org/wiki/Monotone_cubic_interpolation
            private float Intepolate(float y0, float y1, float y2, float y3, float x12Rel)
            {
                // Get consecutive differences and slopes
                float dys0 = y1 - y0;
                float dys1 = y2 - y1;
                float dys2 = y3 - y2;

                // Get degree-1 coefficients
                float c1s1;
                if (dys0 * dys1 <= 0)
                {
                    c1s1 = 0;
                }
                else
                {
                    c1s1 = 6f / (3f / dys0 + 3f / dys1);
                }

                float c1s2;
                if (dys1 * dys2 <= 0)
                {
                    c1s2 = 0;
                }
                else
                {
                    c1s2 = 6f / (3f / dys1 + 3f / dys2);
                }

                // Get degree-2 and degree-3 coefficients
                float c3s1 = c1s1 + c1s2 - dys1 - dys1;
                float c2s1 = (dys1 - c1s1 - c3s1);

                // Interpolate
                var diff = x12Rel;
                var diffSq = diff * diff;
                return y1 + c1s1 * diff + c2s1 * diffSq + c3s1 * diff * diffSq;
            }
        }

        private class CubicImage2fInvalidatableWrapper : CubicImage2fWrapper, IImageInvalidatableImage2i
        {
            public CubicImage2fInvalidatableWrapper(IImage2i image, int subdivision) : base(image, subdivision)
            {
                var invalidatedArea = (image as IImageInvalidatableImage2i).InvalidatedArea;
                InvalidatedArea = Range2i.FromMinAndMax(invalidatedArea.Min * subdivision, invalidatedArea.Max * subdivision);
            }

            public Range2i InvalidatedArea { get; }
        }

        
        private class LinearImage2fWrapper : IImage2f
        {
            protected readonly IImage2i _image;
            protected readonly int _subdivision;
            
            public LinearImage2fWrapper(IImage2i image,int subdivision)
            {
                _image = image;
                _subdivision = subdivision;
                RangeZ = _image.RangeZ.ToRange1hf();
            }
        
            public Range1hf RangeZ { get; }
        
            public Height1f Sample(Vector2i point)
            {
                var minPoint = point / _subdivision;
                var fraction = (point - minPoint * _subdivision) / (float) _subdivision;
        
                var x0y0 = _image.Sample(minPoint);
                var x0y1 = _image.Sample(minPoint + new Vector2i(0, 1));
                var x1y0 = _image.Sample(minPoint + new Vector2i(1, 0));
                var x1y1 = _image.Sample(minPoint + new Vector2i(1, 1));
        
                var value = (1f - fraction.X) * (1f - fraction.Y) * x0y0.RawValue +
                            fraction.X * (1f - fraction.Y) * x1y0.RawValue +
                            (1f - fraction.X) * fraction.Y * x0y1.RawValue + fraction.X * fraction.Y * x1y1.RawValue;
        
                return new Height1f(value );
            }

            public IPoolableMatrix2<Height1f> SampleArea(Range2i area)
            {
                var min = ClipMinPoint(area.Min);
                var max = ClipMinPoint(area.Max);

                var offset = area.Min - min * _subdivision;

                var matrix = PoolableMatrix<Height1f>.CreateDirty(area.Size);
                Area2i.FromMinAndMax(min, max).ForeachPointInclusive(minPoint =>
                {
                    for (int ix = 0; ix < _subdivision; ix++)
                    {
                        for (int iy = 0; iy < _subdivision; iy++)
                        {
                            var imagePos = new Vector2i(ix + minPoint.X * _subdivision, iy + minPoint.Y * _subdivision);
                            var matPos = imagePos - area.Min;
                            if (matrix.ContainsIndex(matPos))
                            {
                                matrix[matPos] = Sample(imagePos);
                            }
                        }
                    }
                });
                return matrix;
            }

            private Vector2i ClipMinPoint(Vector2i point)
            {
                return new Vector2i(ClipMinPoint(point.X), ClipMinPoint(point.Y));
            }

            private int ClipMinPoint(int point)
            {
                if (point > 0)
                    return point / _subdivision;
                else if (point < 0)
                    return point / _subdivision + (point % _subdivision == 0 ? 0 : -1);
                else
                    return 0;
            }
        }
        
        private class LinearImage2fInvalidatableWrapper : LinearImage2fWrapper,IImageInvalidatableImage2i
        {
            public LinearImage2fInvalidatableWrapper(IImage2i image, int subdivision)
                :base(image,subdivision)
            {
                var invalidatedArea = (image as IImageInvalidatableImage2i).InvalidatedArea;
                InvalidatedArea = Range2i.FromMinAndMax(invalidatedArea.Min * subdivision, invalidatedArea.Max * subdivision);
            }
        
            public Range2i InvalidatedArea { get; }
        }
        
        private class NNImage2fWrapper : IImage2f
        {
            protected readonly IImage2i _image;
            protected readonly int _subdivision;
        
            public NNImage2fWrapper(IImage2i image, int subdivision)
            {
                _image = image;
                _subdivision = subdivision;
                RangeZ = _image.RangeZ.ToRange1hf();
            }
        
            public Range1hf RangeZ { get; }
        
            public Height1f Sample(Vector2i point)
            {
                var minPoint = point / _subdivision;
        
                var x0y0 = _image.Sample(minPoint);
                return x0y0.ToHeight1f();
            }

            public IPoolableMatrix2<Height1f> SampleArea(Range2i area)
            {
                var min = ClipMinPoint(area.Min);
                var max = ClipMinPoint(area.Max);

                var offset = area.Min - min * _subdivision;

                var matrix = PoolableMatrix<Height1f>.CreateDirty(area.Size);
                Area2i.FromMinAndMax(min, max).ForeachPointInclusive(minPoint =>
                {
                    for (int ix = 0; ix < _subdivision; ix++)
                    {
                        for (int iy = 0; iy < _subdivision; iy++)
                        {
                            var imagePos = new Vector2i(ix + minPoint.X * _subdivision, iy + minPoint.Y * _subdivision);
                            var matPos = imagePos - area.Min;
                            if (matrix.ContainsIndex(matPos))
                            {
                                matrix[matPos] = Sample(imagePos);
                            }
                        }
                    }
                });
                return matrix;
            }

            private Vector2i ClipMinPoint(Vector2i point)
            {
                return new Vector2i(ClipMinPoint(point.X), ClipMinPoint(point.Y));
            }

            private int ClipMinPoint(int point)
            {
                if (point > 0)
                    return point / _subdivision;
                else if (point < 0)
                    return point / _subdivision + (point % _subdivision == 0 ? 0 : -1);
                else
                    return 0;
            }
        }
        
        private class NNImage2fInvalidatableWrapper : NNImage2fWrapper, IImageInvalidatableImage2i
        {
            public NNImage2fInvalidatableWrapper(IImage2i image, int subdivision)
                : base(image, subdivision)
            {
                var invalidatedArea = (image as IImageInvalidatableImage2i).InvalidatedArea;
                InvalidatedArea =
                    Range2i.FromMinAndMax(invalidatedArea.Min * subdivision, invalidatedArea.Max * subdivision);
            }
        
            public Range2i InvalidatedArea { get; }
        }
    }
}