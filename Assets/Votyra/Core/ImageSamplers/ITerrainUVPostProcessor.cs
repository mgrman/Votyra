using System;
using System.Collections.Generic;
using UnityEngine;
using Votyra.Core.Images;
using Votyra.Core.ImageSamplers;
using Votyra.Core.Models;
using Votyra.Core.Pooling;
using Votyra.Core.TerrainMeshes;
using Votyra.Core.Utils;

namespace Votyra.Core.ImageSamplers
{
    public interface ITerrainUVPostProcessor
    {
        Vector2f ProcessUV(Vector2f vertex);
        Vector2f ReverseUV(Vector2f vertex);
    }
}