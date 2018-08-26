using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Votyra.Core.Images;

namespace Votyra.Core
{
    public interface ITerrainManagerModel
    {
        IBehaviorSubject<IEnumerable<TerrainAlgorithm>> AvailableAlgorithms { get; }
        IBehaviorSubject<TerrainAlgorithm> ActiveAlgorithm { get; }
        IBehaviorSubject<IReadOnlyCollection<ConfigItem>> Config { get; }
    }

}