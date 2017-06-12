using System;
using UnityEngine;
using Votyra.Models;
using Votyra.Utils;

namespace Votyra.Images
{
    internal class MatrixImage2i : IImage2i, IInitializableImage, IImageInvalidatableImage2i, IDisposable
    {
        public Range2i RangeZ { get; }

        public Rect2i InvalidatedArea { get; }

        private readonly LockableMatrix<int> _image;

        public MatrixImage2i(LockableMatrix<int> values, Rect2i invalidatedArea)
        {
            _image = values;
            InvalidatedArea = invalidatedArea;
            RangeZ = CalculateRangeZ(values);
        }

        private static Range2i CalculateRangeZ(LockableMatrix<int> values)
        {
            int countX = values.size.x;
            int countY = values.size.y;

            int min = int.MaxValue;
            int max = int.MinValue;
            for (int x = 0; x < countX; x++)
            {
                for (int y = 0; y < countY; y++)
                {
                    int val = values[x, y];

                    min = Math.Min(min, val);
                    max = Math.Max(max, val);
                }
            }
            return new Range2i(min, max);
        }

        public int Sample(Vector2i point)
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