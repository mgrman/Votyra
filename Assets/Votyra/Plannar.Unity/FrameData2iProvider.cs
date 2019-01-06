using UnityEngine;
using Votyra.Core;
using Votyra.Core.Images;
using Votyra.Core.Models;
using Votyra.Core.Pooling;
using Votyra.Core.Utils;
using Zenject;

namespace Votyra.Plannar
{
    //TODO: move to floats
    public class FrameData2iProvider : IFrameDataProvider2i
    {
        [InjectOptional]
        private readonly IImage2fPostProcessor _image2fPostProcessor;

        [Inject]
        protected IImage2fProvider _imageProvider;

        [InjectOptional]
        protected IMask2eProvider _maskProvider;

        [Inject(Id = "root")]
        protected GameObject _root;

        public IFrameData2i GetCurrentFrameData(int meshTopologyDistance)
        {
            var camera = Camera.main;
            var container = _root.gameObject;


            var image = _imageProvider.CreateImage();
            image = _image2fPostProcessor?.PostProcess(image) ?? image;

            var mask = _maskProvider?.CreateMask();

            var localToProjection = camera.projectionMatrix * camera.worldToCameraMatrix * _root.transform.localToWorldMatrix;

            var planesUnity = PooledArrayContainer<Plane>.CreateDirty(6);
            GeometryUtility.CalculateFrustumPlanes(localToProjection, planesUnity.Array);
            var planes = planesUnity.ToPlane3f();
            planesUnity.Dispose();

            var frustumCornersUnity = PooledArrayContainer<Vector3>.CreateDirty(4);
            camera.CalculateFrustumCorners(new Rect(0, 0, 1, 1), camera.farClipPlane, Camera.MonoOrStereoscopicEye.Mono, frustumCornersUnity.Array);
            var frustumCorners = frustumCornersUnity.ToVector3f();
            frustumCornersUnity.Dispose();

            var invalidatedArea = ((image as IImageInvalidatableImage2)?.InvalidatedArea)?.UnionWith((mask as IImageInvalidatableImage2)?.InvalidatedArea) ?? Range2i.All;
            invalidatedArea = invalidatedArea.ExtendBothDirections(meshTopologyDistance);

            return new FrameData2i(camera.transform.position.ToVector3f(), planes, frustumCorners, camera.transform.localToWorldMatrix.ToMatrix4x4f(), container.transform.worldToLocalMatrix.ToMatrix4x4f(), image, mask, invalidatedArea);
        }
    }
}