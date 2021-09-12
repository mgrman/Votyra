using System;
using System.ComponentModel;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using Votyra.Core.Utils;
using Zenject;

namespace Votyra.Core.Unity
{
    public class TerrainDataController : MonoBehaviour
    {
        [SerializeField]
        public int ActiveTerrainAlgorithm;

        [NonSerialized]
        private GameObject _activeTerrainRoot;

        [SerializeField]
        public GameObject[] AvailableTerrainAlgorithms;

        private ITerrainManagerModel _terrainManagerModel;

        [SerializeField]
        public ConfigItem[] Config;

        private Context _context;

        [Inject]
        public void Initialize(ITerrainManagerModel terrainManagerModel, Context context)
        {
            _terrainManagerModel = terrainManagerModel;
            _context = context;

            UpdateModel();

            _terrainManagerModel.PropertyChanged += OnTerrainManagerModelOnPropertyChanged;
            OnTerrainManagerModelOnPropertyChanged();
        }

        void OnTerrainManagerModelOnPropertyChanged(object o, PropertyChangedEventArgs e)
        {
            OnTerrainManagerModelOnPropertyChanged();
        }

        private void OnTerrainManagerModelOnPropertyChanged()
        {
            var container = _context.Container;
            try
            {
                if (_activeTerrainRoot != null)
                {
                    _activeTerrainRoot.Destroy();
                    _activeTerrainRoot = null;
                }

                if (_terrainManagerModel.ActiveAlgorithm == null || _terrainManagerModel.ActiveAlgorithm.Prefab == null)
                    return;

                if (_terrainManagerModel.Config != null)
                    foreach (var configItem in _terrainManagerModel.Config)
                    {
                        var type = configItem.Type;
                        var value = configItem.Value;

                        container.UnbindId(type, configItem.Id);
                        container.Bind(type)
                            .WithId(configItem.Id)
                            .FromInstance(value);
                    }

                var instance = container.InstantiatePrefab(_terrainManagerModel.ActiveAlgorithm.Prefab, _context.transform);
                _activeTerrainRoot = instance;
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
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

            var availableAlgorithms = AvailableTerrainAlgorithms.EmptyIfNull()
                .Select(o => new TerrainAlgorithm(o.name, o))
                .ToArray();
            if (!_terrainManagerModel.AvailableAlgorithms.SequenceEqual(availableAlgorithms))
                _terrainManagerModel.AvailableAlgorithms = availableAlgorithms;

            TerrainAlgorithm activeAlgorithm;
            if (ActiveTerrainAlgorithm < 0 || ActiveTerrainAlgorithm >= availableAlgorithms.Length)
                activeAlgorithm = null;
            else
                activeAlgorithm = availableAlgorithms.Skip(ActiveTerrainAlgorithm)
                    .FirstOrDefault();
            if (_terrainManagerModel.ActiveAlgorithm != activeAlgorithm)
                _terrainManagerModel.ActiveAlgorithm = activeAlgorithm;

            if (!(_terrainManagerModel.Config ?? Enumerable.Empty<ConfigItem>()).SequenceEqual(Config ?? Enumerable.Empty<ConfigItem>()))
                _terrainManagerModel.Config = Config.ToArray();
        }
    }
}