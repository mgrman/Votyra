using System;
using Votyra.Core.Models;

namespace Votyra.Core.Images
{
    public class MatrixImage2i : IImage2i, IInitializableImage, IImageInvalidatableImage2i, IDisposable
    {
        public MatrixImage2i(LockableMatrix2<Height1i> values, Range2i invalidatedArea,Range1hi rangeZ)
        {
            Image = values;
            InvalidatedArea = invalidatedArea;
            RangeZ = rangeZ;
        }

        public Range1hi RangeZ { get; }

        public Range2i InvalidatedArea { get; }

        public LockableMatrix2<Height1i> Image { get; }

        public Height1i Sample(Vector2i point)
        {
            return Image.TryGet(point, Height1i.Default);
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