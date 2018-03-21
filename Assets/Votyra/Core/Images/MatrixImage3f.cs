using System;
using Votyra.Core.Models;

namespace Votyra.Core.Images
{
    public class MatrixImage3f : IImage3f, IInitializableImage, IImageInvalidatableImage3i, IDisposable
    {
        public Rect3i InvalidatedArea { get; }

        public LockableMatrix3<float> Image { get; }

        public MatrixImage3f(LockableMatrix3<float> values, Rect3i invalidatedArea)
        {
            Image = values;
            InvalidatedArea = invalidatedArea;
        }


        public float Sample(Vector3i point)
        {
            if (point.x < 0 || point.y < 0 || point.z < 0 || point.x >= Image.size.x || point.y >= Image.size.y || point.z >= Image.size.z)
                return 0;
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