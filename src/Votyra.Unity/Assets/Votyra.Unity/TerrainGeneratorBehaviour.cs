using System;
using System.Collections.Generic;
using System.Threading;
using Votyra.Common.Models;
using Votyra.Common.Utils;
using Votyra.Images;
using Votyra.ImageSamplers;
using Votyra.TerrainAlgorithms;
using Votyra.TerrainGenerators;
using Votyra.TerrainMeshers;
using Votyra.TerrainMeshers.TriangleMesh;
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
using Votyra.Common.Profiling;
using Votyra.Unity.Assets.Votyra.Pooling;

namespace Votyra.Unity
{
    public class TerrainGeneratorBehaviour : MonoBehaviour
    {
        public UI_Vector2i CellInGroupCount = new UI_Vector2i(10, 10);

        public bool FlipTriangles = false;
        public bool DrawBounds;

        public Material Material = null;
        public MonoBehaviour Image = null;

        public ITerrainAlgorithm _meshGenerator;
        public ITerrainMesher _terrainMesher;
        public IImageSampler _sampler;

        private IGroupSelector _groupsSelector;
        private IGenerator<TerrainOptions, IReadOnlyDictionary<Vector2i, ITriangleMesh>> _terrainGenerator;
        private IMeshUpdater _meshUpdater;

        public static Thread UnityThread { get; private set; }

        private Task _updateTask = null;

        static TerrainGeneratorBehaviour()
        {
            UnityThread = Thread.CurrentThread;
            if (Common.Logging.LoggerFactory.Factory == null)
                Common.Logging.LoggerFactory.Factory = (name) => new UnityLogger(name);
            if (Common.Profiling.ProfilerFactory.Factory == null)
                Common.Profiling.ProfilerFactory.Factory = (name) => new UnityProfiler(name, UnityThread);
        }

        private void Start()
        {
            this.gameObject.DestroyAllChildren();
            Initialize();
        }

        private void Update()
        {
            if (_updateTask == null || _updateTask.IsCompleted)
            {
                _updateTask?.Dispose();
                _updateTask = null;

                _updateTask = UpdateTerrain();
            }

            if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
            {
                ProcessMouseClick();
            }
        }

        private void ProcessMouseClick()
        {
            Debug.LogFormat("OnMouseDown on tile.");

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            // Casts the ray and get the first game object hit
            Physics.Raycast(ray, out hit);

            var point = this.transform.worldToLocalMatrix.MultiplyPoint(hit.point).XY();
            point = _sampler.Transform(point);

            this.SendMessage("OnCellClick", point, SendMessageOptions.DontRequireReceiver);
        }

        private async Task UpdateTerrain()
        {
            try
            {
                IImage2i image;
                using (ProfilerFactory.Create("Getting image"))
                {
                    image = GetImage();
                    (image as IInitializableImage).StartUsing();
                }

                IReadOnlyPooledCollection<Group> groupsToUpdate;
                using (ProfilerFactory.Create("Creating visible groups"))
                {
                    using (var groupVisibilityOptions = CreateGroupVisibilityOptions(image, _meshUpdater.ExistingGroups))
                    {
                        groupsToUpdate = _groupsSelector.GetGroupsToUpdate(groupVisibilityOptions);
                    }
                }

                IReadOnlyDictionary<Vector2i, ITriangleMesh> results = null;
                await Task.Run(() =>
                 {
                     using (ProfilerFactory.Create("Sampling mesh"))
                     {
                         using (TerrainOptions terrainOptions = CreateTerrainOptions(image, groupsToUpdate.Where(o => o.UpdateAction == UpdateAction.Recompute).Select(o => o.Index).ToArray()))
                         {
                             results = _terrainGenerator.Generate(terrainOptions);
                         }
                        (image as IInitializableImage).FinishUsing();
                     }
                 });

                using (ProfilerFactory.Create("Applying mesh"))
                {
                    using (MeshOptions meshOptions = CreateMeshOptions())
                    {
                        _meshUpdater.UpdateMesh(meshOptions, results, groupsToUpdate.Where(o => o.UpdateAction == UpdateAction.Keep).Select(o => o.Index).ToArray());
                    }
                }
                groupsToUpdate.Dispose();
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }

        }

        private IImage2i GetImage()
        {
            var imageProvider = this.Image as IImage2iProvider;
            return imageProvider == null ? null : imageProvider.CreateImage();
        }

        private GroupVisibilityOptions CreateGroupVisibilityOptions(IImage2i image, IEnumerable<Vector2i> existingGroups)
        {
            if (image == null)
            {
                return null;
            }
            var cellInGroupCount = new Vector2i(this.CellInGroupCount.x, this.CellInGroupCount.y);

            var transformedInvalidatedArea = _sampler
                .InverseTransform(image.InvalidatedArea.ToRect())
                .RoundToContain();

            return new GroupVisibilityOptions(Camera.main, this.gameObject, image.RangeZ, transformedInvalidatedArea, cellInGroupCount, existingGroups);
        }

        private static GameObject GameObjectFactory()
        {
            var go = new GameObject();
            return go;
        }

        private MeshOptions CreateMeshOptions()
        {
            return new MeshOptions(this.Material, this.gameObject, this.DrawBounds, GameObjectFactory);
        }

        private TerrainOptions CreateTerrainOptions(IImage2i image, IReadOnlyCollection<Vector2i> groupsToUpdate)
        {
            Vector2i cellInGroupCount = new Vector2i(this.CellInGroupCount.x, this.CellInGroupCount.y);
            return new TerrainOptions(cellInGroupCount, this.FlipTriangles, image, _sampler, _meshGenerator, _terrainMesher, groupsToUpdate);
        }

        private void Initialize()
        {
            _meshGenerator = new TileSelectTerrainAlgorithm();
            _terrainMesher = new TerrainMesher();
            _sampler = new DualImageSampler();

            _terrainGenerator = new TerrainGenerator();
            _meshUpdater = new TerrainMeshUpdater();
            _groupsSelector = new ForceRecomputeInNotExistingVisibilitySelector(new InvalidatedAreaVisibilitySelector(new GroupsByCameraVisibilitySelector()));
        }


        private void DisposeService()
        {
            (_meshGenerator as IDisposable)?.Dispose();
            _meshGenerator = null;

            (_terrainMesher as IDisposable)?.Dispose();
            _terrainMesher = null;

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
            DisposeService();
        }
    }
}