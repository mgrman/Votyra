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
    public class FixedTerrainMeshPool : PoolWithImplicitKey<uint, ITerrainMesh>, ITerrainMeshPool
    {
        public FixedTerrainMeshPool(ITerrainVertexPostProcessor vertexPostProcessor=null, ITerrainUVPostProcessor uvAdjustor = null)
            : base(CreateMeshFunc(vertexPostProcessor == null ? (Func<Vector3f, Vector3f>) null : vertexPostProcessor.PostProcessVertex, uvAdjustor == null ? (Func<Vector2f, Vector2f>) null : uvAdjustor.ProcessUV), GetKey)
        {
        }

        private static Func<uint, FixedTerrainMesh2i> CreateMeshFunc(Func<Vector3f, Vector3f> vertexPostProcessor, Func<Vector2f, Vector2f> uvAdjustor)
        {
            return (triangleCount) => new FixedTerrainMesh2i(triangleCount, vertexPostProcessor, uvAdjustor);
        }

        private static uint GetKey(ITerrainMesh terrainMesh)
        {
            return terrainMesh.TriangleCount;
        }
    }
}