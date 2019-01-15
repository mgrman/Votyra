using System.Collections.Generic;
using System.ComponentModel;
using UniRx;

namespace Votyra.Core
{
    public interface ITerrainManagerModel:INotifyPropertyChanged
    {
        IEnumerable<TerrainAlgorithm> AvailableAlgorithms { get; set; }
        TerrainAlgorithm ActiveAlgorithm { get; set; }
        IReadOnlyCollection<ConfigItem> Config { get; set; }
    }
}