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
    public class UnityTerrainGenerator2i : IUnityTerrainGenerator2i
    {
        private readonly ITerrainGenerator2i _terrainGenerator;
        private readonly ITerrainMeshConverter _meshConverter;

        public UnityTerrainGenerator2i(ITerrainGenerator2i terrainGenerator, ITerrainMeshConverter meshConverter, IProfiler profiler)
        {
            _terrainGenerator = terrainGenerator;
            _meshConverter = meshConverter;
        }

        public UnityMesh Generate(Vector2i group, IImage2f image, IMask2e mask)
        {
            var mesh = _terrainGenerator.Generate(group, image, mask);
            var convertedMesh = _meshConverter.GetUnityMesh(mesh, null);

            return convertedMesh;
        }
    }
}