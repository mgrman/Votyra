using System;
using System.Collections.Generic;
using Votyra.TerrainGenerators.TerrainMeshers.TerrainMeshes;

namespace Votyra.Unity.Assets.Votyra.Pooling
{
    public interface IPooledTerrainMesh : ITerrainMesh, IDisposable
    {
        ITerrainMesh Mesh { get; }
    }
}