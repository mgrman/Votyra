using System;
using System.Collections.Generic;
using UnityEngine;
using Votyra.Models;
using Votyra.Images;
using Votyra.ImageSamplers;
using Votyra.TerrainAlgorithms;
using Votyra.TerrainMeshers;

namespace Votyra.TerrainMeshGenerators
{
    public interface ITerrainMeshContext : IContext
    {
        Vector2i CellInGroupCount { get; }
        Bounds GroupBounds { get; }
    }
}