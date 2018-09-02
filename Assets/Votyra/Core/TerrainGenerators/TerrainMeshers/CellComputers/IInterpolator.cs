using System;
using System.Collections.Generic;
using UnityEngine;
using Votyra.Core.Images;
using Votyra.Core.ImageSamplers;
using Votyra.Core.Models;
using Votyra.Core.Pooling;
using Votyra.Core.TerrainMeshes;

namespace Votyra.Core.TerrainGenerators.TerrainMeshers.CellComputers
{
    public interface IInterpolator
    {
        float[,] InterpolationMatrix { get; }
        void PrepareInterpolation(Vector2i cell, Func<Vector2i, SampledData2f> sampleFunc);

        float Sample(Vector2f pos);
    }
}