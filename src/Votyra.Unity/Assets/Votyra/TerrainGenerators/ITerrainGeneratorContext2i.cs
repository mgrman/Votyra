using System;
using System.Collections.Generic;
using UnityEngine;
using Votyra.Models;
using Votyra.Images;
using Votyra.ImageSamplers;
using Votyra.TerrainAlgorithms;

namespace Votyra.TerrainGenerators
{
    public interface ITerrainGeneratorContext2i : IContext
    {
        Vector2i CellInGroupCount { get; }
        Bounds GroupBounds { get; }
        IImageSampler2i ImageSampler { get; }
        IImage2i Image { get; }
    }
}