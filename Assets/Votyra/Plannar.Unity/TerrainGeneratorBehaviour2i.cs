using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Votyra.Core.Behaviours;
using Votyra.Core.Images;
using Votyra.Core.Logging;
using Votyra.Core.MeshUpdaters;
using Votyra.Core.Models;
using Votyra.Core.Pooling;
using Votyra.Core.Profiling;
using Votyra.Core.TerrainMeshes;
using Votyra.Core.Utils;
using Votyra.Plannar.GroupSelectors;
using Votyra.Plannar.Images;
using Votyra.Plannar.Images.Constraints;
using Votyra.Plannar.ImageSamplers;
using Votyra.Plannar.TerrainGenerators;
using Zenject;

namespace Votyra.Plannar
{
    //TODO: move to floats
    public class TerrainGeneratorBehaviour2i
    {
        public IEditableImage2f EditableImage { get { return _imageProvider as IEditableImage2f; } }

        [Inject]
        protected ITerrainConfig _terrainConfig;

        [Inject]
        protected IImage2fProvider _imageProvider;

        [Inject]
        protected IImageSampler2i _sampler;

        [Inject]
        protected IGroupSelector2i _groupsSelector;

        [Inject]
        protected ITerrainGenerator2i _terrainGenerator;

        [Inject]
        protected IMeshUpdater<Vector2i> _meshUpdater;

        [Inject(Id = "root")]
        protected GameObject _root;

        private CancellationTokenSource _onDestroyCts = new CancellationTokenSource();

        [Inject]
        public void Initialize()
        {
            Update();
        }

        private async void Update()
        {
            Debug.Log("Updateing...");

            while (!_onDestroyCts.IsCancellationRequested)
            {

                var context = GetSceneContext();
                await UpdateTerrain(context, _terrainConfig.Async, _onDestroyCts.Token);
                await Task.Delay(100);
            }
        }

        private async Task UpdateTerrain(SceneContext2i context, bool async, CancellationToken token)
        {
            GroupActions2i groupActions = null;
            IReadOnlyPooledDictionary<Vector2i, ITerrainMesh> results = null;
            try
            {
                Func<IReadOnlyPooledDictionary<Vector2i, ITerrainMesh>> computeAction = () =>
                {
                    using (context.ProfilerFactory.Create("Creating visible groups"))
                    {
                        groupActions = context.GroupSelector.GetGroupsToUpdate(context);
                        //Debug.Log($"update {groupActions.ToRecompute.Count} keep {groupActions.ToKeep.Count}");
                    }
                    var toRecompute = groupActions?.ToRecompute ?? Enumerable.Empty<Vector2i>();
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
                        var toKeep = groupActions?.ToKeep ?? ReadOnlySet<Vector2i>.Empty;
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

        private SceneContext2i GetSceneContext()
        {
            var camera = Camera.main;
            var container = _root.gameObject;

            var existingGroups = _meshUpdater.ExistingGroups;

            var image = _imageProvider.CreateImage();
            Vector2i cellInGroupCount = _terrainConfig.CellInGroupCount;

            var localToProjection = camera.projectionMatrix * camera.worldToCameraMatrix * _root.transform.localToWorldMatrix;

            var planesUnity = PooledArrayContainer<Plane>.CreateDirty(6);
            GeometryUtility.CalculateFrustumPlanes(localToProjection, planesUnity.Array);
            IEnumerable<Plane3f> planes = planesUnity.ToPlane3f();

            var frustumCornersUnity = PooledArrayContainer<Vector3>.CreateDirty(4);
            camera.CalculateFrustumCorners(new Rect(0, 0, 1, 1), camera.farClipPlane, Camera.MonoOrStereoscopicEye.Mono, frustumCornersUnity.Array);
            IEnumerable<Vector3f> frustumCorners = frustumCornersUnity.ToVector3f();

            return new SceneContext2i(
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
                (image as IImageInvalidatableImage2i)?.InvalidatedArea ?? Rect2i.All,
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
            meshRenderer.material = _terrainConfig.Material;
            meshRenderer.materials = new Material[] { _terrainConfig.Material, _terrainConfig.MaterialWalls };
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

        public void Dispose()
        {
            _onDestroyCts.Cancel();
        }
    }
}