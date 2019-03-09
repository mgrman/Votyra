using System;
using Votyra.Core.TerrainMeshes;

namespace Votyra.Core.Pooling
{
    public interface IPooledTerrainMesh : IDisposable
    {
        ITerrainMesh Mesh { get; }

        event Action<IPooledTerrainMesh> OnDispose; 
    }
}