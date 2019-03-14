using System;
using Votyra.Core.TerrainMeshes;

namespace Votyra.Core.Pooling
{
    public interface IPooledTerrainMesh : ITerrainMesh,IPoolable<IPooledTerrainMesh, int>
    {
    }
}