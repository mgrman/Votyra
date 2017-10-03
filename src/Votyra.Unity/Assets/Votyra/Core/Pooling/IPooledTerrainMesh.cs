using System;
using System.Collections.Generic;
using Votyra.TerrainGenerators.TerrainMeshers.TerrainMeshes;

namespace Votyra.Core.Pooling
{
    public interface IPooledTerrainMesh : ITerrainMesh, IDisposable
    {
        ITerrainMesh Mesh { get; }
    }
}
