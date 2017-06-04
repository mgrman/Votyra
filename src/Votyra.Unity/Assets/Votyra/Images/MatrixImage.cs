using System;
using UnityEngine;
using Votyra.Common.Models;
using Votyra.Common.Utils;
using Votyra.Models;

namespace Votyra.Images
{
    internal class MatrixImage : IImage2i, IDisposable
    {
        public Range2i RangeZ { get; }

        private readonly LockableMatrix<int> _image;

        public Rect InvalidatedArea { get; }

        public MatrixImage(LockableMatrix<int> values, Rect invalidatedArea)
        {
            values.Lock(this);
            _image = values;
            RangeZ = CalculateRangeZ(values);
            InvalidatedArea = invalidatedArea;
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

        public void Dispose()
        {
            _image.Unlock(this);
        }

    }
}