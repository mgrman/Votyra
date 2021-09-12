using System;
using Votyra.Core.ImageSamplers;
using Votyra.Core.Models;
using Votyra.Core.Pooling;
using Votyra.Core.TerrainGenerators.TerrainMeshers;
using Zenject;

namespace Votyra.Core.Unity.TerrainMeshes
{
    public class FixedTerrainMeshFactory : ITerrainMeshFactory
    {
        public FixedTerrainMeshFactory(IInterpolationConfig interpolationConfig, ITerrainConfig terrainConfig, [InjectOptional]  ITerrainVertexPostProcessor vertexPostProcessor,[InjectOptional]  ITerrainUVPostProcessor uvPostProcessor)
        {
            _interpolationConfig = interpolationConfig;
            _vertexPostProcessor = vertexPostProcessor;
            _uvPostProcessor = uvPostProcessor;
            _cellInGroupCount = terrainConfig.CellInGroupCount;
        }

        private readonly IInterpolationConfig _interpolationConfig;
        private readonly ITerrainVertexPostProcessor _vertexPostProcessor;

        private readonly Vector2i _cellInGroupCount;
        private readonly ITerrainUVPostProcessor _uvPostProcessor;

        public IPooledTerrainMesh CreatePooledTerrainMesh()
        {
            var triangleCount = _cellInGroupCount.AreaSum * 2 * _interpolationConfig.MeshSubdivision *
                                _interpolationConfig.MeshSubdivision;
            var pooledMesh = PooledTerrainMeshWithFixedCapacityContainer<FixedUnityTerrainMesh2i>.CreateDirty(triangleCount);

            pooledMesh.Mesh.Initialize(
                _vertexPostProcessor == null ? (Func<Vector3f, Vector3f>) null : _vertexPostProcessor.PostProcessVertex,
                _uvPostProcessor == null ? (Func<Vector2f, Vector2f>) null : _uvPostProcessor.ProcessUV);
            return pooledMesh;
        }
    }
}