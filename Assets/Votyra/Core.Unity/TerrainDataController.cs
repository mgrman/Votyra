using UnityEngine;
using Votyra.Core.Images;
using Votyra.Core.MeshUpdaters;
using Votyra.Core.Models;
using Votyra.Core.Utils;
using Votyra.Core.GroupSelectors;
using Votyra.Core.Images;
using Votyra.Core.Images.Constraints;
using Votyra.Core.ImageSamplers;
using Votyra.Core.TerrainGenerators;
using Votyra.Core.TerrainGenerators.TerrainMeshers;
using Zenject;
using System.Linq;
using UniRx;
using System;

namespace Votyra.Core.Unity
{
    public class TerrainDataController : MonoBehaviour
    {
        [SerializeField]
        private UI_Vector3i _imageSize;

        [SerializeField]
        private UnityEngine.Object _initialData;

        [SerializeField]
        private UI_Vector3f _initialDataScale;

        private Matrix<float> _initialDataFromPrevious;

        [SerializeField]
        private bool _keepImageChanges;

        [SerializeField]
        private UI_Vector3i _cellInGroupCount = new UI_Vector3i(10, 10, 10);

        [SerializeField]
        private bool _flipTriangles;

        [SerializeField]
        private bool _drawBounds;

        [SerializeField]
        private bool _async = true;

        [SerializeField]
        private Material _material;

        [SerializeField]
        private Material _materialWalls;

        [SerializeField]
        private int _activeTerrainAlgorithm;

        [SerializeField]
        private GameObject[] _availableTerrainAlgorithms;

        private ITerrainManagerModel _terrainManagerModel;

        [NonSerialized]
        private GameObject _activeTerrainRoot = null;

        [Inject]
        public void Initialize(ITerrainManagerModel terrainManagerModel, Context context)
        {
            _terrainManagerModel = terrainManagerModel;

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
                    if (data.activeAlgorithm == null || data.activeAlgorithm.Prefab == null)
                        return;

                    Debug.Log("activeAlgorithm:" + data.activeAlgorithm.Name);
                    var instance = context.Container.InstantiatePrefab(data.activeAlgorithm.Prefab, context.transform);

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
            if (_keepImageChanges)
            {
                Debug.Log("Has old values");
                var goContext = _activeTerrainRoot.GetComponentInChildren<GameObjectContext>();
                var oldImageConfig = goContext.Container.Resolve<IImageConfig>();
                var oldImage2f = goContext.Container.Resolve<IImage2fProvider>();
                // var lastImage3f = goContext.Container.Resolve<IImage3fProvider>();

                var image = oldImage2f.CreateImage();
                var matrix = new Matrix<float>(oldImageConfig.ImageSize.XY);
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

            var activeAlgorithm = _activeTerrainAlgorithm < 0 || _activeTerrainAlgorithm >= availableAlgorithms.Length ? null : availableAlgorithms.Skip(_activeTerrainAlgorithm).FirstOrDefault();
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