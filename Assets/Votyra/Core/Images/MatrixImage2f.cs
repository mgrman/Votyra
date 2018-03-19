using System;
using Votyra.Core.Models;

namespace Votyra.Core.Images
{
    public class MatrixImage2f : IImage2f, IInitializableImage, IImageInvalidatableImage2i, IDisposable
    {
        public Range2 RangeZ { get; }

        public Rect2i InvalidatedArea { get; }

        public LockableMatrix2<float> Image { get; }

        public MatrixImage2f(LockableMatrix2<float> values, Rect2i invalidatedArea)
        {
            Image = values;
            InvalidatedArea = invalidatedArea;
            RangeZ = CalculateRangeZ(values);
        }

        private static Range2 CalculateRangeZ(LockableMatrix2<float> values)
        {
            int countX = values.size.x;
            int countY = values.size.y;

            float min = float.MaxValue;
            float max = float.MinValue;
            for (int x = 0; x < countX; x++)
            {
                for (int y = 0; y < countY; y++)
                {
                    float val = values[x, y];

                    min = Math.Min(min, val);
                    max = Math.Max(max, val);
                }
            }
            return new Range2(min, max);
        }

        public float Sample(Vector2i point)
        {
            if (point.x < 0 || point.y < 0 || point.x >= Image.size.x || point.y >= Image.size.y)
                return 0;
            return Image[point.x, point.y];
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