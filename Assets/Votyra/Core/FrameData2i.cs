using System.Collections.Generic;
using Votyra.Core.Images;
using Votyra.Core.Models;

namespace Votyra.Core
{
    public class FrameData2i : IPoolableFrameData2i
    {
        private IImage2f _image;

        public FrameData2i()
        {
            this.CameraPlanes = new Plane3f[6];
            this.CameraFrustumCorners = new Vector3f[4];
        }

        public Vector2i CellInGroupCount { get; set; }

        public Ray3f CameraRay { get; set; }

        public Plane3f[] CameraPlanes { get; }

        public Vector3f[] CameraFrustumCorners { get; }

        IReadOnlyList<Plane3f> IFrameData.CameraPlanes => this.CameraPlanes;

        IReadOnlyList<Vector3f> IFrameData.CameraFrustumCorners => this.CameraFrustumCorners;

        public Area1f RangeZ { get; private set; }

        public Range2i InvalidatedArea { get; set; }

        public IImage2f Image
        {
            get => this._image;
            set
            {
                (this._image as IInitializableImage)?.FinishUsing();
                this._image = value;
                (this._image as IInitializableImage)?.StartUsing();
                this.RangeZ = this._image?.RangeZ ?? Area1f.Zero;
            }
        }
    }
}
