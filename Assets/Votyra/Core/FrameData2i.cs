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
            IMask2e mask,
            Range2i invalidatedArea,
            Vector2i cellInGroupCount, HashSet<Vector2i> skippedAreas)
        {
            CameraPosition = cameraPosition;
            CameraPlanes = cameraPlanes;
            CameraFrustumCorners = cameraFrustumCorners;
            CameraLocalToWorldMatrix = cameraLocalToWorldMatrix;
            ParentContainerWorldToLocalMatrix = parentContainerWorldToLocalMatrix;
            ExistingGroups = existingGroups;
            Image = image;
            Mask = mask;

            RangeZ = image?.RangeZ ?? Range1hf.Default;

            InvalidatedArea = invalidatedArea;
            CellInGroupCount = cellInGroupCount;
            SkippedAreas = skippedAreas;

            (Image as IInitializableImage)?.StartUsing();
        }

        public Vector3f CameraPosition { get; }
        public IEnumerable<Plane3f> CameraPlanes { get; }
        public IEnumerable<Vector3f> CameraFrustumCorners { get; }
        public Matrix4x4f CameraLocalToWorldMatrix { get; }
        public Matrix4x4f ParentContainerWorldToLocalMatrix { get; }
        public Range1hf RangeZ { get; }
        public IReadOnlySet<Vector2i> ExistingGroups { get; }
        public Vector2i CellInGroupCount { get; }
        public IImage2f Image { get; }
        public IMask2e Mask { get; }
        public Range2i InvalidatedArea { get; }
        public HashSet<Vector2i> SkippedAreas { get; }

        public void Dispose()
        {
            ExistingGroups.TryDispose();
            CameraPlanes.TryDispose();
            CameraFrustumCorners.TryDispose();
            (Image as IInitializableImage)?.FinishUsing();
        }
    }
}