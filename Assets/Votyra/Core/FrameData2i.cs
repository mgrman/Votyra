using System.Collections.Generic;
using Votyra.Core.Images;
using Votyra.Core.Models;

namespace Votyra.Core
{
    public class FrameData2I : IPoolableFrameData2I
    {
        private IImage2F image;

        public FrameData2I()
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

        public IImage2F Image
        {
            get => this.image;
            set
            {
                (this.image as IInitializableImage)?.FinishUsing();
                this.image = value;
                (this.image as IInitializableImage)?.StartUsing();
                this.RangeZ = this.image?.RangeZ ?? Area1f.Zero;
            }
        }
    }
}
