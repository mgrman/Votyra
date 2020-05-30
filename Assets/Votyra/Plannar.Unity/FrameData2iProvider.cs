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
    // TODO: move to floats
    public class FrameData2IProvider : IFrameDataProvider2I, ITickable
    {
        private readonly Vector3[] frustumCornersUnity = new Vector3[4];
        private readonly IImage2FPostProcessor image2FPostProcessor;
        private readonly IImage2FProvider imageProvider;
        private readonly IInterpolationConfig interpolationConfig;
        private readonly int meshTopologyDistance;

        private readonly Plane[] planesUnity = new Plane[6];

        private readonly IFrameData2IPool pool;
        private readonly GameObject root;
        private readonly ITerrainConfig terrainConfig;

        private byte frameDataSubscriberCount;

        private Matrix4X4F previousCameraMatrix;

        public event Action<ArcResource<IFrameData2I>> FrameData
        {
            add
            {
                this.RawFrameData += value;
                this.frameDataSubscriberCount++;

                var data = this.GetCurrentFrameData(false);
                if (data == null)
                {
                    return;
                }

                value.Invoke(data);
            }

            remove
            {
                this.RawFrameData -= value;
                this.frameDataSubscriberCount--;
            }
        }

        public void Tick()
        {
            var data = this.GetCurrentFrameData(true);
            if (data == null)
            {
                return;
            }

            for (var i = 1; i < this.frameDataSubscriberCount; i++)
            {
                data.Activate();
            }

            this.RawFrameData?.Invoke(data);
        }

        private ArcResource<IFrameData2I> GetCurrentFrameData(bool computedOnce)
        {
            var camera = CameraUtils.MainCamera;
            var image = this.imageProvider.CreateImage();

            var cameraLocalToWorldMatrix = camera.transform.localToWorldMatrix.ToMatrix4X4F();
            if (computedOnce && (cameraLocalToWorldMatrix == this.previousCameraMatrix) && ((image as IImageInvalidatableImage2)?.InvalidatedArea == Range2i.Zero))
            {
                return null;
            }

            this.previousCameraMatrix = cameraLocalToWorldMatrix;

            var frameDataContainer = this.pool.Get();
            var frameData = frameDataContainer.Value as IPoolableFrameData2I;
            var localToProjection = camera.projectionMatrix * camera.worldToCameraMatrix * this.root.transform.localToWorldMatrix;
            GeometryUtility.CalculateFrustumPlanes(localToProjection, this.planesUnity);
            for (var i = 0; i < 6; i++)
            {
                frameData.CameraPlanes[i] = this.planesUnity[i]
                    .ToPlane3F();
            }

            var container = this.root.gameObject;

            image = this.image2FPostProcessor?.PostProcess(image) ?? image;

            var parentContainerWorldToLocalMatrix = container.transform.worldToLocalMatrix.ToMatrix4X4F();

            camera.CalculateFrustumCorners(new Rect(0, 0, 1, 1), camera.farClipPlane, Camera.MonoOrStereoscopicEye.Mono, this.frustumCornersUnity);
            for (var i = 0; i < this.frustumCornersUnity.Length; i++)
            {
                frameData.CameraFrustumCorners[i] = parentContainerWorldToLocalMatrix.MultiplyPoint(cameraLocalToWorldMatrix.MultiplyVector(this.frustumCornersUnity[i]
                    .ToVector3F()));
            }

            var invalidatedArea = (image as IImageInvalidatableImage2)?.InvalidatedArea ?? Range2i.All;
            invalidatedArea = invalidatedArea.ExtendBothDirections(this.meshTopologyDistance);

            var cameraPosition = this.root.transform.InverseTransformPoint(camera.transform.position)
                .ToVector3F();
            var cameraDirection = this.root.transform.InverseTransformDirection(camera.transform.forward)
                .ToVector3F();

            frameData.CameraRay = new Ray3f(cameraPosition, cameraDirection);
            frameData.Image = image;
            frameData.InvalidatedArea = invalidatedArea;

            return frameDataContainer;
        }

#pragma warning disable SA1201
        private event Action<ArcResource<IFrameData2I>> RawFrameData;

        [Inject]
        public FrameData2IProvider([InjectOptional]
            IImage2FPostProcessor image2FPostProcessor, IImage2FProvider imageProvider, ITerrainConfig terrainConfig, IInterpolationConfig interpolationConfig, [Inject(Id = "root")]
            GameObject root, IFrameData2IPool pool)
        {
            this.image2FPostProcessor = image2FPostProcessor;
            this.imageProvider = imageProvider;
            this.terrainConfig = terrainConfig;
            this.interpolationConfig = interpolationConfig;
            this.root = root;
            this.pool = pool;
            this.meshTopologyDistance = (this.interpolationConfig.ActiveAlgorithm == IntepolationAlgorithm.Cubic) && (this.interpolationConfig.MeshSubdivision != 1) ? 2 : 1;
        }
#pragma warning restore SA1201
    }
}
