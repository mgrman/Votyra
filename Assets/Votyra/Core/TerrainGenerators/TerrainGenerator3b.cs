using System.Collections.Generic;
using Votyra.Core.Models;
using Votyra.Core.Pooling;
using Votyra.Core.Profiling;
using Votyra.Core.TerrainGenerators.TerrainMeshers;
using Votyra.Core.TerrainMeshes;

namespace Votyra.Core.TerrainGenerators
{
    public class TerrainGenerator3b : ITerrainGenerator<IFrameData3b, Vector3i>
    {
        private readonly ITerrainMesher3b _mesher;
        private readonly IProfiler _profiler;
        private readonly Vector3i _cellInGroupCount;

        public TerrainGenerator3b(ITerrainMesher3b mesher, ITerrainConfig terrainConfig, IProfiler profiler)
        {
            _mesher = mesher;
            _profiler = profiler;
            _cellInGroupCount = terrainConfig.CellInGroupCount;
        }

        public IReadOnlyPooledDictionary<Vector3i, ITerrainMesh> Generate(IFrameData3b data, IEnumerable<Vector3i> groupsToUpdate)
        {
            var image = data.Image;
            PooledDictionary<Vector3i, ITerrainMesh> meshes;

            using (_profiler.Start("init"))
            {
                _mesher.Initialize(image);

                meshes = PooledDictionary<Vector3i, ITerrainMesh>.Create();
            }

            foreach (var group in groupsToUpdate)
            {
                using (_profiler.Start("Other"))
                {
                    _mesher.InitializeGroup(group);
                }
                _cellInGroupCount.ToRange3i().ForeachPointExlusive(cellInGroup =>
                {
                    using (_profiler.Start("TerrainMesher.AddCell()"))
                    {
                        _mesher.AddCell(cellInGroup);
                    }
                });
                using (_profiler.Start("Other"))
                {
                    meshes[group] = _mesher.GetResultingMesh();
                }
            }

            return meshes;
        }
    }
}