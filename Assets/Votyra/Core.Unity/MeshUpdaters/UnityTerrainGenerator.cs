using System.Collections.Generic;
using Votyra.Core.MeshUpdaters;
using Votyra.Core.Models;
using Votyra.Core.Pooling;
using Votyra.Core.Profiling;
using Votyra.Core.TerrainGenerators.TerrainMeshers;
using Votyra.Core.TerrainMeshes;

namespace Votyra.Core.TerrainGenerators
{
    public class UnityTerrainGenerator<TFrameData, TGroupKey> : IUnityTerrainGenerator<TFrameData, TGroupKey>
        where TFrameData : IFrameData
    {
        private readonly ITerrainGenerator<TFrameData, TGroupKey> _terrainGenerator;
        private readonly ITerrainMeshConverter _meshConverter;
        private readonly IProfiler _profiler;

        public UnityTerrainGenerator(ITerrainGenerator<TFrameData, TGroupKey> terrainGenerator, ITerrainMeshConverter meshConverter, IProfiler profiler)
        {
            _terrainGenerator = terrainGenerator;
            _meshConverter = meshConverter;
            _profiler = profiler;
        }

        public IReadOnlyPooledDictionary<TGroupKey, UnityMesh> Generate(TFrameData data, IEnumerable<TGroupKey> groupsToUpdate)
        {
            PooledDictionary<TGroupKey, UnityMesh> meshes;
            using (_profiler.Start("init"))
            {
                meshes = PooledDictionary<TGroupKey, UnityMesh>.Create();
            }
            _terrainGenerator.Generate(data, groupsToUpdate, (group, mesh) => meshes[group] = _meshConverter.GetUnityMesh(mesh, null));
            return meshes;
        }
    }
}