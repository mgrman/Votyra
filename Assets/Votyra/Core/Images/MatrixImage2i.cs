using System;
using Votyra.Core.Models;

namespace Votyra.Core.Images
{
    public class MatrixImage2i : IImage2i, IInitializableImage, IImageInvalidatableImage2i, IDisposable
    {
        public Range1f RangeZ { get; }

        public Range2i InvalidatedArea { get; }

        public LockableMatrix2<int> Image { get; }

        public MatrixImage2i(LockableMatrix2<int> values, Range2i invalidatedArea)
        {
            Image = values;
            InvalidatedArea = invalidatedArea;
            RangeZ = CalculateRangeZ(values);
        }

        private static Range1f CalculateRangeZ(LockableMatrix2<int> values)
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

        public int Sample(Vector2i point)
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

        public bool AnyData(Range2i range)
        {
            bool allHoles = true;
            range.ForeachPointExlusive(o =>
            {
                var value = Sample(o);
                allHoles = allHoles && value.IsHole();
            });

            return !allHoles;
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