using System;
using System.Threading;
using UnityEngine;
using Votyra.Core.ImageSamplers;
using Votyra.Core.MeshUpdaters;
using Votyra.Core.Models;
using Votyra.Core.Pooling;
using Votyra.Core.TerrainGenerators.TerrainMeshers;
using Zenject;

namespace Votyra.Core.TerrainMeshes
{
    public class ExpandingTerrainMeshFactory : ITerrainMeshFactory
    {
        public ExpandingTerrainMeshFactory([InjectOptional]  ITerrainVertexPostProcessor vertexPostProcessor,[InjectOptional]  ITerrainUVPostProcessor uvPostProcessor)
        {
            _vertexPostProcessor = vertexPostProcessor;
            _uvPostProcessor = uvPostProcessor;
        }

        private readonly ITerrainVertexPostProcessor _vertexPostProcessor;

        private readonly ITerrainUVPostProcessor _uvPostProcessor;

        public IPooledTerrainMesh CreatePooledTerrainMesh()
        {
            var pooledMesh = PooledTerrainMeshContainer<ExpandingUnityTerrainMesh>.CreateDirty();

            pooledMesh.Mesh.Initialize(
                _vertexPostProcessor == null ? (Func<Vector3f, Vector3f>) null : _vertexPostProcessor.PostProcessVertex,
                _uvPostProcessor == null ? (Func<Vector2f, Vector2f>) null : _uvPostProcessor.ProcessUV);
            return pooledMesh;
        }
    }
}