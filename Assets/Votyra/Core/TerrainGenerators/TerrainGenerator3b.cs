using System.Collections.Generic;
using Votyra.Core.Models;
using Votyra.Core.Pooling;
using Votyra.Core.TerrainMeshes;
using Votyra.Core.TerrainGenerators.TerrainMeshers;

namespace Votyra.Core.TerrainGenerators
{
    public class TerrainGenerator3b : ITerrainGenerator3b
    {
        ITerrainMesher3b _mesher;

        public TerrainGenerator3b(ITerrainMesher3b mesher)
        {
            _mesher = mesher;
        }

        public IReadOnlyPooledDictionary<Vector3i, ITerrainMesh> Generate(ITerrainGeneratorContext3b options, IEnumerable<Vector3i> groupsToUpdate)
        {
            int cellInGroupCount_x = options.CellInGroupCount.x;
            int cellInGroupCount_y = options.CellInGroupCount.y;
            int cellInGroupCount_z = options.CellInGroupCount.z;
            PooledDictionary<Vector3i, ITerrainMesh> meshes;


            using (options.ProfilerFactory("init", this))
            {
                _mesher.Initialize(options);

                meshes = PooledDictionary<Vector3i, ITerrainMesh>.Create();
            }

            foreach (var group in groupsToUpdate)
            {
                using (options.ProfilerFactory("Other", this))
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

                            using (options.ProfilerFactory("TerrainMesher.AddCell()", this))
                            {
                                //process cell to mesh
                                _mesher.AddCell(cellInGroup);
                            }
                        }
                    }
                }
                using (options.ProfilerFactory("Other", this))
                {
                    meshes[group] = _mesher.GetResultingMesh();
                }
            }

            return meshes;
        }
    }
}