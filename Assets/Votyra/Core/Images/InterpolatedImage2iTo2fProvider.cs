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

        public InterpolatedImage2iTo2fProvider(IImage2iProvider provider,IInterpolationConfig interpolationConfig)
        {
            _provider = provider;
            _interpolationConfig = interpolationConfig;
        }


        public IImage2f CreateImage()
        {
            var image = _provider.CreateImage();
            if (image is IImageInvalidatableImage2i)
            {
                return  new Image2fInvalidatableWrapper(image, _interpolationConfig.Subdivision);
            }
            else
            {
                return new Image2fWrapper(image, _interpolationConfig.Subdivision);
            }
        }

        private class Image2fWrapper : IImage2f
        {
            protected readonly IImage2i _image;
            protected readonly int _subdivision;
            
            public Image2fWrapper(IImage2i image,int subdivision)
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
        }

        private class Image2fInvalidatableWrapper : Image2fWrapper,IImageInvalidatableImage2i
        {
            public Image2fInvalidatableWrapper(IImage2i image, int subdivision)
                :base(image,subdivision)
            {
                var invalidatedArea = (image as IImageInvalidatableImage2i).InvalidatedArea;
                InvalidatedArea = Range2i.FromMinAndMax(invalidatedArea.Min * subdivision, invalidatedArea.Max * subdivision);
            }

            public Range2i InvalidatedArea { get; }

        }
    }
}