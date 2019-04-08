using System;
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
    public class FrameData2iProvider : IFrameDataProvider2i, ITickable
    {
        private readonly IImage2fPostProcessor _image2fPostProcessor;
        private readonly IImage2fProvider _imageProvider;
        private readonly IInterpolationConfig _interpolationConfig;
        private readonly IMask2eProvider _maskProvider;
        private readonly int _meshTopologyDistance;
        private readonly GameObject _root;
        private readonly ITerrainConfig _terrainConfig;

        private Matrix4x4f _previousCameraMatrix;

        private IFrameData2iPool _pool;

        private readonly Plane[] _planesUnity = new Plane[6];
        private readonly Vector3[] _frustumCornersUnity = new Vector3[4];

        private byte _frameDataSubscriberCount = 0;

        private event Action<ArcResource<IFrameData2i>> _frameData;

        [Inject]
        public FrameData2iProvider([InjectOptional] IImage2fPostProcessor image2FPostProcessor, IImage2fProvider imageProvider, ITerrainConfig terrainConfig, IInterpolationConfig interpolationConfig, [InjectOptional] IMask2eProvider maskProvider, [Inject(Id = "root")] GameObject root, IFrameData2iPool pool)
        {
            _image2fPostProcessor = image2FPostProcessor;
            _imageProvider = imageProvider;
            _terrainConfig = terrainConfig;
            _interpolationConfig = interpolationConfig;
            _maskProvider = maskProvider;
            _root = root;
            _pool = pool;
            _meshTopologyDistance = _interpolationConfig.ActiveAlgorithm == IntepolationAlgorithm.Cubic && _interpolationConfig.MeshSubdivision != 1 ? 2 : 1;
        }

        public event Action<ArcResource<IFrameData2i>> FrameData
        {
            add
            {
                _frameData += value;
                _frameDataSubscriberCount++;

                var data = GetCurrentFrameData(false);
                if (data == null)
                    return;
                value.Invoke(data);
            }
            remove
            {
                _frameData -= value;
                _frameDataSubscriberCount--;
            }
        }

        public void Tick()
        {
            var data = GetCurrentFrameData(true);
            if (data == null)
                return;
            
            for (int i = 1; i < _frameDataSubscriberCount; i++)
            {
                data.Activate();
            }
            _frameData?.Invoke(data);
        }

        private ArcResource<IFrameData2i> GetCurrentFrameData(bool computedOnce)
        {
            var camera = CameraUtils.MainCamera;
            var image = _imageProvider.CreateImage();

            var cameraLocalToWorldMatrix = camera.transform.localToWorldMatrix.ToMatrix4x4f();
            if (computedOnce && cameraLocalToWorldMatrix == _previousCameraMatrix && (image as IImageInvalidatableImage2)?.InvalidatedArea == Range2i.Zero)
                return null;

            _previousCameraMatrix = cameraLocalToWorldMatrix;

            var frameDataContainer = _pool.Get();
            var frameData = frameDataContainer.Value as IPoolableFrameData2i;
            var localToProjection = camera.projectionMatrix * camera.worldToCameraMatrix * _root.transform.localToWorldMatrix;
            GeometryUtility.CalculateFrustumPlanes(localToProjection, _planesUnity);
            for (int i = 0; i < 6; i++)
            {
                frameData.CameraPlanes[i] = _planesUnity[i]
                    .ToPlane3f();
            }

            var container = _root.gameObject;

            image = _image2fPostProcessor?.PostProcess(image) ?? image;

            var mask = _maskProvider?.CreateMask();

            var parentContainerWorldToLocalMatrix = container.transform.worldToLocalMatrix.ToMatrix4x4f();

            camera.CalculateFrustumCorners(new Rect(0, 0, 1, 1), camera.farClipPlane, Camera.MonoOrStereoscopicEye.Mono, _frustumCornersUnity);
            for (var i = 0; i < _frustumCornersUnity.Length; i++)
            {
                frameData.CameraFrustumCorners[i] = parentContainerWorldToLocalMatrix.MultiplyPoint(cameraLocalToWorldMatrix.MultiplyVector(_frustumCornersUnity[i]
                    .ToVector3f()));
            }

            var invalidatedArea = ((image as IImageInvalidatableImage2)?.InvalidatedArea)?.UnionWith((mask as IImageInvalidatableImage2)?.InvalidatedArea) ?? Range2i.All;
            invalidatedArea = invalidatedArea.ExtendBothDirections(_meshTopologyDistance);

            var cameraPosition = _root.transform.InverseTransformPoint(camera.transform.position)
                .ToVector3f();
            var cameraDirection = _root.transform.InverseTransformDirection(camera.transform.forward)
                .ToVector3f();

            frameData.CameraRay = new Ray3f(cameraPosition, cameraDirection);
            frameData.Image = image;
            frameData.Mask = mask;
            frameData.InvalidatedArea = invalidatedArea;

            return frameDataContainer;
        }
    }
}