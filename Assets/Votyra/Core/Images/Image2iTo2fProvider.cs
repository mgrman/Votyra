using System.Collections.Generic;
using System.Linq;
using Votyra.Core.Images.Constraints;
using Votyra.Core.Models;
using Zenject;

namespace Votyra.Core.Images
{
    public class Image2iTo2fProvider : IImage2fProvider
    {
        private readonly IImage2iProvider _provider;
        
        public Image2iTo2fProvider(IImage2iProvider provider)
        {
            _provider = provider;
        }


        public IImage2f CreateImage()
        {
            var image = _provider.CreateImage();
            if (image is IImageInvalidatableImage2i)
            {
                return  new Image2fInvalidatableWrapper(image);
            }
            else
            {
                return new Image2fWrapper(image);
            }
        }

        private class Image2fWrapper : IImage2f
        {
            public Image2fWrapper(IImage2i image)
            {
                _image = image;
            }

            private readonly IImage2i _image;

            public Range1hf RangeZ => _image.RangeZ.ToRange1hf();

            public Height1f Sample(Vector2i point)
            {
                return _image.Sample(point).ToHeight1f();
            }
        }

        private class Image2fInvalidatableWrapper : IImage2f,IImageInvalidatableImage2i
        {
            public Image2fInvalidatableWrapper(IImage2i image)
            {
                _image = image;
            }

            private readonly IImage2i _image;

            public Range1hf RangeZ => _image.RangeZ.ToRange1hf();

            public Height1f Sample(Vector2i point)
            {
                return _image.Sample(point).ToHeight1f();
            }

            public Range2i InvalidatedArea => (_image as IImageInvalidatableImage2i).InvalidatedArea;
        }
    }
}