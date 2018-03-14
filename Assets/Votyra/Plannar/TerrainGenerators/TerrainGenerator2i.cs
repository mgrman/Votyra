using System.Collections.Generic;
using Votyra.Core.Models;
using Votyra.Core.Pooling;
using Votyra.Core.TerrainMeshes;
using Votyra.Plannar.TerrainGenerators.TerrainMeshers;

namespace Votyra.Plannar.TerrainGenerators
{
    public class TerrainGenerator2i : ITerrainGenerator2i
    {
        ITerrainMesher2i _mesher;
        public TerrainGenerator2i(ITerrainMesher2i mesher)
        {
            _mesher = mesher;
        }

        public IReadOnlyPooledDictionary<Vector2i, ITerrainMesh> Generate(ITerrainGeneratorContext2i options, IEnumerable<Vector2i> groupsToUpdate)
        {
            int cellInGroupCount_x = options.CellInGroupCount.x;
            int cellInGroupCount_y = options.CellInGroupCount.y;
            PooledDictionary<Vector2i, ITerrainMesh> meshes;

            using(options.ProfilerFactory("init", this))
            {
                _mesher.Initialize(options);

                meshes = PooledDictionary<Vector2i, ITerrainMesh>.Create();
            }

            foreach (var group in groupsToUpdate)
            {
                using(options.ProfilerFactory("Other", this))
                {
                    _mesher.InitializeGroup(group);
                }
                for (int cellInGroup_x = 0; cellInGroup_x < cellInGroupCount_x; cellInGroup_x++)
                {
                    for (int cellInGroup_y = 0; cellInGroup_y < cellInGroupCount_y; cellInGroup_y++)
                    {
                        Vector2i cellInGroup = new Vector2i(cellInGroup_x, cellInGroup_y);

                        using(options.ProfilerFactory("TerrainMesher.AddCell()", this))
                        {
                            //process cell to mesh
                            _mesher.AddCell(cellInGroup);
                        }
                    }
                }
                using(options.ProfilerFactory("Other", this))
                {
                    meshes[group] = _mesher.GetResultingMesh();
                }
            }

            return meshes;
        }
    }
}