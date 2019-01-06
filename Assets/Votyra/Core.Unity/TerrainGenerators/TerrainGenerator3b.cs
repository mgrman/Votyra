using Votyra.Core.Images;
using Votyra.Core.Models;
using Votyra.Core.Pooling;
using Votyra.Core.Profiling;
using Votyra.Core.TerrainGenerators.TerrainMeshers;

namespace Votyra.Core.TerrainGenerators
{
    public class TerrainGenerator3b : ITerrainGenerator3b
    {
        private readonly Vector3i _cellInGroupCount;
        private readonly ITerrainMesher3b _mesher;
        private readonly IProfiler _profiler;

        public TerrainGenerator3b(ITerrainMesher3b mesher, ITerrainConfig terrainConfig, IProfiler profiler)
        {
            _mesher = mesher;
            _profiler = profiler;
            _cellInGroupCount = terrainConfig.CellInGroupCount;
        }

        public IPooledTerrainMesh Generate(Vector3i group, IImage3b image)
        {
            using (_profiler.Start("init"))
            {
                _mesher.Initialize(image);
            }

            using (_profiler.Start("Other"))
            {
                _mesher.InitializeGroup(group);
            }

            _cellInGroupCount.ToRange3i()
                .ForeachPointExlusive(cellInGroup =>
                {
                    using (_profiler.Start("TerrainMesher.AddCell()"))
                    {
                        _mesher.AddCell(cellInGroup);
                    }
                });
            using (_profiler.Start("Other"))
            {
                return _mesher.GetResultingMesh();
            }
        }
    }
}