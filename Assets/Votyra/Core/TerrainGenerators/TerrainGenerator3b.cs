using System.Collections.Generic;
using Votyra.Core.Models;
using Votyra.Core.Pooling;
using Votyra.Core.TerrainMeshes;
using Votyra.Core.TerrainGenerators.TerrainMeshers;

namespace Votyra.Core.TerrainGenerators
{
    public class TerrainGenerator3b<TMesher> : ITerrainGenerator3b
        where TMesher : ITerrainMesher3b, new()
    {
        public IReadOnlyPooledDictionary<Vector3i, ITerrainMesh> Generate(ITerrainGeneratorContext3b options, IEnumerable<Vector3i> groupsToUpdate)
        {
            int cellInGroupCount_x = options.CellInGroupCount.x;
            int cellInGroupCount_y = options.CellInGroupCount.y;
            int cellInGroupCount_z = options.CellInGroupCount.z;
            PooledDictionary<Vector3i, ITerrainMesh> meshes;

            TMesher mesher;

            using (options.ProfilerFactory("init", this))
            {
                mesher = new TMesher();
                mesher.Initialize(options);

                meshes = PooledDictionary<Vector3i, ITerrainMesh>.Create();
            }

            foreach (var group in groupsToUpdate)
            {
                using (options.ProfilerFactory("Other", this))
                {
                    mesher.InitializeGroup(group);
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
                                mesher.AddCell(cellInGroup);
                            }
                        }
                    }
                }
                using (options.ProfilerFactory("Other", this))
                {
                    meshes[group] = mesher.GetResultingMesh();
                }
            }

            return meshes;
        }
    }
}