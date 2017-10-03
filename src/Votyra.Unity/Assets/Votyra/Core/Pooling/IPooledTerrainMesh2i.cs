using System;
using System.Collections.Generic;
using Votyra.Core.TerrainMeshes;

namespace Votyra.Core.Pooling
{
    public interface IPooledTerrainMesh2i : IPooledTerrainMesh, ITerrainMesh2i
    {
        new ITerrainMesh2i Mesh { get; }
    }
}
