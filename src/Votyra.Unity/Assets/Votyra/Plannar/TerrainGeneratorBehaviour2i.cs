using System;
using System.Collections.Generic;
using System.Threading;
using Votyra.Models;
using Votyra.Utils;
using Votyra.Images;
using Votyra.ImageSamplers;
using Votyra.TerrainAlgorithms;
using Votyra.TerrainGenerators;
using Votyra.TerrainGenerators.TerrainMeshers.TerrainMeshes;
using Votyra.Unity.Behaviours;
using Votyra.Unity.GroupSelectors;
using Votyra.Unity.Images;
using Votyra.Unity.Logging;
using Votyra.Unity.MeshUpdaters;
using Votyra.Unity.Models;
using Votyra.Unity.Profiling;
using Votyra.Unity.Utils;
using UnityEngine;
using UnityEngine.Profiling;
using System.Linq;
using System.Threading.Tasks;
using Votyra.Profiling;
using Votyra.Unity.Assets.Votyra.Pooling;
using Votyra.Images.EditableImages;
using Votyra.Logging;
using Votyra.TerrainGenerators.TerrainMeshers;

namespace Votyra.Plannar
{

    //TODO: move to floats
    public class TerrainGeneratorBehaviour2i : MonoBehaviour
    {
        public UI_Vector2i CellInGroupCount = new UI_Vector2i(10, 10);
        public bool FlipTriangles = false;
        public bool DrawBounds = false;
        public bool Async = true;
        public Material Material = null;
        public Material MaterialWalls = null;
        public Texture2D InitialTexture = null;
        public float InitialTextureScale = 1;

        public IEditableImage2f EditableImage { get { return _imageProvider as IEditableImage2f; } }

        private IImage2fProvider _imageProvider;
        private IImageConstraint2i _editableImageConstraint;
        private IImageSampler2i _sampler;
        private IGroupSelector2i _groupsSelector;
        private ITerrainGenerator2i _terrainGenerator;
        private IMeshUpdater2i _meshUpdater;

        private Task _updateTask = null;
        private CancellationTokenSource _onDestroyCts = new CancellationTokenSource();

        private void Start()
        {
            this.gameObject.DestroyAllChildren();
            Initialize();
        }

        private void LateUpdate()
        {
            if (_updateTask == null || _updateTask.IsCompleted)
            {
                _updateTask?.Dispose();
                _updateTask = null;

                var context = GetSceneContext();
                _updateTask = UpdateTerrain(context, Async, _onDestroyCts.Token);
            }

            if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
            {
                ProcessMouseClick();
            }
        }

        private void ProcessMouseClick()
        {
            // Debug.LogFormat("OnMouseDown on tile.");

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            // Casts the ray and get the first game object hit
            Physics.Raycast(ray, out hit);

            var position = this.transform.worldToLocalMatrix.MultiplyPoint(hit.point).XY();
            position = _sampler.WorldToImage(position);

            this.SendMessage("OnCellClick", position, SendMessageOptions.DontRequireReceiver);
        }

        private async Task UpdateTerrain(SceneContext2i context, bool async, CancellationToken token)
        {
            GroupActions2i groupActions = null;
            IReadOnlyPooledDictionary<Vector2i, ITerrainMesh2i> results = null;
            try
            {
                Func<IReadOnlyPooledDictionary<Vector2i, ITerrainMesh2i>> computeAction = () =>
                    {
                        using (context.ProfilerFactory.Create("Creating visible groups"))
                        {
                            groupActions = context.GroupSelector.GetGroupsToUpdate(context);
                            Debug.Log($"update {groupActions.ToRecompute.Count} keep {groupActions.ToKeep.Count}");
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
                        var toKeep = groupActions?.ToKeep ?? Enumerable.Empty<Vector2i>();
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
            var container = this.gameObject;

            var existingGroups = _meshUpdater.ExistingGroups;

            var image = _imageProvider.CreateImage();

            Vector2i cellInGroupCount = new Vector2i(this.CellInGroupCount.x, this.CellInGroupCount.y);

            var localToProjection = camera.projectionMatrix * camera.worldToCameraMatrix * this.transform.localToWorldMatrix;

            var planes = GeometryUtility.CalculateFrustumPlanes(localToProjection);

            var frustumCorners = PooledArrayContainer<Vector3>.CreateDirty(4);

            camera.CalculateFrustumCorners(new Rect(0, 0, 1, 1), camera.farClipPlane, Camera.MonoOrStereoscopicEye.Mono, frustumCorners.Array);

            return new SceneContext2i
            (
                _groupsSelector,
                _terrainGenerator,
                _meshUpdater,
                camera.transform.position,
                planes,
                frustumCorners,
                camera.transform.localToWorldMatrix,
                container.transform.worldToLocalMatrix,
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
            go.transform.SetParent(this.transform, false);
            if (DrawBounds)
            {
                go.AddComponent<DrawBounds>();
            }
            var meshRenderer = go.GetOrAddComponent<MeshRenderer>();
            meshRenderer.material = Material;
            meshRenderer.materials = new Material[] { Material, MaterialWalls };
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

        private void Initialize()
        {
            _editableImageConstraint = new TycoonTileConstraint2i();
            _sampler = new DualImageSampler2i();

            _terrainGenerator = new TerrainGenerator2i<TerrainMesher2i>();
            _meshUpdater = new TerrainMeshUpdater2i();
            _groupsSelector = new GroupsByCameraVisibilitySelector2i();
            _imageProvider = new EditableMatrixImage2f(InitialTexture, InitialTextureScale, _sampler, _editableImageConstraint);
        }

        private void DisposeService()
        {
            (_imageProvider as IDisposable)?.Dispose();
            _imageProvider = null;

            (_editableImageConstraint as IDisposable)?.Dispose();
            _editableImageConstraint = null;

            (_sampler as IDisposable)?.Dispose();
            _sampler = null;

            (_terrainGenerator as IDisposable)?.Dispose();
            _terrainGenerator = null;

            (_meshUpdater as IDisposable)?.Dispose();
            _meshUpdater = null;

            (_groupsSelector as IDisposable)?.Dispose();
            _groupsSelector = null;
        }

        private void OnDestroy()
        {
            _onDestroyCts.Cancel();
            DisposeService();
        }
    }
}
