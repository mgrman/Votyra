using System.Collections.Generic;
using UnityEngine;
using Votyra.Logging;
using Votyra.Models;
using Votyra.Profiling;
using Votyra.Utils;
using Votyra.TerrainGenerators.TerrainMeshers.TerrainMeshes;
using Votyra.Unity.Assets.Votyra.Pooling;
using Votyra.TerrainGenerators.TerrainMeshers;

namespace Votyra.TerrainGenerators
{
    public class TerrainGenerator2i<TMesher> : ITerrainGenerator2i
        where TMesher : ITerrainMesher2i, new()
    {
        public IReadOnlyPooledDictionary<Vector2i, ITerrainMesh2> Generate(ITerrainGeneratorContext2i options, IEnumerable<Vector2i> groupsToUpdate)
        {
            var meshes = GenerateMeshUsingTilesImpl(options, groupsToUpdate);

            return meshes;
        }


        private IReadOnlyPooledDictionary<Vector2i, ITerrainMesh2> GenerateMeshUsingTilesImpl(ITerrainGeneratorContext2i options, IEnumerable<Vector2i> groupsToUpdate)
        {
            int cellInGroupCount_x = options.CellInGroupCount.x;
            int cellInGroupCount_y = options.CellInGroupCount.y;
            PooledDictionary<Vector2i, ITerrainMesh2> meshes;

            TerrainMesher2i mesher;

            using (options.ProfilerFactory("init", this))
            {
                mesher = new TerrainMesher2i();
                mesher.Initialize(options);

                meshes = PooledDictionary<Vector2i, ITerrainMesh2>.Create();
            }

            foreach (var group in groupsToUpdate)
            {
                using (options.ProfilerFactory("Other", this))
                {
                    mesher.InitializeGroup(group);
                }
                for (int cellInGroup_x = -1; cellInGroup_x < cellInGroupCount_x; cellInGroup_x++)
                {
                    for (int cellInGroup_y = -1; cellInGroup_y < cellInGroupCount_y; cellInGroup_y++)
                    {
                        Vector2i cellInGroup = new Vector2i(cellInGroup_x, cellInGroup_y);

                        using (options.ProfilerFactory("TerrainMesher.AddCell()", this))
                        {
                            //process cell to mesh
                            mesher.AddCell(cellInGroup);
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