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

            var planesUnity = new Plane[6];
            GeometryUtility.CalculateFrustumPlanes(localToProjection, planesUnity);
            var planes = planesUnity.ToPlane3f();

            var cameraLocalToWorldMatrix = camera.transform.localToWorldMatrix.ToMatrix4x4f();
            var parentContainerWorldToLocalMatrix = _root.transform.worldToLocalMatrix.ToMatrix4x4f();

            var frustumCornersUnity = new Vector3[4];
            camera.CalculateFrustumCorners(new Rect(0, 0, 1, 1), camera.farClipPlane, Camera.MonoOrStereoscopicEye.Mono, frustumCornersUnity);
            var frustumCorners = new Vector3f[4];
            for (var i = 0; i < frustumCornersUnity.Length; i++)
            {
                frustumCorners[i] = parentContainerWorldToLocalMatrix.MultiplyPoint(cameraLocalToWorldMatrix.MultiplyVector(frustumCornersUnity[i].ToVector3f()));
            }

            var cameraPosition = _root.transform.InverseTransformPoint(camera.transform.position)
                .ToVector3f();
            var cameraDirection = _root.transform.InverseTransformDirection(camera.transform.forward)
                .ToVector3f();

            return new FrameData3b(new Ray3f(cameraPosition, cameraDirection), planes, frustumCorners, existingGroups, image, (image as IImageInvalidatableImage3)?.InvalidatedArea ?? Range3i.Zero);
        }
    }
}