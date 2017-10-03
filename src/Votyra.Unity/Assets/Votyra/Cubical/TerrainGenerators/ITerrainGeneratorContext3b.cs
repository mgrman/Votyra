using System;
using System.Collections.Generic;
using UnityEngine;
using Votyra.Core;
using Votyra.Core.Images;
using Votyra.Core.Models;
using Votyra.Cubical.ImageSamplers;

namespace Votyra.Cubical.TerrainGenerators
{
    public interface ITerrainGeneratorContext3b : IContext
    {
        Vector3i CellInGroupCount { get; }
        IImageSampler3b ImageSampler { get; }
        IImage3f Image { get; }
    }
}
