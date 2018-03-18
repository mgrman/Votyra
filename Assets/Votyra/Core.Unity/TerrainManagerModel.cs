using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Votyra.Core.Models;
using System;
using UniRx;
using System.Linq;
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
            InitialImageConfig = new BehaviorSubject<InitialImageConfig>(null);
            ImageConfig = new BehaviorSubject<ImageConfig>(null);
        }

        public IBehaviorSubject<IEnumerable<TerrainAlgorithm>> AvailableAlgorithms { get; }
        public IBehaviorSubject<TerrainAlgorithm> ActiveAlgorithm { get; }
        public IBehaviorSubject<TerrainConfig> TerrainConfig { get; }
        public IBehaviorSubject<InitialImageConfig> InitialImageConfig { get; }
        public IBehaviorSubject<ImageConfig> ImageConfig { get; }
    }
}