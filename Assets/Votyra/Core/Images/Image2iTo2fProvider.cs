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
                return new Image2fInvalidatableWrapper(image);
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
                matrix.Size.ForeachPointExlusive(matPoint =>
                {
                    matrix[matPoint] = Sample(matPoint+min);
                });
                return matrix;
            }
        }

        private class Image2fInvalidatableWrapper : Image2fWrapper, IImageInvalidatableImage2i
        {
            public Image2fInvalidatableWrapper(IImage2i image)
                :base(image)
            {
            }


            public Range2i InvalidatedArea => (_image as IImageInvalidatableImage2i).InvalidatedArea;
        }
    }
}