using System.Collections.Generic;
using Votyra.Core.Images;
using Votyra.Core.Models;

namespace Votyra.Core
{
    public class FrameData2i : IPoolableFrameData2i
    {
        private IImage2f _image;
        private IMask2e _mask;

        public FrameData2i()
        {
            CameraPlanes = new Plane3f[6];
            CameraFrustumCorners = new Vector3f[4];
        }

        public Vector2i CellInGroupCount { get; set; }

        public Ray3f CameraRay { get; set; }

        public Plane3f[] CameraPlanes { get; }

        public Vector3f[] CameraFrustumCorners { get; }

        IReadOnlyList<Plane3f> IFrameData.CameraPlanes => CameraPlanes;

        IReadOnlyList<Vector3f> IFrameData.CameraFrustumCorners => CameraFrustumCorners;

        public Area1f RangeZ { get; private set; }

        public Range2i InvalidatedArea { get; set; }

        public IImage2f Image
        {
            get => _image;
            set
            {
                (_image as IInitializableImage)?.FinishUsing();
                _image = value;
                (_image as IInitializableImage)?.StartUsing();
                RangeZ = _image?.RangeZ ?? Area1f.Zero;
            }
        }

        public IMask2e Mask
        {
            get => _mask;
            set
            {
                (_mask as IInitializableImage)?.FinishUsing();
                _mask = value;
                (_mask as IInitializableImage)?.StartUsing();
            }
        }
    }
}
