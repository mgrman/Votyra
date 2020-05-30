using System;
using Votyra.Core.ImageSamplers;
using Votyra.Core.Models;
using Votyra.Core.TerrainGenerators.TerrainMeshers;
using Votyra.Core.TerrainMeshes;

namespace Votyra.Core.Pooling
{
    public class FixedTerrainMeshPool : Pool<ITerrainMesh2f>, ITerrainMesh2iPool
    {
        public FixedTerrainMeshPool(IInterpolationConfig interpolationConfig, ITerrainConfig terrainConfig, ITerrainVertexPostProcessor vertexPostProcessor = null, ITerrainUvPostProcessor uvAdjustor = null)
            : base(CreateMeshFunc(interpolationConfig.MeshSubdivision, terrainConfig.CellInGroupCount.XY(), vertexPostProcessor == null ? (Func<Vector3f, Vector3f>)null : vertexPostProcessor.PostProcessVertex, uvAdjustor == null ? (Func<Vector2f, Vector2f>)null : uvAdjustor.ProcessUv))
        {
        }

        private static Func<FixedTerrainMesh2i> CreateMeshFunc(Vector2i meshSubdivision, Vector2i cellInGroupCount, Func<Vector3f, Vector3f> vertexPostProcessor, Func<Vector2f, Vector2f> uvAdjustor)
        {
            return () => new FixedTerrainMesh2i(meshSubdivision, cellInGroupCount, vertexPostProcessor, uvAdjustor);
        }
    }
}
