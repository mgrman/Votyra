using System;
using Votyra.Core.Models;

namespace Votyra.Core.Images
{
    public class MatrixImage2i : IImage2i, IInitializableImage, IImageInvalidatableImage2i, IDisposable
    {
        public MatrixImage2i(LockableMatrix2<Height1i> values, Range2i invalidatedArea)
        {
            Image = values;
            InvalidatedArea = invalidatedArea;
            RangeZ = CalculateRangeZ(values);
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

        private static Range1hi CalculateRangeZ(LockableMatrix2<Height1i> values)
        {
            Height1i min = Height1i.MaxValue;
            Height1i max = Height1i.MinValue;
            values.ForeachPointExlusive(i =>
            {
                Height1i val = values[i];

                min = Height1i.Min(min, val);
                max = Height1i.Max(max, val);
            });
            return Height1i.Range(min, max);
        }
    }
}