using System;
using System.Collections.Generic;
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
using Votyra.Plannar.GroupSelectors;
using Votyra.Plannar.Images;
using Votyra.Plannar.Images.Constraints;
using Votyra.Plannar.ImageSamplers;
using Votyra.Plannar.MeshUpdaters;
using Votyra.Plannar.TerrainGenerators;

namespace Votyra.Plannar
{
    //TODO: move to floats
    public abstract class TerrainGeneratorBehaviour2i : MonoBehaviour, IEditableImage2fProvider
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
        protected IImage2fProvider _imageProvider;
        protected IImageConstraint2i _editableImageConstraint;
        protected IImageSampler2i _sampler;
        protected IGroupSelector2i _groupsSelector;
        protected ITerrainGenerator2i _terrainGenerator;
        protected IMeshUpdater2i _meshUpdater;

        private Task _updateTask = null;
        private CancellationTokenSource _onDestroyCts = new CancellationTokenSource();

        private void Start()
        {
            this.gameObject.DestroyAllChildren();
            Initialize();
            _imageProvider = new EditableMatrixImage2f(new Vector2i(InitialTexture.width, InitialTexture.height), _editableImageConstraint);

            FillInitialState(EditableImage, InitialTexture, InitialTextureScale);
        }

        private static void FillInitialState(IEditableImage2f editableImage, object initialData, float scale)
        {
            if (editableImage == null)
                return;
            if (initialData is Texture2D)
            {
                FillInitialState(editableImage, initialData as Texture2D, scale);
            }
        }

        private static void FillInitialState(IEditableImage2f editableImage, Texture2D texture, float scale)
        {
            using (var imageAccessor = editableImage.RequestAccess(Rect2i.All))
            {
                Rect2i matrixAreaToFill;
                if (imageAccessor.Area == Rect2i.All)
                {
                    matrixAreaToFill = new Vector2i(texture.width, texture.height).ToRect2i();
                }
                else
                {
                    matrixAreaToFill = imageAccessor.Area;
                }

                var matrixSizeX = matrixAreaToFill.size.x;
                var matrixSizeY = matrixAreaToFill.size.y;

                for (int x = matrixAreaToFill.xMin; x < matrixAreaToFill.xMax; x++)
                {
                    for (int y = matrixAreaToFill.yMin; y < matrixAreaToFill.yMax; y++)
                    {
                        var pos = new Vector2i(x, y);
                        imageAccessor[pos] = texture.GetPixelBilinear((float)x / matrixSizeX, (float)y / matrixSizeY).grayscale * scale;
                    }
                }
            }
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

            var localPosition = this.transform.worldToLocalMatrix.MultiplyPoint(hit.point);

            var imagePosition = _sampler.WorldToImage(new Vector2f(localPosition.x, localPosition.y));
            this.SendMessage("OnCellClick", imagePosition, SendMessageOptions.DontRequireReceiver);
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

        protected abstract void Initialize();

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