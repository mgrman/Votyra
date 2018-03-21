using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Votyra.Core.Behaviours;
using Votyra.Core.GroupSelectors;
using Votyra.Core.Images;
using Votyra.Core.ImageSamplers;
using Votyra.Core.Logging;
using Votyra.Core.MeshUpdaters;
using Votyra.Core.Models;
using Votyra.Core.Pooling;
using Votyra.Core.Profiling;
using Votyra.Core.TerrainGenerators;
using Votyra.Core.TerrainMeshes;
using Votyra.Core.Utils;
using Votyra.Core.TerrainGenerators.TerrainMeshers;
using Zenject;
using Votyra.Core;

namespace Votyra.Cubical
{
    //TODO: move to floats
    public class TerrainGeneratorManager3b : IDisposable
    {
        public IEditableImage2f EditableImage { get { return _imageProvider as IEditableImage2f; } }

        [Inject]
        protected ITerrainConfig _terrainConfig;

        [Inject]
        protected IImage3fProvider _imageProvider;

        [Inject]
        protected IImageSampler3b _sampler;

        [Inject]
        protected IGroupSelector3i _groupsSelector;

        [Inject]
        protected ITerrainGenerator3b _terrainGenerator;

        [Inject]
        protected IMeshUpdater<Vector3i> _meshUpdater;

        [Inject(Id = "root")]
        protected GameObject _root;


        private Task _updateTask = null;
        private CancellationTokenSource _onDestroyCts = new CancellationTokenSource();

        [Inject]
        public void Initialize()
        {
            StartUpdateing();
        }

        private async void StartUpdateing()
        {
            while (!_onDestroyCts.IsCancellationRequested)
            {
                try
                {
                    if (_root.activeInHierarchy)
                    {
                        var context = GetSceneContext();
                        await UpdateTerrain(context, _terrainConfig.Async, _onDestroyCts.Token);
                    }
                    else
                    {
                        await Task.Delay(100);
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            }
        }

        private async Task UpdateTerrain(SceneContext3b context, bool async, CancellationToken token)
        {
            //Debug.Log($"UpdateTerrain");
            GroupActions3i groupActions = null;
            IReadOnlyPooledDictionary<Vector3i, ITerrainMesh> results = null;
            try
            {
                Func<IReadOnlyPooledDictionary<Vector3i, ITerrainMesh>> computeAction = () =>
                {
                    using (context.ProfilerFactory.Create("Creating visible groups"))
                    {
                        groupActions = context.GroupSelector.GetGroupsToUpdate(context);
                        if (groupActions.ToRecompute.Any())
                        {
                            Debug.Log($"Groups to recompute {groupActions.ToRecompute.Count()}. Groups to keep {groupActions.ToKeep.Count()}.");
                        }
                    }
                    var toRecompute = groupActions.ToRecompute;
                    if (toRecompute.Any())
                    {
                        using (context.ProfilerFactory.Create("TerrainMeshGenerator"))
                        {
                            return context.TerrainGenerator.Generate(context, toRecompute);
                        }
                    }
                    else
                    {
                        return null;
                    }
                };

                if (async)
                {
                    results = await Task.Run(computeAction);
                }
                else
                {
                    results = computeAction();
                }

                if (token.IsCancellationRequested)
                {
                    return;
                }

                if (results != null)
                {
                    using (context.ProfilerFactory.Create("Applying mesh"))
                    {
                        var toKeep = groupActions.ToKeep;
                        context.MeshUpdater.UpdateMesh(context, results, toKeep);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
            finally
            {
                context?.Dispose();
                groupActions?.Dispose();
                results?.Dispose();
            }
        }

        private SceneContext3b GetSceneContext()
        {
            var camera = Camera.main;
            var container = _root.gameObject;

            var existingGroups = _meshUpdater.ExistingGroups;

            var image = _imageProvider.CreateImage();

            Vector3i cellInGroupCount = _terrainConfig.CellInGroupCount;

            var localToProjection = camera.projectionMatrix * camera.worldToCameraMatrix * _root.transform.localToWorldMatrix;

            var planesUnity = PooledArrayContainer<Plane>.CreateDirty(6);
            GeometryUtility.CalculateFrustumPlanes(localToProjection, planesUnity.Array);
            IEnumerable<Plane3f> planes = planesUnity.ToPlane3f();

            var frustumCornersUnity = PooledArrayContainer<Vector3>.CreateDirty(4);
            camera.CalculateFrustumCorners(new Rect(0, 0, 1, 1), camera.farClipPlane, Camera.MonoOrStereoscopicEye.Mono, frustumCornersUnity.Array);
            IEnumerable<Vector3f> frustumCorners = frustumCornersUnity.ToVector3f();

            return new SceneContext3b(
                _groupsSelector,
                _terrainGenerator,
                _meshUpdater,
                camera.transform.position.ToVector3f(),
                planes,
                frustumCorners,
                camera.transform.localToWorldMatrix.ToMatrix4x4f(),
                container.transform.worldToLocalMatrix.ToMatrix4x4f(),
                existingGroups,
                cellInGroupCount,
                image,
                (image as IImageInvalidatableImage3i)?.InvalidatedArea ?? Rect3i.zero,
                _sampler,
                () => this.GameObjectFactory(),
                CreateProfiler,
                CreateLogger
            );
        }

        private GameObject GameObjectFactory()
        {
            var go = new GameObject();
            go.transform.SetParent(_root.transform, false);
            if (_terrainConfig.DrawBounds)
            {
                go.AddComponent<DrawBounds>();
            }
            var meshRenderer = go.GetOrAddComponent<MeshRenderer>();
            meshRenderer.materials = ArrayUtils.CreateNonNull(_terrainConfig.Material, _terrainConfig.MaterialWalls);
            return go;
        }

        private static IThreadSafeLogger CreateLogger(string name, object owner)
        {
            return new UnityLogger(name, owner);
        }

        private IProfiler CreateProfiler(string name, object owner)
        {
            return new UnityProfiler(name, owner);
        }

        // private void Initialize()
        // {
        //     // _editableImageConstraint = new TycoonTileConstraint2i();
        //     _sampler = new SimpleImageSampler3b();

        //     _terrainGenerator = new TerrainGenerator3b<TerrainMesher3b>();
        //     _meshUpdater = new TerrainMeshUpdater<Vector3i>();
        //     _groupsSelector = new GroupsByCameraVisibilitySelector3i();
        //     //_image = new NoiseImage3b(Vector3.zero, new Vector3(100, 100, 100));//new EditableMatrixImagei(InitialTexture, InitialTextureScale, _sampler, _editableImageConstraint);

        //     int width = InitialTexture.width;
        //     int height = InitialTexture.height;

        //     var size = new Vector2i(width, height);
        //     var matrix = new LockableMatrix<float>(size);

        //     for (int x = 0; x < width; x++)
        //     {
        //         for (int y = 0; y < height; y++)
        //         {
        //             matrix[x, y] = (InitialTexture.GetPixel(x, y).grayscale * InitialTextureScale);
        //         }
        //     }
        //     _image = new UmbraImage3f(new MatrixImage2f(matrix, Rect2i.All));
        // }


        public void Dispose()
        {
            _onDestroyCts.Cancel();
        }
    }
}