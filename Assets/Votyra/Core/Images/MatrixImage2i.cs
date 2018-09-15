using System;
using Votyra.Core.Models;

namespace Votyra.Core.Images
{
    public class MatrixImage2i : IImage2i, IInitializableImage, IImageInvalidatableImage2i, IDisposable
    {
        public MatrixImage2i(LockableMatrix2<Height> values, Range2i invalidatedArea)
        {
            Image = values;
            InvalidatedArea = invalidatedArea;
            RangeZ = CalculateRangeZ(values);
        }

        public Range1h RangeZ { get; }

        public Range2i InvalidatedArea { get; }

        public LockableMatrix2<Height> Image { get; }

        public Height Sample(Vector2i point)
        {
            return Image.TryGet(point, Height.Default);
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

        private static Range1h CalculateRangeZ(LockableMatrix2<Height> values)
        {
            Height min = Height.MaxValue;
            Height max = Height.MinValue;
            values.ForeachPointExlusive(i =>
            {
                Height val = values[i];

                min = Height.Min(min, val);
                max = Height.Max(max, val);
            });
            return Height.Range(min, max);
        }
    }
}