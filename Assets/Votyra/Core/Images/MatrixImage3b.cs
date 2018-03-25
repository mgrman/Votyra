using System;
using Votyra.Core.Models;

namespace Votyra.Core.Images
{
    public class MatrixImage3b : IImage3b, IInitializableImage, IImageInvalidatableImage3i, IDisposable
    {
        public Rect3i InvalidatedArea { get; }

        public LockableMatrix3<bool> Image { get; }

        public MatrixImage3b(LockableMatrix3<bool> values, Rect3i invalidatedArea)
        {
            Image = values;
            InvalidatedArea = invalidatedArea;
        }


        public bool Sample(Vector3i point)
        {
            if (point.x < 0 || point.y < 0 || point.z < 0 || point.x >= Image.size.x || point.y >= Image.size.y || point.z >= Image.size.z)
                return false;
            return Image[point.x, point.y, point.z];
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