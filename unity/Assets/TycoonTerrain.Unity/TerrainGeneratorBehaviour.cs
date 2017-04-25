using System;
using System.Collections.Generic;
using System.Threading;
using TycoonTerrain.Common.Utils;
using TycoonTerrain.ImageSamplers;
using TycoonTerrain.TerrainAlgorithms;
using TycoonTerrain.TerrainGenerators;
using TycoonTerrain.TerrainMeshers;
using TycoonTerrain.TerrainMeshers.TriangleMesh;
using TycoonTerrain.Unity.GroupSelectors;
using TycoonTerrain.Unity.Logging;
using TycoonTerrain.Unity.MeshUpdaters;
using TycoonTerrain.Unity.Models;
using TycoonTerrain.Unity.Profiling;
using TycoonTerrain.Unity.TerrainGenerators;
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
            Profiler.BeginSample("Updating cached services");
            UpdateCachedServices();
            Profiler.EndSample();

            Profiler.BeginSample("Creating visible groups");
            var groupVisibilityOptions = new GroupVisibilityOptions(this, Camera.main);
            var groupsToUpdate = _groupsSelector.GetGroupsToUpdate(groupVisibilityOptions);
            Profiler.EndSample();

            Profiler.BeginSample("Sampling mesh");
            TerrainOptions terrainOptions = TerrainOptionsFactory.Create(this, groupsToUpdate);
            var results = _terrainGenerator.Generate(terrainOptions);
            Profiler.EndSample();

            Profiler.BeginSample("Applying mesh");
            MeshOptions meshOptions = new MeshOptions(this);
            _meshUpdater.UpdateMesh(meshOptions, results);
            Profiler.EndSample();
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