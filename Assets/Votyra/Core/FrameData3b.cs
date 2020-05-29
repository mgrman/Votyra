using System.Collections.Generic;
using Votyra.Core.Images;
using Votyra.Core.Models;
using Votyra.Core.Utils;

namespace Votyra.Core
{
    public class FrameData3b : IFrameData3b
    {
        private int _activeCounter;

        public FrameData3b(Ray3f cameraRay, IReadOnlyList<Plane3f> cameraPlanes, IReadOnlyList<Vector3f> cameraFrustumCorners, IReadOnlySet<Vector3i> existingGroups, IImage3b image, Range3i invalidatedArea_imageSpace)
        {
            this.CameraRay = cameraRay;
            this.CameraPlanes = cameraPlanes;
            this.CameraFrustumCorners = cameraFrustumCorners;
            this.ExistingGroups = existingGroups;
            this.Image = image;

            this.InvalidatedArea_imageSpace = invalidatedArea_imageSpace;

            (this.Image as IInitializableImage)?.StartUsing();
        }

        public Ray3f CameraRay { get; }

        public IReadOnlyList<Plane3f> CameraPlanes { get; }

        public IReadOnlyList<Vector3f> CameraFrustumCorners { get; }

        public IReadOnlySet<Vector3i> ExistingGroups { get; }

        public IImage3b Image { get; }

        public Range3i InvalidatedArea_imageSpace { get; }

        public void Activate()
        {
            this._activeCounter++;
        }

        public void Deactivate()
        {
            this._activeCounter--;
            if (this._activeCounter <= 0)
            {
                this.Dispose();
            }
        }

        private void Dispose()
        {
            this.CameraPlanes.TryDispose();
            this.CameraFrustumCorners.TryDispose();
            (this.Image as IInitializableImage)?.FinishUsing();
        }
    }
}
