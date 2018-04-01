using System;
using Votyra.Core.Models;

namespace Votyra.Core.Images
{
    public class MatrixImage2f : IImage2f, IInitializableImage, IImageInvalidatableImage2i, IDisposable
    {
        public Range1f RangeZ { get; }

        public Range2i InvalidatedArea { get; }

        public LockableMatrix2<float> Image { get; }

        public MatrixImage2f(LockableMatrix2<float> values, Range2i invalidatedArea)
        {
            Image = values;
            InvalidatedArea = invalidatedArea;
            RangeZ = CalculateRangeZ(values);
        }

        private static Range1f CalculateRangeZ(LockableMatrix2<float> values)
        {
            float min = float.MaxValue;
            float max = float.MinValue;
            values.ForeachPointExlusive(i =>
            {
                float val = values[i];

                min = Math.Min(min, val);
                max = Math.Max(max, val);
            });
            return new Range1f(min, max);
        }

        public float Sample(Vector2i point)
        {
            return Image.TryGet(point, 0);
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