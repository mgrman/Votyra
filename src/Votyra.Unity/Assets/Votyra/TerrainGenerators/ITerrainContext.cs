using System;
using System.Collections.Generic;
using UnityEngine;
using Votyra.Models;
using Votyra.Images;
using Votyra.ImageSamplers;
using Votyra.TerrainAlgorithms;
using Votyra.TerrainMeshers;
using Votyra.Unity.Assets.Votyra.Pooling;

namespace Votyra.TerrainGenerators
{
    public interface ITerrainContext
    {
        Vector2i CellInGroupCount { get; }
        Bounds GroupBounds { get; }
        Range2i RangeZ { get; }

        IImage2i Image { get; }
        Rect2i TransformedInvalidatedArea { get; }
        IImageSampler ImageSampler { get; }
        ITerrainAlgorithm TerrainAlgorithm { get; }
        ITerrainMesher TerrainMesher { get; }
    }
}