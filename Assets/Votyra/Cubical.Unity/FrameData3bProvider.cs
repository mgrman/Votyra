using System.Collections.Generic;
using UnityEngine;
using Votyra.Core;
using Votyra.Core.Images;
using Votyra.Core.MeshUpdaters;
using Votyra.Core.Models;
using Votyra.Core.Pooling;
using Votyra.Core.Utils;
using Zenject;

namespace Votyra.Cubical
{
    //TODO: move to floats
    public class FrameData3bProvider : IFrameDataProvider<IFrameData3b>
    {
        [Inject]
        protected IImage3bProvider _imageProvider;

        [Inject]
        protected IMeshUpdater<Vector3i> _meshUpdater;

        [Inject(Id = "root")]
        protected GameObject _root;

        public IFrameData3b GetCurrentFrameData()
        {
            var camera = Camera.main;
            var container = _root.gameObject;

            var existingGroups = _meshUpdater.ExistingGroups;

            var image = _imageProvider.CreateImage();

            var localToProjection = camera.projectionMatrix * camera.worldToCameraMatrix * _root.transform.localToWorldMatrix;

            var planesUnity = PooledArrayContainer<Plane>.CreateDirty(6);
            GeometryUtility.CalculateFrustumPlanes(localToProjection, planesUnity.Array);
            IEnumerable<Plane3f> planes = planesUnity.ToPlane3f();

            var frustumCornersUnity = PooledArrayContainer<Vector3>.CreateDirty(4);
            camera.CalculateFrustumCorners(new Rect(0, 0, 1, 1), camera.farClipPlane, Camera.MonoOrStereoscopicEye.Mono, frustumCornersUnity.Array);
            IEnumerable<Vector3f> frustumCorners = frustumCornersUnity.ToVector3f();

            return new FrameData3b(
                camera.transform.position.ToVector3f(),
                planes,
                frustumCorners,
                camera.transform.localToWorldMatrix.ToMatrix4x4f(),
                container.transform.worldToLocalMatrix.ToMatrix4x4f(),
                existingGroups,
                image,
                (image as IImageInvalidatableImage3i)?.InvalidatedArea ?? Rect3i.Zero
            );
        }
    }
}