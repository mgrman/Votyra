using System;
using System.Collections.Generic;
using Votyra.TerrainMeshers.TriangleMesh;

namespace Votyra.Unity.Assets.Votyra.Pooling
{
    public interface IPooledTriangleMesh : ITerrainMesh, IDisposable
    {
        ITerrainMesh Mesh { get; }
    }
}