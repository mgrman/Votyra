using System;
using Votyra.Core.Models;

namespace Votyra.Core.Images
{
    public class MatrixImage3b : IImage3b, IInitializableImage, IImageInvalidatableImage3, IDisposable
    {
        public MatrixImage3b(LockableMatrix3<bool> values, Range3i invalidatedArea)
        {
            Image = values;
            InvalidatedArea = invalidatedArea;
        }

        public LockableMatrix3<bool> Image { get; }

        public void Dispose()
        {
            if (Image.IsLocked)
                Image.Unlock(this);
        }

        public bool Sample(Vector3i point) => Image.TryGet(point, false);

        public bool AnyData(Range3i range)
        {
            var allFalse = true;
            var allTrue = true;
            range.ForeachPointExlusive(o =>
            {
                var value = Sample(o);
                allFalse = allFalse && !value;
                allTrue = allTrue && value;
            });

            return !allFalse && !allTrue;
        }

        public Range3i InvalidatedArea { get; }

        public void StartUsing()
        {
            Image.Lock(this);
        }

        public void FinishUsing()
        {
            Image.Unlock(this);
        }
    }
}