using System.Collections.Generic;
using Votyra.Core.Models;
using Votyra.Core.Pooling;
using Votyra.Core.Profiling;
using Votyra.Core.TerrainGenerators.TerrainMeshers;
using Votyra.Core.TerrainMeshes;

namespace Votyra.Core.TerrainGenerators
{
    public class TerrainGenerator2i : ITerrainGenerator<IFrameData2i, Vector2i>
    {
        private readonly ITerrainMesher2i _mesher;
        private readonly IProfiler _profiler;
        private readonly Vector2i _cellInGroupCount;

        public TerrainGenerator2i(ITerrainMesher2i mesher, ITerrainConfig terrainConfig, IProfiler profiler)
        {
            _mesher = mesher;
            _profiler = profiler;
            _cellInGroupCount = terrainConfig.CellInGroupCount.XY;
        }

        public IReadOnlyPooledDictionary<Vector2i, ITerrainMesh> Generate(IFrameData2i data, IEnumerable<Vector2i> groupsToUpdate)
        {
            var image = data.Image;
            PooledDictionary<Vector2i, ITerrainMesh> meshes;

            using (_profiler.Start("init"))
            {
                _mesher.Initialize(image);

                meshes = PooledDictionary<Vector2i, ITerrainMesh>.Create();
            }

            foreach (var group in groupsToUpdate)
            {
                using (_profiler.Start("Other"))
                {
                    _mesher.InitializeGroup(group);
                }
                _cellInGroupCount.ToRange2i().ForeachPointExlusive(cellInGroup =>
                {
                    using (_profiler.Start("TerrainMesher.AddCell()"))
                    {
                        //process cell to mesh
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