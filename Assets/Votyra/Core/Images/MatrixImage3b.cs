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
            var min = range.Min;
            for (var ix = 0; ix < range.Size.X; ix++)
            {
                for (var iy = 0; iy < range.Size.Y; iy++)
                {
                    for (var iz = 0; iz < range.Size.Z; iz++)
                    {
                        var o=new Vector3i(ix, iy, iz)+min;
                        var value = Sample(o);
                        allFalse = allFalse && !value;
                        allTrue = allTrue && value;
                    }
                }
            }

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