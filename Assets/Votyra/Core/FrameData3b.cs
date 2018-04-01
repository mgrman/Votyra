using System;
using System.Collections.Generic;
using Votyra.Core.Images;
using Votyra.Core.Models;
using Votyra.Core.Utils;

namespace Votyra.Core
{
    public class FrameData3b : IFrameData3b, IDisposable
    {
        public FrameData3b(
            Vector3f cameraPosition,
            IEnumerable<Plane3f> cameraPlanes,
            IEnumerable<Vector3f> cameraFrustumCorners,
            Matrix4x4f cameraLocalToWorldMatrix,
            Matrix4x4f parentContainerWorldToLocalMatrix,
            IReadOnlySet<Vector3i> existingGroups,
            IImage3b image,
            Rect3i invalidatedArea_imageSpace)
        {
            CameraPosition = cameraPosition;
            CameraPlanes = cameraPlanes;
            CameraFrustumCorners = cameraFrustumCorners;
            CameraLocalToWorldMatrix = cameraLocalToWorldMatrix;
            ParentContainerWorldToLocalMatrix = parentContainerWorldToLocalMatrix;
            ExistingGroups = existingGroups;
            Image = image;

            InvalidatedArea_imageSpace = invalidatedArea_imageSpace;

            (Image as IInitializableImage)?.StartUsing();
        }

        public Vector3f CameraPosition { get; }
        public IEnumerable<Plane3f> CameraPlanes { get; }
        public IEnumerable<Vector3f> CameraFrustumCorners { get; }
        public Matrix4x4f CameraLocalToWorldMatrix { get; }
        public Matrix4x4f ParentContainerWorldToLocalMatrix { get; }

        public IReadOnlySet<Vector3i> ExistingGroups { get; }
        public IImage3b Image { get; }
        public Rect3i InvalidatedArea_imageSpace { get; }

        public void Dispose()
        {
            CameraPlanes.TryDispose();
            CameraFrustumCorners.TryDispose();
            (Image as IInitializableImage)?.FinishUsing();
        }
    }
}