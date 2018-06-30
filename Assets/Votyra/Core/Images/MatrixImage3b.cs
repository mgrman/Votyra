using System;
using Votyra.Core.Models;

namespace Votyra.Core.Images
{
    public class MatrixImage3b : IImage3b, IInitializableImage, IImageInvalidatableImage3i, IDisposable
    {
        public MatrixImage3b(LockableMatrix3<bool> values, Range3i invalidatedArea)
        {
            Image = values;
            InvalidatedArea = invalidatedArea;
        }

        public LockableMatrix3<bool> Image { get; }
        public Range3i InvalidatedArea { get; }

        public bool AnyData(Range3i range)
        {
            bool allFalse = true;
            bool allTrue = true;
            range.ForeachPointExlusive(o =>
            {
                var value = Sample(o);
                allFalse = allFalse && !value;
                allTrue = allTrue && value;
            });

            return !allFalse && !allTrue;
        }

        public void Dispose()
        {
            if (Image.IsLocked)
            {
                Image.Unlock(this);
            }
        }

        public void FinishUsing()
        {
            Image.Unlock(this);
        }

        public bool Sample(Vector3i point)
        {
            return Image.TryGet(point, false);
        }

        public void StartUsing()
        {
            Image.Lock(this);
        }
    }
}