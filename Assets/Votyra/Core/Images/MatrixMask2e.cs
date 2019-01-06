using System;
using Votyra.Core.Models;

namespace Votyra.Core.Images
{
    public class MatrixMask2e : IMask2e, IInitializableImage, IImageInvalidatableImage2, IDisposable
    {
        public MatrixMask2e(LockableMatrix2<MaskValues> values, Range2i invalidatedArea)
        {
            Image = values;
            InvalidatedArea = invalidatedArea;
        }

        public Range2i InvalidatedArea { get; }

        public LockableMatrix2<MaskValues> Image { get; }

        public bool AnyData(Range2i range)
        {
            bool allHoles = true;
            range.ForeachPointExlusive(o =>
            {
                var value = Sample(o);
                allHoles = allHoles && value.IsHole();
            });

            return !allHoles;
        }

        public MaskValues Sample(Vector2i point)
        {
            return Image.TryGet(point, MaskValues.Terrain);
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