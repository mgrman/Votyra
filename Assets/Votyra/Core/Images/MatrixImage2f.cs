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
            int countX = values.size.X;
            int countY = values.size.Y;

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
            return new Range2f(min, max);
        }

        public float Sample(Vector2i point)
        {
            if (point.X < 0 || point.Y < 0 || point.X >= Image.size.X || point.Y >= Image.size.Y)
                return 0;
            return Image[point.X, point.Y];
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