using System;
using TycoonTerrain.Common.Models;
using TycoonTerrain.Common.Utils;

namespace TycoonTerrain.Images
{
    internal class MatrixImage : IImage2i
    {
        public Vector2i Size { get; private set; }

        public Range2i RangeZ { get; private set; }

        private readonly int[,] _image;

        public MatrixImage(Vector2i size, Range2i rangeZ)
        {
            Size = size;
            RangeZ = rangeZ;
            _image = new int[size.x, size.y];
        }

        public int this[int x, int y]
        {
            get
            {
                x = Math.Max(Math.Min(x, Size.x - 1), 0);
                y = Math.Max(Math.Min(y, Size.y - 1), 0);
                return _image[x, y];
            }
            set
            {
                if (x >= 0 && x < Size.x && y >= 0 && y < Size.y)
                {
                    _image[x, y] = value;
                    RecalculateRange(value);
                }
            }
        }

        private void RecalculateRange(int value)
        {
            int min = Math.Min(RangeZ.min, value);
            int max = Math.Max(RangeZ.max, value);
            RangeZ = new Range2i(min, max);
        }

        private void RecalculateRange()
        {
            int countX = _image.GetCountX();
            int countY = _image.GetCountY();

            int min = int.MaxValue;
            int max = int.MinValue;
            for (int x = 0; x < countX; x++)
            {
                for (int y = 0; y < countY; y++)
                {
                    int val = _image[x, y];

                    min = Math.Min(min, val);
                    max = Math.Max(max, val);
                }
            }
            RangeZ = new Range2i(min, max);
        }

        #region IImage2i

        Range2i IImage2i.RangeZ
        {
            get { return RangeZ; }
        }

        public bool IsAnimated
        {
            get { return true; }
        }

        public int Sample(Vector2i point, float time)
        {
            if (point.x < 0 || point.y < 0 || point.x >= Size.x || point.y >= Size.y)
                return 0;

            return _image[point.x, point.y];
        }

        #endregion IImage2i
    }
}