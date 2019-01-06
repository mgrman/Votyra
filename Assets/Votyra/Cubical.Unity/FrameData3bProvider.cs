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
            var container = _root.gameObject;


            var image = _imageProvider.CreateImage();

            var localToProjection = camera.projectionMatrix * camera.worldToCameraMatrix * _root.transform.localToWorldMatrix;

            var planesUnity = PooledArrayContainer<Plane>.CreateDirty(6);
            GeometryUtility.CalculateFrustumPlanes(localToProjection, planesUnity.Array);
            var planes = planesUnity.ToPlane3f();

            var frustumCornersUnity = PooledArrayContainer<Vector3>.CreateDirty(4);
            camera.CalculateFrustumCorners(new Rect(0, 0, 1, 1), camera.farClipPlane, Camera.MonoOrStereoscopicEye.Mono, frustumCornersUnity.Array);
            var frustumCorners = frustumCornersUnity.ToVector3f();

            return new FrameData3b(camera.transform.position.ToVector3f(), planes, frustumCorners, camera.transform.localToWorldMatrix.ToMatrix4x4f(), container.transform.worldToLocalMatrix.ToMatrix4x4f(), existingGroups, image, (image as IImageInvalidatableImage3)?.InvalidatedArea ?? Range3i.Zero);
        }
    }
}