using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Votyra.Core.Behaviours;
using Votyra.Core.Images;
using Votyra.Core.Logging;
using Votyra.Core.Models;
using Votyra.Core.Pooling;
using Votyra.Core.Profiling;
using Votyra.Core.TerrainMeshes;
using Votyra.Core.Utils;
using Votyra.Cubical.GroupSelectors;
using Votyra.Cubical.ImageSamplers;
using Votyra.Cubical.MeshUpdaters;
using Votyra.Cubical.TerrainGenerators;
using Votyra.Cubical.TerrainGenerators.TerrainMeshers;

namespace Votyra.Cubical
{
    //TODO: move to floats
    public class TerrainGeneratorBehaviour3b : MonoBehaviour
    {
        public UI_Vector3i CellInGroupCount = new UI_Vector3i(10, 10, 10);
        public bool DrawBounds = false;
        public bool Async = true;
        public Material Material = null;

        // public Material MaterialWalls = null;
        public Texture2D InitialTexture = null;

        public float InitialTextureScale = 1;

        // public IEditableImage3i EditableImage { get { return _imageProvider as IEditableImage2i; } }

        private IImage3f _image;

        // private IImage3iProvider _imageProvider;
        // private IImageConstraint2i _editableImageConstraint;
        private IImageSampler3b _sampler;

        private IGroupSelector3i _groupsSelector;
        private ITerrainGenerator3b _terrainGenerator;
        private IMeshUpdater3i _meshUpdater;

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

            // if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
            // {
            //     ProcessMouseClick();
            // }
        }

        // private void ProcessMouseClick()
        // {
        //     // Debug.LogFormat("OnMouseDown on tile.");

        //     Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //     RaycastHit hit;
        //     // Casts the ray and get the first game object hit
        //     Physics.Raycast(ray, out hit);

        //     var position = this.transform.worldToLocalMatrix.MultiplyPoint(hit.point).XY();
        //     position = _sampler.WorldToImage(position);

        //     this.SendMessage("OnCellClick", position, SendMessageOptions.DontRequireReceiver);
        // }

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
            var container = this.gameObject;

            var existingGroups = _meshUpdater.ExistingGroups;

            var image = _image;//_image ?? _imageProvider.CreateImage();

            Vector3i cellInGroupCount = new Vector3i(this.CellInGroupCount.x, this.CellInGroupCount.y, this.CellInGroupCount.z);

            var localToProjection = camera.projectionMatrix * camera.worldToCameraMatrix * this.transform.localToWorldMatrix;

            var planes = GeometryUtility.CalculateFrustumPlanes(localToProjection);

            var frustumCorners = PooledArrayContainer<Vector3>.CreateDirty(4);

            camera.CalculateFrustumCorners(new Rect(0, 0, 1, 1), camera.farClipPlane, Camera.MonoOrStereoscopicEye.Mono, frustumCorners.Array);

            return new SceneContext3b
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
                null,//Rect3i.All, //(image as IImageInvalidatableImage3i)?.InvalidatedArea ?? Rect2i.All,
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
            // _editableImageConstraint = new TycoonTileConstraint2i();
            _sampler = new SimpleImageSampler3b();

            _terrainGenerator = new TerrainGenerator3b<TerrainMesher3b>();
            _meshUpdater = new TerrainMeshUpdater3i();
            _groupsSelector = new GroupsByCameraVisibilitySelector3i();
            //_image = new NoiseImage3b(Vector3.zero, new Vector3(100, 100, 100));//new EditableMatrixImagei(InitialTexture, InitialTextureScale, _sampler, _editableImageConstraint);

            int width = InitialTexture.width;
            int height = InitialTexture.height;

            var size = new Vector2i(width, height);
            var matrix = new LockableMatrix<float>(size);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    matrix[x, y] = (InitialTexture.GetPixel(x, y).grayscale * InitialTextureScale);
                }
            }
            _image = new UmbraImage3f(new MatrixImage2f(matrix, Rect2i.All));
        }

        private void DisposeService()
        {
            (_image as IDisposable)?.Dispose();
            _image = null;
            // (_imageProvider as IDisposable)?.Dispose();
            // _imageProvider = null;

            // (_editableImageConstraint as IDisposable)?.Dispose();
            // _editableImageConstraint = null;

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