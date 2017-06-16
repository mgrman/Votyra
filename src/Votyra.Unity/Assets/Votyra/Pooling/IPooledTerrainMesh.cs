using System;
using System.Collections.Generic;
using Votyra.TerrainGenerators.TerrainMeshers.TerrainMeshes;

namespace Votyra.Unity.Assets.Votyra.Pooling
{
    public interface IPooledTerrainMesh : ITerrainMesh2i, IDisposable
    {
        ITerrainMesh2i Mesh { get; }
    }
}