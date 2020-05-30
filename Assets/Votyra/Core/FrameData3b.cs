using System.Collections.Generic;
using Votyra.Core.Images;
using Votyra.Core.Models;
using Votyra.Core.Utils;

namespace Votyra.Core
{
    public class FrameData3B : IFrameData3B
    {
        private int activeCounter;

        public FrameData3B(Ray3f cameraRay, IReadOnlyList<Plane3f> cameraPlanes, IReadOnlyList<Vector3f> cameraFrustumCorners, IReadOnlySet<Vector3i> existingGroups, IImage3B image, Range3i invalidatedAreaImageSpace)
        {
            this.CameraRay = cameraRay;
            this.CameraPlanes = cameraPlanes;
            this.CameraFrustumCorners = cameraFrustumCorners;
            this.ExistingGroups = existingGroups;
            this.Image = image;

            this.InvalidatedAreaImageSpace = invalidatedAreaImageSpace;

            (this.Image as IInitializableImage)?.StartUsing();
        }

        public Ray3f CameraRay { get; }

        public IReadOnlyList<Plane3f> CameraPlanes { get; }

        public IReadOnlyList<Vector3f> CameraFrustumCorners { get; }

        public IReadOnlySet<Vector3i> ExistingGroups { get; }

        public IImage3B Image { get; }

        public Range3i InvalidatedAreaImageSpace { get; }

        public void Activate()
        {
            this.activeCounter++;
        }

        public void Deactivate()
        {
            this.activeCounter--;
            if (this.activeCounter <= 0)
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
