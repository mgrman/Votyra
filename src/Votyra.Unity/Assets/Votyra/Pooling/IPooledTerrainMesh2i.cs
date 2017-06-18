using System;
using System.Collections.Generic;
using Votyra.TerrainGenerators.TerrainMeshers.TerrainMeshes;

namespace Votyra.Unity.Assets.Votyra.Pooling
{
    public interface IPooledTerrainMesh2i : IPooledTerrainMesh, ITerrainMesh2i
    {
        new ITerrainMesh2i Mesh { get; }
    }
}