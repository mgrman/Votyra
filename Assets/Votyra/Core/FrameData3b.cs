using System;
using System.Collections.Generic;
using Votyra.Core.Images;
using Votyra.Core.Models;
using Votyra.Core.Utils;

namespace Votyra.Core
{
    public class FrameData3b : IFrameData3b
    {
        private int _activeCounter;

        public FrameData3b(
            Vector3f cameraPosition,
            Plane3f[] cameraPlanes,
            Vector3f[] cameraFrustumCorners,
            Matrix4x4f cameraLocalToWorldMatrix,
            Matrix4x4f parentContainerWorldToLocalMatrix,
            IReadOnlySet<Vector3i> existingGroups,
            IImage3b image,
            Range3i invalidatedArea_imageSpace)
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
        public Plane3f[] CameraPlanes { get; }
        public Vector3f[] CameraFrustumCorners { get; }
        public Matrix4x4f CameraLocalToWorldMatrix { get; }
        public Matrix4x4f ParentContainerWorldToLocalMatrix { get; }

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
            {
                Dispose();
            }
        }
        
        private void Dispose()
        {
            CameraPlanes.TryDispose();
            CameraFrustumCorners.TryDispose();
            (Image as IInitializableImage)?.FinishUsing();
        }
    }
}