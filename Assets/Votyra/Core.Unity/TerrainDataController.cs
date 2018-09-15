using System;
using System.Linq;
using UniRx;
using UnityEngine;
using Votyra.Core.Utils;
using Zenject;

namespace Votyra.Core.Unity
{
    public class TerrainDataController : MonoBehaviour
    {
        [SerializeField]
        public int _activeTerrainAlgorithm = 0;

        [SerializeField]
        public GameObject[] _availableTerrainAlgorithms;

        [SerializeField]
        public ConfigItem[] Config;

        [NonSerialized]
        private GameObject _activeTerrainRoot = null;

        private ITerrainManagerModel _terrainManagerModel;

        [Inject]
        public void Initialize(ITerrainManagerModel terrainManagerModel, Context context)
        {
            _terrainManagerModel = terrainManagerModel;
            var container = context.Container;

            UpdateModel();

            UniRx.Observable
                .CombineLatest(_terrainManagerModel.ActiveAlgorithm, _terrainManagerModel.Config, (activeAlgorithm, config) =>
                 {
                     return new { activeAlgorithm, config };
                 })
                .Throttle(TimeSpan.FromMilliseconds(200))
                .Synchronize()
                .Subscribe(data =>
                {
                    try
                    {
                        if (_activeTerrainRoot != null)
                        {
                            _activeTerrainRoot.Destroy();
                            _activeTerrainRoot = null;
                        }

                        if (data.activeAlgorithm == null || data.activeAlgorithm.Prefab == null)
                            return;

                        if (data.config != null)
                        {
                            foreach (var configItem in data.config)
                            {
                                var type = configItem.Type;
                                var value = configItem.Value;

                                container.UnbindId(type, configItem.Id);
                                container.Bind(type).WithId(configItem.Id).FromInstance(value);
                            }
                        }

                        var instance = container.InstantiatePrefab(data.activeAlgorithm.Prefab, context.transform);
                        _activeTerrainRoot = instance;
                    }
                    catch (Exception ex)
                    {
                        Debug.LogException(ex);
                    }
                });
        }

        protected void OnValidate()
        {
            UpdateModel();
        }

        protected void Start()
        {
            UpdateModel();
        }

        private void UpdateModel()
        {
            if (_terrainManagerModel == null)
            {
                return;
            }
            // if (_keepImageChanges && _activeTerrainRoot != null)
            // {
            //     Debug.Log("Has old values");
            //     var goContext = _activeTerrainRoot.GetComponentInChildren<GameObjectContext>();
            //     var oldImageConfig = goContext.Container.Resolve<IImageConfig>();
            //     var oldImage2i = goContext.Container.TryResolve<IImage2iProvider>();
            //     var oldImage3b = goContext.Container.TryResolve<IImage3bProvider>();
            //     if (oldImage2i != null && oldImage3b != null)
            //     {
            //         Debug.LogWarning("Previous algorithm worked in 2d and 3d mode. Using 2D data to keep.");
            //     }

            //     if (oldImage2i != null)
            //     {
            //         var image = oldImage2i.CreateImage();
            //         var matrix = new Matrix2<Height>(oldImageConfig.ImageSize.XY);
            //         matrix.Size
            //             .ToRange2i()
            //             .ForeachPointExlusive(i =>
            //             {
            //                 matrix[i] = image.Sample(i);
            //             });

            //         _initialDataFromPrevious = matrix;
            //     }
            //     else if (oldImage3b != null)
            //     {
            //         var image = oldImage3b.CreateImage();
            //         var matrix = new Matrix3<bool>(oldImageConfig.ImageSize);
            //         matrix.Size
            //             .ToRange3i()
            //             .ForeachPointExlusive(i =>
            //             {
            //                 matrix[i] = image.Sample(i);
            //             });
            //         _initialDataFromPrevious = matrix;
            //     }
            // }
            // else
            // {
            //     _initialDataFromPrevious = null;
            // }

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

            if (!(_terrainManagerModel.Config.Value ?? Enumerable.Empty<ConfigItem>()).SequenceEqual(Config ?? Enumerable.Empty<ConfigItem>()))
                _terrainManagerModel.Config.OnNext(Config.ToArray());
        }
    }
}