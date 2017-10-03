using System;
using System.Collections.Generic;
using UnityEngine;
using Votyra.Core.Images;
using Votyra.Core.Logging;
using Votyra.Core.Models;
using Votyra.Core.Pooling;
using Votyra.Core.Profiling;
using Votyra.Core.Utils;
using Votyra.Plannar.GroupSelectors;
using Votyra.Plannar.ImageSamplers;
using Votyra.Plannar.MeshUpdaters;
using Votyra.Plannar.TerrainGenerators;

namespace Votyra.Plannar
{
    public class SceneContext2i : ITerrainGeneratorContext2i, IGroupVisibilityContext2i, IMeshContext, IDisposable
    {
        private const int MAX_CELL_COUNT = 60 * 60;

        public SceneContext2i(
            IGroupSelector2i groupSelector,
            ITerrainGenerator2i terrainGenerator,
            IMeshUpdater2i meshUpdater,
            Vector3 cameraPosition,
            IReadOnlyList<Plane> cameraPlanes,
            IReadOnlyPooledList<Vector3> cameraFrustumCorners,
            Matrix4x4 cameraLocalToWorldMatrix,
            Matrix4x4 parentContainerWorldToLocalMatrix,
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

            RangeZ = image.RangeZ;
            GroupBounds = new Bounds(new Vector3(CellInGroupCount.x / 2.0f, CellInGroupCount.y / 2.0f, RangeZ.Center), new Vector3(CellInGroupCount.x, CellInGroupCount.y, RangeZ.Size));
            InvalidatedArea_worldSpace = ImageSampler
                  .ImageToWorld(invalidatedArea_imageSpace)
                  .RoundToContain();

            (Image as IInitializableImage)?.StartUsing();
        }

        public IGroupSelector2i GroupSelector { get; }
        public ITerrainGenerator2i TerrainGenerator { get; }
        public IMeshUpdater2i MeshUpdater { get; }


        public Vector3 CameraPosition { get; }
        public IReadOnlyList<Plane> CameraPlanes { get; }
        public IReadOnlyPooledList<Vector3> CameraFrustumCorners { get; }
        public Matrix4x4 CameraLocalToWorldMatrix { get; }
        public Matrix4x4 ParentContainerWorldToLocalMatrix { get; }
        public Bounds GroupBounds { get; }
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
            CameraFrustumCorners.Dispose();
            (Image as IInitializableImage)?.FinishUsing();
        }
    }
}
