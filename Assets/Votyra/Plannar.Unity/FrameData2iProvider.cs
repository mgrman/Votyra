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
        
        private readonly GameObject _root;
        
        private readonly ITerrainConfig _terrainConfig;

        private Matrix4x4 _previousCameraMatrix;

        [Inject]
        public FrameData2iProvider([InjectOptional] IImage2fPostProcessor image2FPostProcessor, IImage2fProvider imageProvider, ITerrainConfig terrainConfig, IInterpolationConfig interpolationConfig, [Inject(Id = "root")] GameObject root)
        {
            _image2fPostProcessor = image2FPostProcessor;
            _imageProvider = imageProvider;
            _terrainConfig = terrainConfig;
            _interpolationConfig = interpolationConfig;
            _root = root;
        }

        public IFrameData2i GetCurrentFrameData(int meshTopologyDistance, bool computedOnce)
        {
            var camera = CameraUtils.MainCamera;
            var image = _imageProvider.CreateImage();

            var localToProjection = camera.projectionMatrix * camera.worldToCameraMatrix * _root.transform.localToWorldMatrix;
            var planesUnity = PooledArrayContainer<Plane>.CreateDirty(6);
            var planes = PooledArrayContainer<Plane3f>.CreateDirty(planesUnity.Length);
            GeometryUtility.CalculateFrustumPlanes(localToProjection, planesUnity);
            planesUnity.ToPlane3f(planes);
            PooledArrayContainer<Plane>.Return(planesUnity);

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
            {
                PooledArrayContainer<Plane3f>.Return(planes);
                return null;
            }

            _previousCameraMatrix = localToWorldMatrix;

            var container = _root.gameObject;

            image = _image2fPostProcessor?.PostProcess(image) ?? image;

            var frustumCornersUnity = PooledArrayContainer<Vector3>.CreateDirty(4);
            var frustumCorners = PooledArrayContainer<Vector3f>.CreateDirty(planesUnity.Length);
            camera.CalculateFrustumCorners(new Rect(0, 0, 1, 1), camera.farClipPlane, Camera.MonoOrStereoscopicEye.Mono, frustumCornersUnity);
            frustumCornersUnity.ToVector3f(frustumCorners);
            PooledArrayContainer<Vector3>.Return(frustumCornersUnity);

            var invalidatedArea = ((image as IImageInvalidatableImage2)?.InvalidatedArea) ?? Range2i.All;
            invalidatedArea = invalidatedArea.ExtendBothDirections(meshTopologyDistance);

            return new FrameData2i(camera.transform.position.ToVector3f(), planes, frustumCorners, localToWorldMatrix.ToMatrix4x4f(), container.transform.worldToLocalMatrix.ToMatrix4x4f(), image, invalidatedArea, _terrainConfig.CellInGroupCount.XY, _interpolationConfig.MeshSubdivision);
        }
    }
}