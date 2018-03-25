using UnityEngine;
using Votyra.Core.Images;
using Votyra.Core.Models;
using Votyra.Core.Utils;
using Zenject;
using System.Linq;
using UniRx;
using System;

namespace Votyra.Core.Unity
{
    public class TerrainDataController : MonoBehaviour
    {
        [SerializeField]
        private UI_Vector3i _imageSize = new UI_Vector3i(100, 100, 100);

        [SerializeField]
        private UnityEngine.Object _initialData = null;

        [SerializeField]
        private UI_Vector3f _initialDataScale = new UI_Vector3f(1, 1, 1);

        private object _initialDataFromPrevious = null;

        [SerializeField]
        private bool _keepImageChanges = true;

        [SerializeField]
        private UI_Vector3i _cellInGroupCount = new UI_Vector3i(10, 10, 10);

        [SerializeField]
        private bool _flipTriangles = false;

        [SerializeField]
        private bool _drawBounds = false;

        [SerializeField]
        private bool _async = true;

        [SerializeField]
        private Material _material = null;

        [SerializeField]
        private Material _materialWalls = null;

        [SerializeField]
        private int _activeTerrainAlgorithm = 0;

        [SerializeField]
        private GameObject[] _availableTerrainAlgorithms;

        private ITerrainManagerModel _terrainManagerModel;

        [NonSerialized]
        private GameObject _activeTerrainRoot = null;

        [Inject]
        public void Initialize(ITerrainManagerModel terrainManagerModel, Context context)
        {
            _terrainManagerModel = terrainManagerModel;
            var container = context.Container;

            UpdateModel();

            UniRx.Observable
                .CombineLatest(_terrainManagerModel.ActiveAlgorithm, _terrainManagerModel.ImageConfig, _terrainManagerModel.InitialImageConfig, _terrainManagerModel.TerrainConfig, (activeAlgorithm, imageConfig, initialImageConfig, terrainConfig) =>
                {
                    return new { activeAlgorithm, imageConfig, initialImageConfig, terrainConfig };
                })
                .Throttle(TimeSpan.FromMilliseconds(200))
                .Synchronize()
                .Subscribe(data =>
                {
                    if (_activeTerrainRoot != null)
                    {
                        _activeTerrainRoot.Destroy();
                        _activeTerrainRoot = null;
                    }
                    container.Unbind<IImageConfig>();
                    container.Unbind<IInitialImageConfig>();
                    container.Unbind<ITerrainConfig>();
                    if (data.activeAlgorithm == null || data.activeAlgorithm.Prefab == null)
                        return;

                    Debug.Log("activeAlgorithm:" + data.activeAlgorithm.Name);
                    container.Bind<IImageConfig>().FromInstance(data.imageConfig);
                    container.Bind<IInitialImageConfig>().FromInstance(data.initialImageConfig);
                    container.Bind<ITerrainConfig>().FromInstance(data.terrainConfig);
                    var instance = container.InstantiatePrefab(data.activeAlgorithm.Prefab, context.transform);

                    // TODO Not working, but there might be a way to inject directly into the child container
                    // var childContainer = instance.GetComponentInChildren<GameObjectContext>().Container;
                    // childContainer.Bind<IImageConfig>().FromInstance(data.imageConfig);
                    // childContainer.Bind<IInitialImageConfig>().FromInstance(data.initialImageConfig);
                    // childContainer.Bind<ITerrainConfig>().FromInstance(data.terrainConfig);

                    _activeTerrainRoot = instance;
                });
        }

        protected void Start()
        {
            UpdateModel();
        }
        protected void OnValidate()
        {
            UpdateModel();
        }

        private void UpdateModel()
        {
            if (_terrainManagerModel == null)
            {
                return;
            }
            if (_keepImageChanges && _activeTerrainRoot != null)
            {
                Debug.Log("Has old values");
                var goContext = _activeTerrainRoot.GetComponentInChildren<GameObjectContext>();
                var oldImageConfig = goContext.Container.Resolve<IImageConfig>();
                var oldImage2f = goContext.Container.TryResolve<IImage2fProvider>();
                var oldImage3f = goContext.Container.TryResolve<IImage3fProvider>();
                if (oldImage2f != null && oldImage3f != null)
                {
                    Debug.LogWarning("Previous algorithm worked in 2d and 3d mode. Using 2D data to keep.");
                }

                if (oldImage2f != null)
                {
                    var image = oldImage2f.CreateImage();
                    var matrix = new Matrix2<float>(oldImageConfig.ImageSize.XY);
                    for (int ix = 0; ix < matrix.size.x; ix++)
                    {
                        for (int iy = 0; iy < matrix.size.y; iy++)
                        {
                            var i = new Vector2i(ix, iy);
                            matrix[i] = image.Sample(i);
                        }
                    }
                    _initialDataFromPrevious = matrix;
                }
                else if (oldImage3f != null)
                {
                    var image = oldImage3f.CreateImage();
                    var matrix = new Matrix3<bool>(oldImageConfig.ImageSize);
                    for (int ix = 0; ix < matrix.size.x; ix++)
                    {
                        for (int iy = 0; iy < matrix.size.y; iy++)
                        {

                            for (int iz = 0; iz < matrix.size.z; iz++)
                            {
                                var i = new Vector3i(ix, iy, iz);
                                matrix[i] = image.Sample(i) > 0;
                            }
                        }
                    }
                    _initialDataFromPrevious = matrix;
                }
            }
            else
            {
                _initialDataFromPrevious = null;
            }

            var availableAlgorithms = _availableTerrainAlgorithms
                .EmptyIfNull()
                .Select(o => new TerrainAlgorithm(o.name, o))
                .ToArray();
            if (!_terrainManagerModel.AvailableAlgorithms.Value.SequenceEqual(availableAlgorithms))
                _terrainManagerModel.AvailableAlgorithms.OnNext(availableAlgorithms);

            TerrainAlgorithm activeAlgorithm;
            if (_activeTerrainAlgorithm < 0 || _activeTerrainAlgorithm >= availableAlgorithms.Length)
                activeAlgorithm = null;
            else
                activeAlgorithm = availableAlgorithms.Skip(_activeTerrainAlgorithm).FirstOrDefault();
            if (_terrainManagerModel.ActiveAlgorithm.Value != activeAlgorithm)
                _terrainManagerModel.ActiveAlgorithm.OnNext(activeAlgorithm);

            var imageConfig = new ImageConfig(_imageSize);
            if (_terrainManagerModel.ImageConfig.Value != imageConfig)
                _terrainManagerModel.ImageConfig.OnNext(imageConfig);

            if (_initialDataFromPrevious != null)
            {
                Debug.Log("Using _initialDataFromPrevious");
            }
            var initialImageConfig = new InitialImageConfig(_initialDataFromPrevious != null ? (object)_initialDataFromPrevious : _initialData, _initialDataFromPrevious != null ? Vector3f.One : (Vector3f)_initialDataScale);
            if (_terrainManagerModel.InitialImageConfig.Value != initialImageConfig)
                _terrainManagerModel.InitialImageConfig.OnNext(initialImageConfig);

            var terrainConfig = new TerrainConfig(_cellInGroupCount, _flipTriangles, _drawBounds, _async, _material, _materialWalls);
            if (_terrainManagerModel.TerrainConfig.Value != terrainConfig)
                _terrainManagerModel.TerrainConfig.OnNext(terrainConfig);
        }
    }
}