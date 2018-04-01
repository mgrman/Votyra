using System;
using Votyra.Core.Models;

namespace Votyra.Core.Images
{
    public class MatrixImage2f : IImage2f, IInitializableImage, IImageInvalidatableImage2i, IDisposable
    {
        public Range2f RangeZ { get; }

        public Rect2i InvalidatedArea { get; }

        public LockableMatrix2<float> Image { get; }

        public MatrixImage2f(LockableMatrix2<float> values, Rect2i invalidatedArea)
        {
            Image = values;
            InvalidatedArea = invalidatedArea;
            RangeZ = CalculateRangeZ(values);
        }

        private static Range2f CalculateRangeZ(LockableMatrix2<float> values)
        {
            int countX = values.Size.X;
            int countY = values.Size.Y;

            float min = float.MaxValue;
            float max = float.MinValue;
            for (int ix = 0; ix < countX; ix++)
            {
                for (int iy = 0; iy < countY; iy++)
                {
                    var i = new Vector2i(ix, iy);
                    float val = values[i];

                    min = Math.Min(min, val);
                    max = Math.Max(max, val);
                }
            }
            return new Range2f(min, max);
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