using Votyra.Core.Images;
using Votyra.Core.Models;
using Votyra.Core.Pooling;
using Votyra.Core.Utils;

namespace Votyra.Core
{
    public class FrameData2i : IFrameData2i
    {
        private int _activeCounter;

        public FrameData2i(Vector3f cameraPosition, Plane3f[] cameraPlanes_pooled, Vector3f[] cameraFrustumCorners_pooled, Matrix4x4f cameraLocalToWorldMatrix, Matrix4x4f parentContainerWorldToLocalMatrix, IImage2f image, Range2i invalidatedArea, Vector2i cellInGroupCount, int meshSubdivision)
        {
            CameraPosition = cameraPosition;
            CameraPlanes = cameraPlanes_pooled;
            CameraFrustumCorners = cameraFrustumCorners_pooled;
            CameraLocalToWorldMatrix = cameraLocalToWorldMatrix;
            ParentContainerWorldToLocalMatrix = parentContainerWorldToLocalMatrix;
            Image = image;

            RangeZ = image?.RangeZ ?? Area1f.Zero;

            InvalidatedArea = invalidatedArea;
            CellInGroupCount = cellInGroupCount;
            MeshSubdivision = meshSubdivision;

            (Image as IInitializableImage)?.StartUsing();
        }

        public Vector3f CameraPosition { get; }
        
        public Plane3f[] CameraPlanes { get; }
        
        public Vector3f[] CameraFrustumCorners { get; }
        
        public Matrix4x4f CameraLocalToWorldMatrix { get; }
        
        public Matrix4x4f ParentContainerWorldToLocalMatrix { get; }

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

        private void Dispose()
        {
            PooledArrayContainer<Plane3f>.Return(CameraPlanes);
            PooledArrayContainer<Vector3f>.Return(CameraFrustumCorners);
            CameraPlanes.TryDispose();
            CameraFrustumCorners.TryDispose();
            (Image as IInitializableImage)?.FinishUsing();
        }
    }
}