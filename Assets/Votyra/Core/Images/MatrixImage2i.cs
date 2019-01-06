using System;
using Votyra.Core.Models;

namespace Votyra.Core.Images
{
    public class MatrixImage2i : IImage2f, IInitializableImage, IImageInvalidatableImage2i, IDisposable
    {
        public MatrixImage2i(LockableMatrix2<float> values, Range2i invalidatedArea,Area1f rangeZ)
        {
            Image = values;
            InvalidatedArea = invalidatedArea;
            RangeZ = rangeZ;
        }

        public Area1f RangeZ { get; }

        public Range2i InvalidatedArea { get; }

        public LockableMatrix2<float> Image { get; }

        public float Sample(Vector2i point)
        {
            return Image.TryGet(point, 0f);
        }

        public IPoolableMatrix2<float> SampleArea(Range2i area)
        {
            var min = area.Min;
            var matrix = PoolableMatrix<float>.CreateDirty(area.Size);
            matrix.Size.ForeachPointExlusive(matPoint => { matrix[matPoint] = Sample(matPoint + min); });
            return matrix;
        }

        public void StartUsing()
        {
            Image.Lock(this);
        }

        public void FinishUsing()
        {
            Image.Unlock(this);
        }

        public void Dispose()
        {
            if (Image.IsLocked)
            {
                Image.Unlock(this);
            }
        }
    }
}