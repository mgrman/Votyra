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
        private readonly IImage2fPostProcessor _image2fPostProcessor;
        
        private readonly IImage2fProvider _imageProvider;
        
        private readonly IInterpolationConfig _interpolationConfig;
        
        private readonly IMask2eProvider _maskProvider;
        
        private readonly GameObject _root;
        
        private readonly ITerrainConfig _terrainConfig;

        private Matrix4x4 _previousCameraMatrix;

        [Inject]
        public FrameData2iProvider([InjectOptional] IImage2fPostProcessor image2FPostProcessor, IImage2fProvider imageProvider, ITerrainConfig terrainConfig, IInterpolationConfig interpolationConfig, [InjectOptional] IMask2eProvider maskProvider, [Inject(Id = "root")] GameObject root)
        {
            _image2fPostProcessor = image2FPostProcessor;
            _imageProvider = imageProvider;
            _terrainConfig = terrainConfig;
            _interpolationConfig = interpolationConfig;
            _maskProvider = maskProvider;
            _root = root;
        }

        public IFrameData2i GetCurrentFrameData(int meshTopologyDistance, bool computedOnce)
        {
            var camera = CameraUtils.MainCamera;
            var image = _imageProvider.CreateImage();

            var localToProjection = camera.projectionMatrix * camera.worldToCameraMatrix * _root.transform.localToWorldMatrix;
            var planesUnity = PooledArrayContainer<Plane>.CreateDirty(6);
            GeometryUtility.CalculateFrustumPlanes(localToProjection, planesUnity.Array);
            var planes = planesUnity.ToPlane3f();
            planesUnity.Dispose();

            foreach (var plane in planes)
            {
                var dirPerp= plane.Normal.XY.Perpendicular.Normalized * 100;
                var start = plane.Normal.XY.Normalized * plane.Distance;
                var end = start + dirPerp;
                start = end- dirPerp -dirPerp;
                Debug.DrawLine(new Vector3(start.X, 0,start.Y), new Vector3(end.X,0, end.Y));
            }

            var localToWorldMatrix = camera.transform.localToWorldMatrix;
            if (computedOnce && localToWorldMatrix == _previousCameraMatrix && (image as IImageInvalidatableImage2)?.InvalidatedArea == Range2i.Zero)
                return null;

            _previousCameraMatrix = localToWorldMatrix;

            var container = _root.gameObject;

            image = _image2fPostProcessor?.PostProcess(image) ?? image;

            var mask = _maskProvider?.CreateMask();



            var frustumCornersUnity = PooledArrayContainer<Vector3>.CreateDirty(4);
            camera.CalculateFrustumCorners(new Rect(0, 0, 1, 1), camera.farClipPlane, Camera.MonoOrStereoscopicEye.Mono, frustumCornersUnity.Array);
            var frustumCorners = frustumCornersUnity.ToVector3f();
            frustumCornersUnity.Dispose();

            var invalidatedArea = ((image as IImageInvalidatableImage2)?.InvalidatedArea)?.UnionWith((mask as IImageInvalidatableImage2)?.InvalidatedArea) ?? Range2i.All;
            invalidatedArea = invalidatedArea.ExtendBothDirections(meshTopologyDistance);

            return new FrameData2i(camera.transform.position.ToVector3f(), planes, frustumCorners, localToWorldMatrix.ToMatrix4x4f(), container.transform.worldToLocalMatrix.ToMatrix4x4f(), image, mask, invalidatedArea, _terrainConfig.CellInGroupCount.XY, _interpolationConfig.MeshSubdivision);
        }
    }
}