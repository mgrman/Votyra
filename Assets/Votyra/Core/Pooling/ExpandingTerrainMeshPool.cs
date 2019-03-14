using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Votyra.Core.Models;
using Votyra.Core.Models.ObjectPool;
using Votyra.Core.TerrainMeshes;

namespace Votyra.Core.Pooling
{
    public class ExpandingTerrainMeshPool : Pool<IPooledTerrainMesh>, ITerrainMeshPool
    {
        public ExpandingTerrainMeshPool(Func<Vector3f, Vector3f> vertexPostProcessor, Func<Vector2f, Vector2f> uvAdjustor)
            : base(CreateMeshFunc(vertexPostProcessor, uvAdjustor))
        {
        }

        private static Func< ExpandingTerrainMesh> CreateMeshFunc(Func<Vector3f, Vector3f> vertexPostProcessor, Func<Vector2f, Vector2f> uvAdjustor)
        {
            return () => new ExpandingTerrainMesh(vertexPostProcessor, uvAdjustor);
        }

        public IPooledTerrainMesh Get(int arg) => Get();
    }
}