using Votyra.Core.Models;

namespace Votyra.Core.Images
{
    public class BaseMatrix2<T> : IInitializableImage, IImageInvalidatableImage2
        where T : struct
    {
        private readonly T[,] image;
        private readonly Range2i imageRange;

        private int usingCounter;

        protected BaseMatrix2(Vector2i size)
        {
            this.image = new T[size.X, size.Y];
            this.imageRange = Range2i.FromMinAndSize(Vector2i.Zero, size);
        }

        public bool IsBeingUsed => this.usingCounter > 0;

        public Range2i InvalidatedArea { get; private set; }

        public void StartUsing()
        {
            this.usingCounter++;
        }

        public void FinishUsing()
        {
            this.usingCounter--;
        }

        public T Sample(Vector2i point) => this.imageRange.Contains(point) ? this.image[point.X, point.Y] : default;

        public PoolableMatrix2<T> SampleArea(Range2i area)
        {
            var min = area.Min;
            var matrix = PoolableMatrix2<T>.CreateDirty(area.Size);
            var rawMatrix = matrix.RawMatrix;

            for (var ix = 0; ix < rawMatrix.SizeX(); ix++)
            {
                for (var iy = 0; iy < rawMatrix.SizeY(); iy++)
                {
                    var matPoint = new Vector2i(ix, iy);
                    rawMatrix.Set(matPoint, this.Sample(matPoint + min));
                }
            }

            return matrix;
        }

        public void UpdateImage(T[,] template)
        {
            var size = this.image.Size();
            for (var ix = 0; ix < size.X; ix++)
            {
                for (var iy = 0; iy < size.Y; iy++)
                {
                    this.image[ix, iy] = template[ix, iy];
                }
            }
        }

        public void UpdateInvalidatedArea(Range2i invalidatedArea)
        {
            this.InvalidatedArea = invalidatedArea;
        }
    }
}
