using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Votyra.Core.Models;
using Votyra.Plannar.Images.Constraints;

namespace Votyra.Plannar.Images
{
    public interface ITerrainConfig : IInitialImageConfig
    {
        Vector2i CellInGroupCount { get; }
        bool FlipTriangles { get; }
        bool DrawBounds { get; }
        bool Async { get; }
        Material Material { get; }
        Material MaterialWalls { get; }
    }
}