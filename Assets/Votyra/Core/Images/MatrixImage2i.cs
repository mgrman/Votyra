using System;
using Votyra.Core.Models;

namespace Votyra.Core.Images
{
    public class MatrixImage2i : IImage2i, IInitializableImage, IImageInvalidatableImage2i, IDisposable
    {
        private const int subdivision = 8;
        public MatrixImage2i(LockableMatrix2<Height> values, Range2i invalidatedArea)
        {
            Image = values;
            InvalidatedArea = Range2i.FromMinAndMax(invalidatedArea.Min * subdivision, invalidatedArea.Max *
                                                                                       subdivision);
            RangeZ = CalculateRangeZ(values);
            RangeZ=new Range1h(new Height(RangeZ.Min.RawValue * 10), new Height(RangeZ.Max.RawValue * 10));
        }

        public Range1h RangeZ { get; }

        public Range2i InvalidatedArea { get; }

        public LockableMatrix2<Height> Image { get; }

        public Height Sample(Vector2i point)
        {
            var minPoint = point / subdivision;

            var fraction = (point - minPoint * subdivision) / (float) subdivision;

            var x0y0 = Image.TryGet(minPoint, Height.Default);
            var x0y1 = Image.TryGet(minPoint + new Vector2i(0, 1), Height.Default);
            var x1y0 = Image.TryGet(minPoint + new Vector2i(1, 0), Height.Default);
            var x1y1 = Image.TryGet(minPoint + new Vector2i(1, 1), Height.Default);

            var value = (1f - fraction.X) * (1f - fraction.Y) * x0y0.RawValue + fraction.X * (1f - fraction.Y) * x1y0.RawValue+ (1f - fraction.X) * fraction.Y *x0y1.RawValue + fraction.X * fraction.Y * x1y1.RawValue;
            
            return new Height((int) (value*10f));
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