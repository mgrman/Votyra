using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Votyra.Core.Models;
using Votyra.Core.Images.Constraints;
using System;

namespace Votyra.Core
{
    public interface ITerrainConfig
    {
        Vector3i CellInGroupCount { get; }
        bool FlipTriangles { get; }
        bool DrawBounds { get; }
        bool Async { get; }
        Material Material { get; }
        Material MaterialWalls { get; }
    }
}