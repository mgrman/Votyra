using System.Collections.Generic;
using UnityEngine;
using Votyra.Common.Logging;
using Votyra.Common.Models;
using Votyra.Common.Profiling;
using Votyra.Common.Utils;
using Votyra.Pooling;
using Votyra.TerrainMeshers.TriangleMesh;

namespace Votyra.TerrainGenerators
{
    //TODO
    // Split into TerrainMeshGenerator and TerrainTilesGenerator
    public class TerrainGenerator : ITerrainMeshGenerator, ITerrainTileGenerator
    {
        // private readonly ILogger _logger = LoggerFactory.Create<TerrainGenerator>();

        private MatrixWithOffset<ResultHeightData> _resultsCache;

        private TerrainOptions _old_options;

        IReadOnlyDictionary<Vector2i, ITriangleMesh> IGenerator<TerrainOptions, IReadOnlyDictionary<Vector2i, ITriangleMesh>>.Generate(TerrainOptions options)
        {
            if (!options.IsValid)
            {
                return null;
            }
            if (_old_options != null && !options.IsChanged(_old_options))
            {
                return null;
            }

            // _logger.LogMessage("Recomputing meshes");

            if (_old_options != null)
                _old_options.Dispose();

            _old_options = options.Clone();
            var meshes = GenerateMeshUsingTilesImpl(options);

            options.Dispose();
            return meshes;
        }

        IList<ITerrainGroup> IGenerator<TerrainOptions, IList<ITerrainGroup>>.Generate(TerrainOptions options)
        {
            if (!options.IsValid)
            {
                return null;
            }
            if (_old_options != null && !options.IsChanged(_old_options))
            {
                return null;
            }
            if (_old_options != null)
                _old_options.Dispose();

            _old_options = options.Clone();

            var terrainGroups = GenerateTilesImpl(options);

            options.Dispose();
            return terrainGroups;
        }

        private IList<ITriangleMesh> GenerateMeshImpl(TerrainOptions options)
        {
            int cellInGroupCount_x = options.CellInGroupCount.x;
            int cellInGroupCount_y = options.CellInGroupCount.y;
            IList<ITriangleMesh> meshes;
            using (ProfilerFactory.Create("init"))
            {
                options.TerrainMesher.Initialize(options);

                meshes = Pool.Meshes2.GetObject(new Pool.MeshKey(options.GroupsToUpdate.Count, options.TerrainMesher.TriangleCount));
            }

            int groupIndex = -1;
            foreach (var group in options.GroupsToUpdate)
            {
                groupIndex++;

                Vector2i firstCell = options.CellInGroupCount * group;
                ITriangleMesh mesh;
                MatrixWithOffset<ResultHeightData> results;
                using (ProfilerFactory.Create("Other"))
                {
                    results = GetCachedResults(options);

                    mesh = meshes[groupIndex];
                    options.TerrainMesher.InitializeGroup(group, mesh, results);
                }
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
                            results[cellInGroup_x, cellInGroup_y] = options.TerrainAlgorithm.Process(inputData);
                        }

                        using (ProfilerFactory.Create("TerrainMesher.AddCell()"))
                        {
                            //process cell to mesh
                            options.TerrainMesher.AddCell(cellInGroup);
                        }
                    }
                }
                mesh.FinalizeMesh();
            }

            return meshes;
        }

        private IReadOnlyDictionary<Vector2i, ITriangleMesh> GenerateMeshUsingTilesImpl(TerrainOptions options)
        {
            int cellInGroupCount_x = options.CellInGroupCount.x;
            int cellInGroupCount_y = options.CellInGroupCount.y;
            IReadOnlyDictionary<Vector2i, ITriangleMesh> readonlyMeshes;
            IDictionary<Vector2i, ITriangleMesh> meshes;
            using (ProfilerFactory.Create("init"))
            {
                options.TerrainMesher.Initialize(options);

                readonlyMeshes = Pool.Meshes3.GetObject(new Pool.MeshKey2(options.TerrainMesher.TriangleCount));
                meshes = readonlyMeshes as IDictionary<Vector2i, ITriangleMesh>;
                meshes.Clear();
            }

            var terrainGroups = GenerateTilesImpl(options);

            foreach (var group in terrainGroups)
            {
                ITriangleMesh mesh;
                MatrixWithOffset<ResultHeightData> results = group.Data;

                if (group.Invalidated)
                {
                    using (ProfilerFactory.Create("Other"))
                    {
                        if (!meshes.TryGetValue(group.Group, out mesh))
                        {
                            mesh = Pool.Meshes.GetObject(options.TerrainMesher.TriangleCount);
                            meshes[group.Group] = mesh;
                        }
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
                    mesh.FinalizeMesh();
                }
                else
                {
                    meshes[group.Group] = null;
                }
            }

            return readonlyMeshes;
        }

        private IList<ITerrainGroup> GenerateTilesImpl(TerrainOptions options)
        {
            int cellInGroupCount_x = options.CellInGroupCount.x;
            int cellInGroupCount_y = options.CellInGroupCount.y;
            IList<ITerrainGroup> terrainGroups;
            using (ProfilerFactory.Create("init"))
            {
                terrainGroups = Pool.TerrainGroups.GetObject(new Pool.TerrainGroupKey(options.GroupsToUpdate.Count, options.CellInGroupCount));
            }


            int invalidatedCount = 0;
            int groupIndex = -1;
            foreach (var group in options.GroupsToUpdate)
            {
                groupIndex++;

                Vector2i firstCell = options.CellInGroupCount * group;

                var groupArea = new UnityEngine.Rect(firstCell.x, firstCell.y, options.CellInGroupCount.x, options.CellInGroupCount.y);

                ITerrainGroup terrainGroup;

                using (ProfilerFactory.Create("Other"))
                {
                    terrainGroup = terrainGroups[groupIndex];
                    bool recomputeArea = groupArea.Overlaps(options.TransformedInvalidatedArea);
                    terrainGroup.Clear(group, recomputeArea);
                }

                if (terrainGroup.Invalidated)
                {
                    invalidatedCount++;
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
            }
            // Debug.Log($"Invalidated count {invalidatedCount}. {options.TransformedInvalidatedArea}");
            return terrainGroups;
        }

        private MatrixWithOffset<ResultHeightData> GetCachedResults(TerrainOptions options)
        {
            var cellCount = options.CellInGroupCount;

            MatrixWithOffset<ResultHeightData> results;
            if (_resultsCache != null && _resultsCache.IsSameSize(cellCount, Vector2i.One))
            {
                results = _resultsCache;
            }
            else
            {
                results = new MatrixWithOffset<ResultHeightData>(cellCount, Vector2i.One);
                _resultsCache = results;
                //Debug.Log("Creating new HeightData[,]");
            }
            return results;
        }
    }
}