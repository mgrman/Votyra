using System.Collections.Generic;
using Votyra.Core.Images;
using Votyra.Core.MeshUpdaters;
using Votyra.Core.Models;
using Votyra.Core.Pooling;
using Votyra.Core.Profiling;
using Votyra.Core.TerrainGenerators.TerrainMeshers;
using Votyra.Core.TerrainMeshes;

namespace Votyra.Core.TerrainGenerators
{
    public class UnityTerrainGenerator3b : IUnityTerrainGenerator3b
    {
        private readonly ITerrainGenerator3b _terrainGenerator;
        private readonly ITerrainMeshConverter _meshConverter;

        public UnityTerrainGenerator3b(ITerrainGenerator3b terrainGenerator, ITerrainMeshConverter meshConverter, IProfiler profiler)
        {
            _terrainGenerator = terrainGenerator;
            _meshConverter = meshConverter;
        }

        public UnityMesh Generate(Vector3i group, IImage3b image)
        {
            var mesh = _terrainGenerator.Generate(group, image);
            var convertedMesh = _meshConverter.GetUnityMesh(mesh, null);

            return convertedMesh;
        }
    }
}