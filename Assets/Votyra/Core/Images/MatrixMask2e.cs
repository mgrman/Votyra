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

        public LockableMatrix2<MaskValues> Image { get; }

        public void Dispose()
        {
            if (Image.IsLocked)
                Image.Unlock(this);
        }

        public Range2i InvalidatedArea { get; }

        public void StartUsing()
        {
            Image.Lock(this);
        }

        public void FinishUsing()
        {
            Image.Unlock(this);
        }

        public bool AnyData(Range2i range)
        {
            var allHoles = true;
            var min = range.Min;
            var max = range.Max;
            for (var ix = min.X; ix < max.X; ix++)
            {
                for (var iy = min.Y; iy <= max.Y; iy++)
                {
                    var o=new Vector2i(ix, iy);
                    var value = Sample(o);
                    allHoles = allHoles && value.IsHole();
                }
            }

            return !allHoles;
        }

        public MaskValues Sample(Vector2i point) => Image.TryGet(point, MaskValues.Terrain);
    }
}