using System;
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
        private readonly Vector2i _cellInGroupCount;
        private readonly ITerrainMesher2f _mesher;
        private readonly IProfiler _profiler;

        public TerrainGenerator2i(ITerrainMesher2f mesher, ITerrainConfig terrainConfig, IProfiler profiler)
        {
            _mesher = mesher;
            _profiler = profiler;
            _cellInGroupCount = terrainConfig.CellInGroupCount.XY;
        }

        public void Generate(IFrameData2i data, IEnumerable<Vector2i> groupsToUpdate, Action<Vector2i, IPooledTerrainMesh> onMeshCreated)
        {
            var image = data.Image;
            var mask = data.Mask;

            using (_profiler.Start("init"))
            {
                _mesher.Initialize(image, mask);
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
                    onMeshCreated(group, _mesher.GetResultingMesh());
                }
            }
        }
    }
}