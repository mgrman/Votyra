using System;
using Votyra.Core.TerrainMeshes;

namespace Votyra.Core.Pooling
{
    public interface IPooledTerrainMesh : ITerrainMesh, IDisposable
    {
        ITerrainMesh Mesh { get; }
    }
}