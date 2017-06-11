using System;
using System.Collections.Generic;
using UnityEngine;
using Votyra.Models;
using Votyra.Utils;
using Votyra.Images;
using Votyra.ImageSamplers;
using Votyra.TerrainAlgorithms;
using Votyra.TerrainMeshGenerators;
using Votyra.TerrainTileGenerators;
using Votyra.TerrainMeshers;
using Votyra.TerrainMeshers.TriangleMesh;
using Votyra.Unity.Assets.Votyra.Pooling;
using Votyra.Unity.GroupSelectors;
using Votyra.Unity.MeshUpdaters;

namespace Votyra.Unity
{
    public class SceneContext : ITerrainMeshContext, ITerrainTileContext, IGroupVisibilityContext, IMeshContext, IDisposable
    {
        private const int MAX_CELL_COUNT = 60 * 60;

        public SceneContext(
            IGroupSelector groupSelector,
            ITerrainMeshGenerator terrainMeshGenerator,
            ITerrainTileGenerator terrainTileGenerator,
            IMeshUpdater meshUpdater,
            Vector3 cameraPosition,
            IReadOnlyList<Plane> cameraPlanes,
            IReadOnlyPooledList<Vector3> cameraFrustumCorners,
            Matrix4x4 cameraLocalToWorldMatrix,
            Matrix4x4 parentContainerWorldToLocalMatrix,
            IReadOnlySet<Vector2i> existingGroups,
            Vector2i cellInGroupCount,
            IImage2i image,
            IImageSampler imageSampler,
            ITerrainAlgorithm terrainAlgorithm,
            Func<GameObject> gameObjectFactory)
        {
            GroupSelector = groupSelector;
            TerrainMeshGenerator = terrainMeshGenerator;
            TerrainTileGenerator = terrainTileGenerator;
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
            TerrainAlgorithm = terrainAlgorithm;
            GameObjectFactory = gameObjectFactory;

            RangeZ = image.RangeZ;
            GroupBounds = new Bounds(new Vector3(CellInGroupCount.x / 2.0f, CellInGroupCount.y / 2.0f, RangeZ.Center), new Vector3(CellInGroupCount.x, CellInGroupCount.y, RangeZ.Size));
            TransformedInvalidatedArea = ImageSampler
                  .ImageToWorld(image.InvalidatedArea)
                  .RoundToContain();

            (Image as IInitializableImage)?.StartUsing();
        }

        public IGroupSelector GroupSelector { get; }
        public ITerrainMeshGenerator TerrainMeshGenerator { get; }
        public ITerrainTileGenerator TerrainTileGenerator { get; }
        public IMeshUpdater MeshUpdater { get; }


        public Vector3 CameraPosition { get; }
        public IReadOnlyList<Plane> CameraPlanes { get; }
        public IReadOnlyPooledList<Vector3> CameraFrustumCorners { get; }
        public Matrix4x4 CameraLocalToWorldMatrix { get; }
        public Matrix4x4 ParentContainerWorldToLocalMatrix { get; }
        public Bounds GroupBounds { get; }
        public Range2i RangeZ { get; }
        public IReadOnlySet<Vector2i> ExistingGroups { get; }
        public Vector2i CellInGroupCount { get; }
        public IImage2i Image { get; }
        public Rect2i TransformedInvalidatedArea { get; }
        public IImageSampler ImageSampler { get; }
        public ITerrainAlgorithm TerrainAlgorithm { get; }
        public Func<GameObject> GameObjectFactory { get; }

        public void Dispose()
        {
            CameraFrustumCorners.Dispose();
            (Image as IInitializableImage)?.FinishUsing();
        }
    }
}