using System;
using Votyra.Core.Models;

namespace Votyra.Core.Images
{
    public class BaseMatrix3<T> : IInitializableImage, IImageInvalidatableImage3
        where T : struct
    {
        private int usingCounter;

        protected BaseMatrix3(Vector3i size)
        {
            this.Image = new T[size.X, size.Y, size.Z];
        }

        public bool IsBeingUsed => this.usingCounter > 0;

        protected T[,,] Image { get; }

        public Range3i InvalidatedArea { get; private set; }

        public void StartUsing()
        {
            this.usingCounter++;
        }

        public void FinishUsing()
        {
            this.usingCounter--;
        }

        public T Sample(Vector3i point) => this.Image.TryGet(point, default);

        public PoolableMatrix3<T> SampleArea(Range3i area)
        {
            var min = area.Min;
            var matrix = PoolableMatrix3<T>.CreateDirty(area.Size);
            var rawMatrix = matrix.RawMatrix;

            for (var ix = 0; ix < rawMatrix.SizeX(); ix++)
            {
                for (var iy = 0; iy < rawMatrix.SizeY(); iy++)
                {
                    for (var iz = 0; iy < rawMatrix.SizeZ(); iz++)
                    {
                        var matPoint = new Vector3i(ix, iy, iz);
                        rawMatrix.Set(matPoint, this.Sample(matPoint + min));
                    }
                }
            }

            return matrix;
        }

        public void UpdateImage(T[,,] template)
        {
            Array.Copy(template, this.Image, this.Image.Length);
        }

        public void UpdateInvalidatedArea(Range3i invalidatedArea)
        {
            this.InvalidatedArea = invalidatedArea;
        }
    }
}
