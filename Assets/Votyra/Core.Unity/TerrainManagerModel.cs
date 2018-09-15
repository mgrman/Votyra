using System.Collections.Generic;
using System.Linq;
using UniRx;

namespace Votyra.Core
{
    public class TerrainManagerModel : ITerrainManagerModel
    {
        public TerrainManagerModel()
        {
            AvailableAlgorithms = new BehaviorSubject<IEnumerable<TerrainAlgorithm>>(Enumerable.Empty<TerrainAlgorithm>());
            ActiveAlgorithm = new BehaviorSubject<TerrainAlgorithm>(null);
            Config = new BehaviorSubject<IReadOnlyCollection<ConfigItem>>(null);
        }

        public IBehaviorSubject<IEnumerable<TerrainAlgorithm>> AvailableAlgorithms { get; }
        public IBehaviorSubject<TerrainAlgorithm> ActiveAlgorithm { get; }

        public IBehaviorSubject<IReadOnlyCollection<ConfigItem>> Config { get; }
    }
}