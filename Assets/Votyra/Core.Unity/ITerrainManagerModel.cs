using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Votyra.Core.Models;
using System;
using UniRx;
using Votyra.Core.Images;

namespace Votyra.Core
{
    public interface ITerrainManagerModel
    {
        IBehaviorSubject<IEnumerable<TerrainAlgorithm>> AvailableAlgorithms { get; }
        IBehaviorSubject<TerrainAlgorithm> ActiveAlgorithm { get; }
        IBehaviorSubject<TerrainConfig> TerrainConfig { get; }
        IBehaviorSubject<MaterialConfig> MaterialConfig { get; }
        IBehaviorSubject<InitialImageConfig> InitialImageConfig { get; }
        IBehaviorSubject<ImageConfig> ImageConfig { get; }
    }
}