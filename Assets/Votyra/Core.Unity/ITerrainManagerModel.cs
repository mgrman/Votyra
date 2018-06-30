using System.Collections.Generic;
using UniRx;
using Votyra.Core.Images;

namespace Votyra.Core
{
    public interface ITerrainManagerModel
    {
        IBehaviorSubject<TerrainAlgorithm> ActiveAlgorithm { get; }
        IBehaviorSubject<IEnumerable<TerrainAlgorithm>> AvailableAlgorithms { get; }
        IBehaviorSubject<ImageConfig> ImageConfig { get; }
        IBehaviorSubject<InitialImageConfig> InitialImageConfig { get; }
        IBehaviorSubject<MaterialConfig> MaterialConfig { get; }
        IBehaviorSubject<TerrainConfig> TerrainConfig { get; }
    }
}