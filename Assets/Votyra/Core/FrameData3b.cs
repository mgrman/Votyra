using Votyra.Core.Images;
using Votyra.Core.Models;
using Votyra.Core.Pooling;
using Votyra.Core.Utils;

namespace Votyra.Core
{
    public class FrameData3b : IFrameData3b
    {
        private int _activeCounter;

        public FrameData3b(Ray3f cameraRay, IReadOnlyPooledList<Plane3f> cameraPlanes, IReadOnlyPooledList<Vector3f> cameraFrustumCorners, IReadOnlySet<Vector3i> existingGroups, IImage3b image, Range3i invalidatedArea_imageSpace)
        {
            CameraRay = cameraRay;
            CameraPlanes = cameraPlanes;
            CameraFrustumCorners = cameraFrustumCorners;
            ExistingGroups = existingGroups;
            Image = image;

            InvalidatedArea_imageSpace = invalidatedArea_imageSpace;

            (Image as IInitializableImage)?.StartUsing();
        }

        public Ray3f CameraRay { get; }
        public IReadOnlyPooledList<Plane3f> CameraPlanes { get; }
        public IReadOnlyPooledList<Vector3f> CameraFrustumCorners { get; }
        public IReadOnlySet<Vector3i> ExistingGroups { get; }
        public IImage3b Image { get; }
        public Range3i InvalidatedArea_imageSpace { get; }

        public void Activate()
        {
            _activeCounter++;
        }

        public void Deactivate()
        {
            _activeCounter--;
            if (_activeCounter <= 0)
                Dispose();
        }

        private void Dispose()
        {
            CameraPlanes.TryDispose();
            CameraFrustumCorners.TryDispose();
            (Image as IInitializableImage)?.FinishUsing();
        }
    }
}