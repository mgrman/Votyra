using System;
using System.Collections.Generic;
using UnityEngine;
using Votyra.Models;
using Votyra.Utils;
using Votyra.Images;
using Votyra.ImageSamplers;
using Votyra.TerrainAlgorithms;
using Votyra.TerrainGenerators;
using Votyra.TerrainGenerators.TerrainMeshers.TerrainMeshes;
using Votyra.Unity.Assets.Votyra.Pooling;
using Votyra.Unity.GroupSelectors;
using Votyra.Unity.MeshUpdaters;
using Votyra.Profiling;
using Votyra.Logging;

namespace Votyra.Cubical
{
    public class SceneContext3b : ITerrainGeneratorContext3b, IGroupVisibilityContext3i, IMeshContext, IDisposable
    {
        private const int MAX_CELL_COUNT = 60 * 60;

        public SceneContext3b(
            IGroupSelector3i groupSelector,
            ITerrainGenerator3b terrainGenerator,
            IMeshUpdater3i meshUpdater,
            Vector3 cameraPosition,
            IReadOnlyList<Plane> cameraPlanes,
            IReadOnlyPooledList<Vector3> cameraFrustumCorners,
            Matrix4x4 cameraLocalToWorldMatrix,
            Matrix4x4 parentContainerWorldToLocalMatrix,
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


        public Vector3 CameraPosition { get; }
        public IReadOnlyList<Plane> CameraPlanes { get; }
        public IReadOnlyPooledList<Vector3> CameraFrustumCorners { get; }
        public Matrix4x4 CameraLocalToWorldMatrix { get; }
        public Matrix4x4 ParentContainerWorldToLocalMatrix { get; }
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
            CameraFrustumCorners.Dispose();
            (Image as IInitializableImage)?.FinishUsing();
        }
    }
}
