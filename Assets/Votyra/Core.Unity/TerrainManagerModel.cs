﻿using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UniRx;

namespace Votyra.Core
{
    public class TerrainManagerModel : ITerrainManagerModel
    {
        private IReadOnlyCollection<ConfigItem> _config;
        
        private TerrainAlgorithm _activeAlgorithm;
        
        private IEnumerable<TerrainAlgorithm> _availableAlgorithms;

        public TerrainManagerModel()
        {
            AvailableAlgorithms = Enumerable.Empty<TerrainAlgorithm>();
            ActiveAlgorithm = null;
            Config = null;
        }

        public IEnumerable<TerrainAlgorithm> AvailableAlgorithms
        {
            get => _availableAlgorithms;
            set
            {
                if (Equals(value, _availableAlgorithms))
                    return;
                _availableAlgorithms = value;
                OnPropertyChanged();
            }
        }

        public TerrainAlgorithm ActiveAlgorithm
        {
            get => _activeAlgorithm;
            set
            {
                if (Equals(value, _activeAlgorithm))
                    return;
                _activeAlgorithm = value;
                OnPropertyChanged();
            }
        }

        public IReadOnlyCollection<ConfigItem> Config
        {
            get => _config;
            set
            {
                if (Equals(value, _config))
                    return;
                _config = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}