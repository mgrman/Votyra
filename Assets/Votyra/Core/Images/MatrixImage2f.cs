using System;
using Votyra.Core.Models;

namespace Votyra.Core.Images
{
    public class MatrixImage2f : IImage2f, IInitializableImage, IImageInvalidatableImage2
    {
        private int usingCounter;

        public MatrixImage2f(MatrixImage2f template, Range2i invalidatedArea, Area1f rangeZ)
        {
            _image = template._image;
            InvalidatedArea = invalidatedArea;
            RangeZ = rangeZ;
        }

        public MatrixImage2f(Matrix2<float> template, Range2i invalidatedArea, Area1f rangeZ)
        {
            _image= new Matrix2<float>(template.Size);
            InvalidatedArea = invalidatedArea;
            RangeZ = rangeZ;

            UpdateImage(template);
        }

        public void UpdateImage(Matrix2<float> template)
        {

            template.ForeachPointExlusive(i =>
            {
                _image[i] = template[i];
            });
        }

        private readonly Matrix2<float> _image;

        public Area1f RangeZ { get; }

        public float Sample(Vector2i point) => _image.TryGet(point, 0f);

        public IPoolableMatrix2<float> SampleArea(Range2i area)
        {
            var min = area.Min;
            var matrix = PoolableMatrix<float>.CreateDirty(area.Size);
            matrix.Size.ForeachPointExlusive(matPoint =>
            {
                matrix[matPoint] = Sample(matPoint + min);
            });
            return matrix;
        }

        public Range2i InvalidatedArea { get; }

        public bool IsBeingUsed => usingCounter > 0;

        public void StartUsing()
        {
            usingCounter++;
        }

        public void FinishUsing()
        {
            usingCounter--;
        }
    }
}