﻿using Votyra.Core.Models;

namespace Votyra.Core
{
    public interface ITerrainConfig : IConfig
    {
        Vector3i CellInGroupCount { get; }
        bool DrawBounds { get; }
        bool Async { get; }
    }
}