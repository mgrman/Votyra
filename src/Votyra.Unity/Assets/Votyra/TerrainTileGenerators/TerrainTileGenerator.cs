using System.Collections.Generic;
using UnityEngine;
using Votyra.Logging;
using Votyra.Models;
using Votyra.Profiling;
using Votyra.Utils;
using Votyra.TerrainMeshers.TriangleMesh;
using Votyra.Unity.Assets.Votyra.Pooling;
using Votyra.TerrainTileGenerators.TerrainGroups;

namespace Votyra.TerrainTileGenerators
{
    public class TerrainTileGenerator : ITerrainTileGenerator
    {
        public IReadOnlyPooledCollection<ITerrainGroup> Generate(ITerrainTileContext options, IEnumerable<Vector2i> groupsToUpdate)
        {
            var terrainGroups = GenerateTilesImpl(options, groupsToUpdate);

            return terrainGroups;
        }

        private IReadOnlyPooledCollection<ITerrainGroup> GenerateTilesImpl(ITerrainTileContext options, IEnumerable<Vector2i> groupsToUpdate)
        {
            int cellInGroupCount_x = options.CellInGroupCount.x;
            int cellInGroupCount_y = options.CellInGroupCount.y;
            PooledList<ITerrainGroup> terrainGroups = PooledList<ITerrainGroup>.Create();

            int groupIndex = -1;
            foreach (var group in groupsToUpdate)
            {
                groupIndex++;

                Vector2i firstCell = options.CellInGroupCount * group;

                PooledTerrainGroup terrainGroup;

                using (options.ProfilerFactory("Other", this))
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
                        using (options.ProfilerFactory("ImageSampler.Sample()", this))
                        {
                            //sample image
                            inputData = options.ImageSampler.Sample(options.Image, cell);
                        }

                        using (options.ProfilerFactory("TerrainAlgorithm.Process()", this))
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