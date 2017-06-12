using System;
using System.Collections.Generic;
using UnityEngine;
using Votyra.Models;
using Votyra.Images;
using Votyra.ImageSamplers;
using Votyra.TerrainAlgorithms;
using Votyra.TerrainMeshers;
using Votyra.Unity.Assets.Votyra.Pooling;

namespace Votyra.TerrainTileGenerators
{
    public interface ITerrainTileContext : IContext
    {
        Vector2i CellInGroupCount { get; }

        IImage2i Image { get; }
        IImageSampler ImageSampler { get; }
        ITerrainAlgorithm TerrainAlgorithm { get; }
    }
}