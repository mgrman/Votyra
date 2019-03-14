using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Votyra.Core.ImageSamplers;
using Votyra.Core.Models;
using Votyra.Core.Models.ObjectPool;
using Votyra.Core.TerrainGenerators.TerrainMeshers;
using Votyra.Core.TerrainMeshes;

namespace Votyra.Core.Pooling
{
    public class ExpandingTerrainMeshPool : Pool<IPooledTerrainMesh>, ITerrainMeshPool
    {
        public ExpandingTerrainMeshPool(ITerrainVertexPostProcessor vertexPostProcessor = null, ITerrainUVPostProcessor uvAdjustor = null)
            : base(CreateMeshFunc(vertexPostProcessor == null ? (Func<Vector3f, Vector3f>) null : vertexPostProcessor.PostProcessVertex, uvAdjustor == null ? (Func<Vector2f, Vector2f>) null : uvAdjustor.ProcessUV))
        {
        }

        private static Func< ExpandingTerrainMesh> CreateMeshFunc(Func<Vector3f, Vector3f> vertexPostProcessor, Func<Vector2f, Vector2f> uvAdjustor)
        {
            return () => new ExpandingTerrainMesh(vertexPostProcessor, uvAdjustor);
        }

        public IPooledTerrainMesh Get(int arg) => Get();
    }
}