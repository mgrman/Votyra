using System;
using Votyra.Core.Models;

namespace Votyra.Core.Images
{
    public class BaseMatrix3<T> : IInitializableImage, IImageInvalidatableImage3 where T : struct
    {
        protected readonly T[,,] _image;

        private int _usingCounter;

        protected BaseMatrix3(Vector3i size)
        {
            this._image = new T[size.X, size.Y, size.Z];
        }

        public bool IsBeingUsed => this._usingCounter > 0;

        public Range3i InvalidatedArea { get; private set; }

        public void StartUsing()
        {
            this._usingCounter++;
        }

        public void FinishUsing()
        {
            this._usingCounter--;
        }

        public T Sample(Vector3i point) => this._image.TryGet(point, default);

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
            Array.Copy(template, this._image, this._image.Length);
        }

        public void UpdateInvalidatedArea(Range3i invalidatedArea)
        {
            this.InvalidatedArea = invalidatedArea;
        }
    }
}
