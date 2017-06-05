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

namespace Votyra.Unity
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
        private IGenerator<TerrainOptions, IReadOnlyDictionary<Vector2i, ITriangleMesh>> _terrainGenerator;
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
                (image as IInitializableImage).StartUsing();
                Profiler.EndSample();

                Profiler.BeginSample("Creating visible groups");
                var groupVisibilityOptions = CreateGroupVisibilityOptions(image);
                var groupsToUpdate = _groupsSelector.GetGroupsToUpdate(groupVisibilityOptions);
                groupVisibilityOptions.Dispose();
                Profiler.EndSample();

                Profiler.BeginSample("Sampling mesh");
                TerrainOptions terrainOptions = CreateTerrainOptions(image, groupsToUpdate);
                var results = _terrainGenerator.Generate(terrainOptions);
                terrainOptions.Dispose();
                (image as IInitializableImage).FinishUsing();
                Profiler.EndSample();

                Profiler.BeginSample("Applying mesh");
                MeshOptions meshOptions = CreateMeshOptions();
                _meshUpdater.UpdateMesh(meshOptions, results);
                meshOptions.Dispose();
                Profiler.EndSample();
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }

            if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
            {

                Debug.LogFormat("OnMouseDown on tile.");

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                // Casts the ray and get the first game object hit
                Physics.Raycast(ray, out hit);

                var point = this.transform.worldToLocalMatrix.MultiplyPoint(hit.point).XY();
                point = this.Sampler.Transform(point);

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
            if (image == null)
            {
                return null;
            }
            var CellInGroupCount = new Vector2i(this.CellInGroupCount.x, this.CellInGroupCount.y);
            return new GroupVisibilityOptions(Camera.main, this.gameObject, image.RangeZ, CellInGroupCount);
        }

        private static GameObject GameObjectFactory()
        {
            var go = new GameObject();
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
            return new TerrainOptions(cellInGroupCount, this.FlipTriangles, image, this.Sampler, this.MeshGenerator, this.TerrainMesher, groupsToUpdate);
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
                ObjectUtils.UpdateType<AsyncGenerator<TerrainGenerator, TerrainOptions, IReadOnlyDictionary<Vector2i, ITriangleMesh>>, IGenerator<TerrainOptions, IReadOnlyDictionary<Vector2i, ITriangleMesh>>>(ref _terrainGenerator);
            }
            else
            {
                ObjectUtils.UpdateType<TerrainGenerator, IGenerator<TerrainOptions, IReadOnlyDictionary<Vector2i, ITriangleMesh>>>(ref _terrainGenerator);
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
            (_terrainGenerator as IDisposable)?.Dispose();
            _terrainGenerator = null;

            (_meshUpdater as IDisposable)?.Dispose();
            _meshUpdater = null;
        }
    }
}