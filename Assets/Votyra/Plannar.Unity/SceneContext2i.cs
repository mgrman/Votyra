using System;
using System.Collections.Generic;
using UnityEngine;
using Votyra.Core.Images;
using Votyra.Core.Logging;
using Votyra.Core.MeshUpdaters;
using Votyra.Core.Models;
using Votyra.Core.Pooling;
using Votyra.Core.Profiling;
using Votyra.Core.Utils;
using Votyra.Core.GroupSelectors;
using Votyra.Core.ImageSamplers;
using Votyra.Core.TerrainGenerators;

namespace Votyra.Plannar
{
    public class SceneContext2i : ITerrainGeneratorContext2i, IGroupVisibilityContext2i, IMeshContext, IDisposable
    {
        private const int MAX_CELL_COUNT = 60 * 60;

        public SceneContext2i(
            IGroupSelector2i groupSelector,
            ITerrainGenerator2i terrainGenerator,
            IMeshUpdater<Vector2i> meshUpdater,
            Vector3f cameraPosition,
            IEnumerable<Plane3f> cameraPlanes,
            IEnumerable<Vector3f> cameraFrustumCorners,
            Matrix4x4f cameraLocalToWorldMatrix,
            Matrix4x4f parentContainerWorldToLocalMatrix,
            IReadOnlySet<Vector2i> existingGroups,
            Vector2i cellInGroupCount,
            IImage2f image,
            Rect2i invalidatedArea_imageSpace,
            IImageSampler2i imageSampler,
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

            RangeZ = image?.RangeZ ?? Range2.Zero;

            GroupBounds = new Rect3f(new Vector3f(CellInGroupCount.x / 2.0f, CellInGroupCount.y / 2.0f, RangeZ.Center), new Vector3f(CellInGroupCount.x, CellInGroupCount.y, RangeZ.Size));
            InvalidatedArea_worldSpace = ImageSampler
                .ImageToWorld(invalidatedArea_imageSpace)
                .RoundToContain();

            (Image as IInitializableImage)?.StartUsing();
        }

        public IGroupSelector2i GroupSelector { get; }
        public ITerrainGenerator2i TerrainGenerator { get; }
        public IMeshUpdater<Vector2i> MeshUpdater { get; }

        public Vector3f CameraPosition { get; }
        public IEnumerable<Plane3f> CameraPlanes { get; }
        public IEnumerable<Vector3f> CameraFrustumCorners { get; }
        public Matrix4x4f CameraLocalToWorldMatrix { get; }
        public Matrix4x4f ParentContainerWorldToLocalMatrix { get; }
        public Rect3f GroupBounds { get; }
        public Range2 RangeZ { get; }
        public IReadOnlySet<Vector2i> ExistingGroups { get; }
        public Vector2i CellInGroupCount { get; }
        public IImage2f Image { get; }
        public Rect2i InvalidatedArea_worldSpace { get; }
        public IImageSampler2i ImageSampler { get; }
        public Func<GameObject> GameObjectFactory { get; }

        public ProfilerFactoryDelegate ProfilerFactory { get; }
        public LoggerFactoryDelegate LoggerFactory { get; }

        public void Dispose()
        {
            ExistingGroups.TryDispose();
            CameraPlanes.TryDispose();
            CameraFrustumCorners.TryDispose();
            (Image as IInitializableImage)?.FinishUsing();
        }
    }
}