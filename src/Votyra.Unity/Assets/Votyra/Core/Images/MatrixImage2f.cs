using System;
using UnityEngine;
using Votyra.Models;
using Votyra.Utils;

namespace Votyra.Core.Images
{
    internal class MatrixImage2f : IImage2f, IInitializableImage, IImageInvalidatableImage2i, IDisposable
    {
        public Range2 RangeZ { get; }

        public Rect2i InvalidatedArea { get; }

        private readonly LockableMatrix<float> _image;

        public MatrixImage2f(LockableMatrix<float> values, Rect2i invalidatedArea)
        {
            _image = values;
            InvalidatedArea = invalidatedArea;
            RangeZ = CalculateRangeZ(values);
        }

        private static Range2 CalculateRangeZ(LockableMatrix<float> values)
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
            if (point.x < 0 || point.y < 0 || point.x >= _image.size.x || point.y >= _image.size.y)
                return 0;
            return _image[point.x, point.y];
        }

        public void StartUsing()
        {
            _image.Lock(this);
        }

        public void FinishUsing()
        {
            _image.Unlock(this);
        }

        public void Dispose()
        {
            if (_image.IsLocked)
            {
                _image.Unlock(this);
            }
        }
    }
}
