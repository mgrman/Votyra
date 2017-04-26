using System;
using System.Collections.Generic;
using System.Threading;
using TycoonTerrain.Common.Models;
using TycoonTerrain.Common.Utils;
using TycoonTerrain.Images;
using TycoonTerrain.ImageSamplers;
using TycoonTerrain.TerrainAlgorithms;
using TycoonTerrain.TerrainGenerators;
using TycoonTerrain.TerrainMeshers;
using TycoonTerrain.TerrainMeshers.TriangleMesh;
using TycoonTerrain.Unity.Behaviours;
using TycoonTerrain.Unity.GroupSelectors;
using TycoonTerrain.Unity.Images;
using TycoonTerrain.Unity.Logging;
using TycoonTerrain.Unity.MeshUpdaters;
using TycoonTerrain.Unity.Models;
using TycoonTerrain.Unity.Profiling;
using TycoonTerrain.Unity.Utils;
using UnityEngine;
using UnityEngine.Profiling;

namespace TycoonTerrain.Unity
{
    [ExecuteInEditMode]
    public class TerrainGeneratorBehaviour : MonoBehaviour
    {
        public UI_Vector2i CellInGroupCount = new UI_Vector2i(10, 10);

        public bool FlipTriangles = false;
        public bool ComputeOnAnotherThread;
        public bool DrawBounds;

        public Material Material = null;
        public MonoBehaviour Image = null;

        public ITerrainAlgorithm MeshGenerator = new TileSelectTerrainAlgorithm();
        public ITerrainMesher TerrainMesher = new TerrainMesher();
        public IImageSampler Sampler = new DualImageSampler();

        private IGroupSelector _groupsSelector;
        private IGenerator<TerrainOptions, IList<ITriangleMesh>> _terrainGenerator;
        private IMeshUpdater _meshUpdater;

        public static Thread UnityThread { get; private set; }

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
        }

        private void Update()
        {
            try
            {
                Profiler.BeginSample("Updating cached services");
                UpdateCachedServices();
                Profiler.EndSample();

                Profiler.BeginSample("Getting image");
                var image = GetImage();
                Profiler.EndSample();

                Profiler.BeginSample("Creating visible groups");
                var groupVisibilityOptions = CreateGroupVisibilityOptions(image);
                var groupsToUpdate = _groupsSelector.GetGroupsToUpdate(groupVisibilityOptions);
                Profiler.EndSample();

                Profiler.BeginSample("Sampling mesh");
                TerrainOptions terrainOptions = CreateTerrainOptions(image, groupsToUpdate);
                var results = _terrainGenerator.Generate(terrainOptions);
                Profiler.EndSample();

                Profiler.BeginSample("Applying mesh");
                MeshOptions meshOptions = CreateMeshOptions();
                _meshUpdater.UpdateMesh(meshOptions, results);
                Profiler.EndSample();
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }

            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
            {

                Debug.LogFormat("OnMouseDown on tile.");

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                // Casts the ray and get the first game object hit
                Physics.Raycast(ray, out hit);

                var point = this.transform.worldToLocalMatrix.MultiplyPoint(hit.point).XY();
                point = this.Sampler.TransformPoint(point);

                this.SendMessage("OnCellClick", point, SendMessageOptions.DontRequireReceiver);
            }
        }
        
        private IImage2i GetImage()
        {
            var imageProvider = this.Image as IImage2iProvider;
            return imageProvider == null ? null : imageProvider.CreateImage();
        }

        private GroupVisibilityOptions CreateGroupVisibilityOptions(IImage2i image)
        {
            var CellInGroupCount = new Vector2i(this.CellInGroupCount.x, this.CellInGroupCount.y);
            return new GroupVisibilityOptions(Camera.main, this.gameObject, image.RangeZ, CellInGroupCount);
        }

        private static GameObject GameObjectFactory()
        {
            var go= new GameObject();
            go.AddComponent<ClickGroupBehaviour>();
            return go;
        }

        private MeshOptions CreateMeshOptions()
        {
            return new MeshOptions(this.Material, this.gameObject, this.DrawBounds, GameObjectFactory);
        }

        private TerrainOptions CreateTerrainOptions(IImage2i image, IList<Vector2i> groupsToUpdate)
        {
            Vector2i cellInGroupCount = new Vector2i(this.CellInGroupCount.x, this.CellInGroupCount.y);
            return new TerrainOptions(cellInGroupCount, this.FlipTriangles, image, this.Sampler, this.MeshGenerator, this.TerrainMesher, UnityEngine.Time.time, groupsToUpdate);
        }

        private void UpdateCachedServices()
        {
            bool computeOnAnotherThread = this.ComputeOnAnotherThread;
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                computeOnAnotherThread = false;
            }
#endif
            if (computeOnAnotherThread)
            {
                ObjectUtils.UpdateType<AsyncGenerator<TerrainGenerator, TerrainOptions, IList<ITriangleMesh>>, IGenerator<TerrainOptions, IList<ITriangleMesh>>>(ref _terrainGenerator);
            }
            else
            {
                ObjectUtils.UpdateType<TerrainGenerator, IGenerator<TerrainOptions, IList<ITriangleMesh>>>(ref _terrainGenerator);
            }

            ObjectUtils.UpdateType<TerrainMeshUpdater, IMeshUpdater>(ref _meshUpdater);
            ObjectUtils.UpdateType<GroupsByCameraVisibilitySelector, IGroupSelector>(ref _groupsSelector);
        }

        private void OnDisable()
        {
            DisposeService();
        }

        private void OnDestroy()
        {
            DisposeService();
        }

        private void DisposeService()
        {
            if (_terrainGenerator is IDisposable)
            {
                (_terrainGenerator as IDisposable).Dispose();
            }
            _terrainGenerator = null;

            if (_meshUpdater is IDisposable)
            {
                (_meshUpdater as IDisposable).Dispose();
            }
            _meshUpdater = null;
        }
    }
}