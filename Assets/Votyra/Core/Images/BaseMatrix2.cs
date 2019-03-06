using System;
using Votyra.Core.Models;

namespace Votyra.Core.Images
{
    public class BaseMatrix2<T> : IInitializableImage, IImageInvalidatableImage2 where T : struct
    {
        private readonly T[,] _image;
        private readonly Range2i _imageRange;

        private int _usingCounter;

        public BaseMatrix2(Vector2i size)
        {
            _image = new T[size.X, size.Y];
            _imageRange = Range2i.FromMinAndSize(Vector2i.Zero, size);
        }

        public bool IsBeingUsed => _usingCounter > 0;

        public Range2i InvalidatedArea { get; private set; }

        public void StartUsing()
        {
            _usingCounter++;
        }

        public void FinishUsing()
        {
            _usingCounter--;
        }

        public T Sample(Vector2i point) => _imageRange.Contains(point) ? _image[point.X, point.Y] : default;

        public IPoolableMatrix2<T> SampleArea(Range2i area)
        {
            var min = area.Min;
            var matrix = PoolableMatrix<T>.CreateDirty(area.Size);

            for (var ix = 0; ix < matrix.Size.X; ix++)
            {
                for (var iy = 0; iy < matrix.Size.Y; iy++)
                {
                    var matPoint = new Vector2i(ix, iy);
                    matrix[matPoint] = Sample(matPoint + min);
                }
            }

            return matrix;
        }

        public void UpdateImage(Matrix2<T> template)
        {
            Array.Copy(template.NativeMatrix, _image, _image.Length);
        }

        public void UpdateInvalidatedArea(Range2i invalidatedArea)
        {
            InvalidatedArea = invalidatedArea;
        }
    }
}