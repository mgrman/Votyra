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
            if (point.X < 0 || point.Y < 0 || point.Z < 0 || point.X >= Image.size.X || point.Y >= Image.size.Y || point.Z >= Image.size.Z)
                return false;
            return Image[point.X, point.Y, point.Z];
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