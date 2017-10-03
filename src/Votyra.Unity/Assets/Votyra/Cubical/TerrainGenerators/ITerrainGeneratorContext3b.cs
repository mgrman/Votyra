using System;
using System.Collections.Generic;
using UnityEngine;
using Votyra.Models;
using Votyra.Images;
using Votyra.ImageSamplers;
using Votyra.TerrainAlgorithms;

namespace Votyra.TerrainGenerators
{
    public interface ITerrainGeneratorContext3b : IContext
    {
        Vector3i CellInGroupCount { get; }
        IImageSampler3b ImageSampler { get; }
        IImage3f Image { get; }
    }
}