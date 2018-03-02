using System;
using System.Collections.Generic;
using UnityEngine;
using Votyra.Core.Images;
using Votyra.Core.Logging;
using Votyra.Core.Models;
using Votyra.Core.Pooling;
using Votyra.Core.Profiling;
using Votyra.Core.Utils;
using Votyra.Cubical.GroupSelectors;
using Votyra.Cubical.ImageSamplers;
using Votyra.Cubical.MeshUpdaters;
using Votyra.Cubical.TerrainGenerators;

namespace Votyra.Cubical
{
    public class SceneContext3b : ITerrainGeneratorContext3b, IGroupVisibilityContext3i, IMeshContext, IDisposable
    {
        private const int MAX_CELL_COUNT = 60 * 60;

        public SceneContext3b(
            IGroupSelector3i groupSelector,
            ITerrainGenerator3b terrainGenerator,
            IMeshUpdater3i meshUpdater,
            Vector3f cameraPosition,
            IEnumerable<Plane3f> cameraPlanes,
            IEnumerable<Vector3f> cameraFrustumCorners,
            Matrix4x4f cameraLocalToWorldMatrix,
            Matrix4x4f parentContainerWorldToLocalMatrix,
            IReadOnlySet<Vector3i> existingGroups,
            Vector3i cellInGroupCount,
            IImage3f image,
            Rect3i? invalidatedArea_imageSpace,
            IImageSampler3b imageSampler,
            Func<GameObject> gameObjectFactory,
            ProfilerFactoryDelegate profilerFactory,
            LoggerFactoryDelegate loggerFactory)
        {
            GroupSelector = groupSelector;
            TerrainGenerator = terrainGenerator;
            MeshUpdater = meshUpdater;
            CameraPosition = cameraPosition;
            CameraPlanes = cameraPlanes;
            CameraFrustumCorners = cameraFrustumCorners;
            CameraLocalToWorldMatrix = cameraLocalToWorldMatrix;
            ParentContainerWorldToLocalMatrix = parentContainerWorldToLocalMatrix;
            ExistingGroups = existingGroups;
            CellInGroupCount = cellInGroupCount;
            Image = image;
            ImageSampler = imageSampler;
            GameObjectFactory = gameObjectFactory;
            ProfilerFactory = profilerFactory;
            LoggerFactory = loggerFactory;

            if (invalidatedArea_imageSpace.HasValue)
            {
                InvalidatedArea_worldSpace = ImageSampler
                      .ImageToWorld(invalidatedArea_imageSpace.Value)
                      .RoundToContain();
            }
            else
            {
                InvalidatedArea_worldSpace = null;
            }

            (Image as IInitializableImage)?.StartUsing();
        }

        public IGroupSelector3i GroupSelector { get; }
        public ITerrainGenerator3b TerrainGenerator { get; }
        public IMeshUpdater3i MeshUpdater { get; }

        public Vector3f CameraPosition { get; }
        public IEnumerable<Plane3f> CameraPlanes { get; }
        public IEnumerable<Vector3f> CameraFrustumCorners { get; }
        public Matrix4x4f CameraLocalToWorldMatrix { get; }
        public Matrix4x4f ParentContainerWorldToLocalMatrix { get; }
        public IReadOnlySet<Vector3i> ExistingGroups { get; }
        public Vector3i CellInGroupCount { get; }
        public IImage3f Image { get; }
        public Rect3i? InvalidatedArea_worldSpace { get; }
        public IImageSampler3b ImageSampler { get; }
        public Func<GameObject> GameObjectFactory { get; }

        public ProfilerFactoryDelegate ProfilerFactory { get; }
        public LoggerFactoryDelegate LoggerFactory { get; }

        public void Dispose()
        {
            CameraPlanes.TryDispose();
            CameraFrustumCorners.TryDispose();
            (Image as IInitializableImage)?.FinishUsing();
        }
    }
}