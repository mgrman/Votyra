using System.Collections.Generic;
using UnityEngine;
using Votyra.Logging;
using Votyra.Models;
using Votyra.Profiling;
using Votyra.Utils;
using Votyra.TerrainMeshers.TriangleMesh;
using Votyra.Unity.Assets.Votyra.Pooling;
using Votyra.TerrainTileGenerators.TerrainGroups;
using Votyra.TerrainMeshGenerators.TerrainMeshers;

namespace Votyra.TerrainMeshGenerators
{
    //TODO
    // Split into TerrainMeshGenerator and TerrainTilesGenerator
    public class TerrainMeshGenerator<TMesher> : ITerrainMeshGenerator
        where TMesher : ITerrainMesher, new()
    {
        // private readonly ILogger _logger = LoggerFactory.Create<TerrainGenerator>();

        private MatrixWithOffset<ResultHeightData> _resultsCache;

        public IReadOnlyPooledDictionary<Vector2i, ITerrainMesh> Generate(ITerrainMeshContext options, IReadOnlyPooledCollection<ITerrainGroup> terrainGroups)
        {
            var meshes = GenerateMeshUsingTilesImpl(options, terrainGroups);

            return meshes;
        }


        private IReadOnlyPooledDictionary<Vector2i, ITerrainMesh> GenerateMeshUsingTilesImpl(ITerrainMeshContext options, IReadOnlyPooledCollection<ITerrainGroup> terrainGroups)
        {
            int cellInGroupCount_x = options.CellInGroupCount.x;
            int cellInGroupCount_y = options.CellInGroupCount.y;
            PooledDictionary<Vector2i, ITerrainMesh> meshes;

            TerrainMesher mesher;

            using (options.ProfilerFactory("init", this))
            {
                mesher = new TerrainMesher();
                mesher.Initialize(options);

                meshes = PooledDictionary<Vector2i, ITerrainMesh>.Create();
            }

            foreach (var group in terrainGroups)
            {
                MatrixWithOffset<ResultHeightData> results = group.Data;

                using (options.ProfilerFactory("Other", this))
                {
                    mesher.InitializeGroup(group.Group, results);
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
                    meshes[group.Group] = mesher.GetResultingMesh();
                }
            }

            return meshes;
        }
    }
}