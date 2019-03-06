using UnityEngine;
using Votyra.Core;
using Votyra.Core.Images;
using Votyra.Core.Models;
using Votyra.Core.Pooling;
using Votyra.Core.Utils;
using Zenject;

namespace Votyra.Cubical
{
    //TODO: move to floats
    public class FrameData3bProvider : IFrameDataProvider3b
    {
        [Inject]
        protected IImage3bProvider _imageProvider;

        [Inject(Id = "root")]
        protected GameObject _root;

        public IFrameData3b GetCurrentFrameData(IReadOnlySet<Vector3i> existingGroups)
        {
            var camera = Camera.main;
            var image = _imageProvider.CreateImage();

            var localToProjection = camera.projectionMatrix * camera.worldToCameraMatrix * _root.transform.localToWorldMatrix;

            var planesUnity = PooledArrayContainer<Plane>.CreateDirty(6);
            GeometryUtility.CalculateFrustumPlanes(localToProjection, planesUnity.Array);
            var planes = planesUnity.ToPlane3f();

            var cameraLocalToWorldMatrix = camera.transform.localToWorldMatrix.ToMatrix4x4f();
            var parentContainerWorldToLocalMatrix = _root.transform.worldToLocalMatrix.ToMatrix4x4f();

            var frustumCornersUnity = PooledArrayContainer<Vector3>.CreateDirty(4);
            camera.CalculateFrustumCorners(new Rect(0, 0, 1, 1), camera.farClipPlane, Camera.MonoOrStereoscopicEye.Mono, frustumCornersUnity.Array);
            var frustumCorners = frustumCornersUnity.ToVector3f();
            for (var i = 0; i < frustumCorners.Array.Length; i++)
            {
                frustumCorners.Array[i] = parentContainerWorldToLocalMatrix.MultiplyPoint(cameraLocalToWorldMatrix.MultiplyVector(frustumCorners.Array[i]));
            }

            var cameraPosition = _root.transform.InverseTransformPoint(camera.transform.position)
                .ToVector3f();
            var cameraDirection = _root.transform.InverseTransformDirection(camera.transform.forward)
                .ToVector3f();


            return new FrameData3b(new Ray3f(cameraPosition, cameraDirection), planes, frustumCorners, existingGroups, image, (image as IImageInvalidatableImage3)?.InvalidatedArea ?? Range3i.Zero);
        }
    }
}