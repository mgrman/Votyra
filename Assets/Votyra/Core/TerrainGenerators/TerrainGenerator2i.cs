using System;
using System.Collections.Generic;
using Votyra.Core.Images;
using Votyra.Core.Models;
using Votyra.Core.Pooling;
using Votyra.Core.Profiling;
using Votyra.Core.TerrainGenerators.TerrainMeshers;
using Votyra.Core.TerrainMeshes;

namespace Votyra.Core.TerrainGenerators
{
    public class TerrainGenerator2i : ITerrainGenerator2i
    {
        private readonly Vector2i _cellInGroupCount;
        private readonly ITerrainMesher2f _mesher;
        private readonly IProfiler _profiler;

        public TerrainGenerator2i(ITerrainMesher2f mesher, ITerrainConfig terrainConfig, IProfiler profiler)
        {
            _mesher = mesher;
            _profiler = profiler;
            _cellInGroupCount = terrainConfig.CellInGroupCount.XY;
        }

        public IPooledTerrainMesh Generate(Vector2i group, IImage2f image, IMask2e mask)
        {
            using (_profiler.Start("Other"))
            {
                return _mesher.GetResultingMesh(group,image, mask);
            }
        }
    }
}