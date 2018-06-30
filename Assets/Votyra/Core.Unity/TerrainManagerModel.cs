using System.Collections.Generic;
using System.Linq;
using UniRx;
using Votyra.Core.Images;

namespace Votyra.Core
{
    public class TerrainManagerModel : ITerrainManagerModel
    {
        public TerrainManagerModel()
        {
            AvailableAlgorithms = new BehaviorSubject<IEnumerable<TerrainAlgorithm>>(Enumerable.Empty<TerrainAlgorithm>());
            ActiveAlgorithm = new BehaviorSubject<TerrainAlgorithm>(null);
            TerrainConfig = new BehaviorSubject<TerrainConfig>(null);
            MaterialConfig = new BehaviorSubject<MaterialConfig>(null);
            InitialImageConfig = new BehaviorSubject<InitialImageConfig>(null);
            ImageConfig = new BehaviorSubject<ImageConfig>(null);
        }

        public IBehaviorSubject<TerrainAlgorithm> ActiveAlgorithm { get; }
        public IBehaviorSubject<IEnumerable<TerrainAlgorithm>> AvailableAlgorithms { get; }
        public IBehaviorSubject<ImageConfig> ImageConfig { get; }
        public IBehaviorSubject<InitialImageConfig> InitialImageConfig { get; }
        public IBehaviorSubject<MaterialConfig> MaterialConfig { get; }
        public IBehaviorSubject<TerrainConfig> TerrainConfig { get; }
    }
}