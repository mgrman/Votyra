using System;
using TycoonTerrain.Common.Models;
using TycoonTerrain.Common.Utils;
using TycoonTerrain.Models;

namespace TycoonTerrain.Images
{
    internal class MatrixImage : IImage2i,IDisposable
    {
        public Range2i RangeZ { get; private set; }

        private readonly LockableMatrix<int> _image;

        public MatrixImage(LockableMatrix<int> values )
        {
            values.Lock(this);
            _image = values;
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
        
        public bool IsAnimated
        {
            get { return false; }
        }

        public int Sample(Vector2i point, float time)
        {
            if (point.x < 0 || point.y < 0 || point.x >= _image.size.x || point.y >= _image.size.y)
                return 0;

            return _image[point.x, point.y];
        }

        public void Dispose()
        {
            _image .Unlock(this);
        }
        
    }
}