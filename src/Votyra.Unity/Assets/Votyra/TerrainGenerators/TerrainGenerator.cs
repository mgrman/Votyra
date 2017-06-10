using System.Collections.Generic;
using UnityEngine;
using Votyra.Common.Logging;
using Votyra.Common.Models;
using Votyra.Common.Profiling;
using Votyra.Common.Utils;
using Votyra.TerrainMeshers.TriangleMesh;
using Votyra.Unity.Assets.Votyra.Pooling;

namespace Votyra.TerrainGenerators
{
    //TODO
    // Split into TerrainMeshGenerator and TerrainTilesGenerator
    public class TerrainGenerator : ITerrainMeshGenerator, ITerrainTileGenerator
    {
        // private readonly ILogger _logger = LoggerFactory.Create<TerrainGenerator>();

        private MatrixWithOffset<ResultHeightData> _resultsCache;

        IReadOnlyPooledDictionary<Vector2i, ITriangleMesh> ITerrainMeshGenerator.Generate(ITerrainContext options, IEnumerable<Vector2i> groupsToUpdate)
        {
            var meshes = GenerateMeshUsingTilesImpl(options, groupsToUpdate);

            return meshes;
        }

        IReadOnlyPooledCollection<ITerrainGroup> ITerrainTileGenerator.Generate(ITerrainContext options, IEnumerable<Vector2i> groupsToUpdate)
        {
            var terrainGroups = GenerateTilesImpl(options, groupsToUpdate);

            return terrainGroups;
        }

        private IReadOnlyPooledDictionary<Vector2i, ITriangleMesh> GenerateMeshUsingTilesImpl(ITerrainContext options, IEnumerable<Vector2i> groupsToUpdate)
        {
            int cellInGroupCount_x = options.CellInGroupCount.x;
            int cellInGroupCount_y = options.CellInGroupCount.y;
            PooledDictionary<Vector2i, ITriangleMesh> meshes;
            using (ProfilerFactory.Create("init"))
            {
                options.TerrainMesher.Initialize(options);

                meshes = PooledDictionary<Vector2i, ITriangleMesh>.Create();
            }

            using (var terrainGroups = GenerateTilesImpl(options, groupsToUpdate))
            {
                foreach (var group in terrainGroups)
                {
                    IPooledTriangleMesh mesh;
                    MatrixWithOffset<ResultHeightData> results = group.Data;

                    using (ProfilerFactory.Create("Other"))
                    {
                        mesh = options.TerrainMeshFactory(options.TerrainMesher.CellCount);
                        meshes[group.Group] = mesh;

                        options.TerrainMesher.InitializeGroup(group.Group, mesh, results);
                    }
                    for (int cellInGroup_x = -1; cellInGroup_x < cellInGroupCount_x; cellInGroup_x++)
                    {
                        for (int cellInGroup_y = -1; cellInGroup_y < cellInGroupCount_y; cellInGroup_y++)
                        {
                            Vector2i cellInGroup = new Vector2i(cellInGroup_x, cellInGroup_y);

                            using (ProfilerFactory.Create("TerrainMesher.AddCell()"))
                            {
                                //process cell to mesh
                                options.TerrainMesher.AddCell(cellInGroup);
                            }
                        }
                    }
                }
            }

            return meshes;
        }

        private IReadOnlyPooledCollection<ITerrainGroup> GenerateTilesImpl(ITerrainContext options, IEnumerable<Vector2i> groupsToUpdate)
        {
            int cellInGroupCount_x = options.CellInGroupCount.x;
            int cellInGroupCount_y = options.CellInGroupCount.y;
            PooledList<ITerrainGroup> terrainGroups = PooledList<ITerrainGroup>.Create();

            int groupIndex = -1;
            foreach (var group in groupsToUpdate)
            {
                groupIndex++;

                Vector2i firstCell = options.CellInGroupCount * group;

                var groupArea = new Rect2i(firstCell, options.CellInGroupCount);

                PooledTerrainGroup terrainGroup;

                using (ProfilerFactory.Create("Other"))
                {
                    terrainGroup = PooledTerrainGroup.CreateDirty(options.CellInGroupCount);
                    terrainGroup.Clear(group);
                    terrainGroups.Add(terrainGroup);
                }

                MatrixWithOffset<ResultHeightData> terrainGroupData = terrainGroup.Data;
                for (int cellInGroup_x = -1; cellInGroup_x < cellInGroupCount_x; cellInGroup_x++)
                {
                    for (int cellInGroup_y = -1; cellInGroup_y < cellInGroupCount_y; cellInGroup_y++)
                    {
                        Vector2i cellInGroup = new Vector2i(cellInGroup_x, cellInGroup_y);
                        Vector2i cell = cellInGroup + firstCell;

                        HeightData inputData;
                        using (ProfilerFactory.Create("ImageSampler.Sample()"))
                        {
                            //sample image
                            inputData = options.ImageSampler.Sample(options.Image, cell);
                        }

                        using (ProfilerFactory.Create("TerrainAlgorithm.Process()"))
                        {
                            //compute cell using alg
                            terrainGroupData[cellInGroup_x, cellInGroup_y] = options.TerrainAlgorithm.Process(inputData);
                        }
                    }
                }
            }
            return terrainGroups;
        }
    }
}