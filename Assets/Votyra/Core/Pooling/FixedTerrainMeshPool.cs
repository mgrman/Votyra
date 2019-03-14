using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Votyra.Core.Models;
using Votyra.Core.Models.ObjectPool;
using Votyra.Core.TerrainMeshes;

namespace Votyra.Core.Pooling
{
    public class FixedTerrainMeshPool : PoolWithKey<IPooledTerrainMesh, int>, ITerrainMeshPool
    {
        public FixedTerrainMeshPool(Func<Vector3f, Vector3f> vertexPostProcessor, Func<Vector2f, Vector2f> uvAdjustor)
            : base(CreateMeshFunc(vertexPostProcessor, uvAdjustor))
        {
        }

        private static Func<int, FixedTerrainMesh2i> CreateMeshFunc(Func<Vector3f, Vector3f> vertexPostProcessor, Func<Vector2f, Vector2f> uvAdjustor)
        {
            return (triangleCount) => new FixedTerrainMesh2i(triangleCount, vertexPostProcessor, uvAdjustor);
        }
    }
}