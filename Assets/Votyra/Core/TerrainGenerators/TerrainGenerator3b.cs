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
            int cellInGroupCount_x = _cellInGroupCount.X;
            int cellInGroupCount_y = _cellInGroupCount.Y;
            int cellInGroupCount_z = _cellInGroupCount.Z;
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

                for (int cellInGroup_x = 0; cellInGroup_x < cellInGroupCount_x; cellInGroup_x++)
                {
                    for (int cellInGroup_y = 0; cellInGroup_y < cellInGroupCount_y; cellInGroup_y++)
                    {
                        for (int cellInGroup_z = 0; cellInGroup_z < cellInGroupCount_z; cellInGroup_z++)
                        {
                            Vector3i cellInGroup = new Vector3i(cellInGroup_x, cellInGroup_y, cellInGroup_z);

                            using (_profiler.Start("TerrainMesher.AddCell()"))
                            {
                                //process cell to mesh
                                _mesher.AddCell(cellInGroup);
                            }
                        }
                    }
                }
                using (_profiler.Start("Other"))
                {
                    meshes[group] = _mesher.GetResultingMesh();
                }
            }

            return meshes;
        }
    }
}