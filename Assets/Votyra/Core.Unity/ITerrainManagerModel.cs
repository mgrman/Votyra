using System.Collections.Generic;
using System.ComponentModel;
using Votyra.Core.Unity.Config;

namespace Votyra.Core.Unity
{
    public interface ITerrainManagerModel:INotifyPropertyChanged
    {
        IEnumerable<ITerrainAlgorithm> AvailableAlgorithms { get; set; }
        ITerrainAlgorithm ActiveAlgorithm { get; set; }
        IReadOnlyCollection<ConfigItem> Config { get; set; }
    }
}