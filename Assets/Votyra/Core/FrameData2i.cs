using Votyra.Core.Images;
using Votyra.Core.Models;
using Votyra.Core.Pooling;
using Votyra.Core.Utils;

namespace Votyra.Core
{
    public class FrameData2i : IFrameData2i
    {
        private int _activeCounter;

        public FrameData2i(Ray3f cameraRay, IReadOnlyPooledList<Plane3f> cameraPlanes, IReadOnlyPooledList<Vector3f> cameraFrustumCorners, IImage2f image, IMask2e mask, Range2i invalidatedArea, Vector2i cellInGroupCount, int meshSubdivision)
        {
            CameraRay = cameraRay;
            CameraPlanes = cameraPlanes;
            CameraFrustumCorners = cameraFrustumCorners;
            Image = image;
            Mask = mask;

            RangeZ = image?.RangeZ ?? Area1f.Zero;

            InvalidatedArea = invalidatedArea;
            CellInGroupCount = cellInGroupCount;
            MeshSubdivision = meshSubdivision;

            (Image as IInitializableImage)?.StartUsing();
        }

        public Ray3f CameraRay { get; }
        public IReadOnlyPooledList<Plane3f> CameraPlanes { get; }
        public IReadOnlyPooledList<Vector3f> CameraFrustumCorners { get; }

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

        public Area1f RangeZ { get; }
        public Range2i InvalidatedArea { get; }
        public Vector2i CellInGroupCount { get; }
        public int MeshSubdivision { get; }
        public IImage2f Image { get; }
        public IMask2e Mask { get; }

        private void Dispose()
        {
            CameraPlanes.TryDispose();
            CameraFrustumCorners.TryDispose();
            (Image as IInitializableImage)?.FinishUsing();
        }
    }
}