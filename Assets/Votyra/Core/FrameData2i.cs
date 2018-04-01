using System;
using System.Collections.Generic;
using Votyra.Core.Images;
using Votyra.Core.Models;
using Votyra.Core.Utils;

namespace Votyra.Core
{
    public class FrameData2i : IFrameData2i, IDisposable
    {
        public FrameData2i(
            Vector3f cameraPosition,
            IEnumerable<Plane3f> cameraPlanes,
            IEnumerable<Vector3f> cameraFrustumCorners,
            Matrix4x4f cameraLocalToWorldMatrix,
            Matrix4x4f parentContainerWorldToLocalMatrix,
            IReadOnlySet<Vector2i> existingGroups,
            IImage2f image,
            Rect2i invalidatedArea_imageSpace)
        {
            CameraPosition = cameraPosition;
            CameraPlanes = cameraPlanes;
            CameraFrustumCorners = cameraFrustumCorners;
            CameraLocalToWorldMatrix = cameraLocalToWorldMatrix;
            ParentContainerWorldToLocalMatrix = parentContainerWorldToLocalMatrix;
            ExistingGroups = existingGroups;
            Image = image;

            RangeZ = image?.RangeZ ?? Range2.Zero;

            InvalidatedArea_imageSpace = invalidatedArea_imageSpace;

            (Image as IInitializableImage)?.StartUsing();
        }

        public Vector3f CameraPosition { get; }
        public IEnumerable<Plane3f> CameraPlanes { get; }
        public IEnumerable<Vector3f> CameraFrustumCorners { get; }
        public Matrix4x4f CameraLocalToWorldMatrix { get; }
        public Matrix4x4f ParentContainerWorldToLocalMatrix { get; }
        public Range2 RangeZ { get; }
        public IReadOnlySet<Vector2i> ExistingGroups { get; }
        public IImage2f Image { get; }
        public Rect2i InvalidatedArea_imageSpace { get; }

        public void Dispose()
        {
            ExistingGroups.TryDispose();
            CameraPlanes.TryDispose();
            CameraFrustumCorners.TryDispose();
            (Image as IInitializableImage)?.FinishUsing();
        }
    }
}