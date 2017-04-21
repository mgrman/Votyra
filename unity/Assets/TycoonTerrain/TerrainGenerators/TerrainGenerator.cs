using System.Collections.Generic;
using TycoonTerrain.Common.Logging;
using TycoonTerrain.Common.Models;
using TycoonTerrain.Common.Profiling;
using TycoonTerrain.Pooling;
using TycoonTerrain.TerrainMeshers.TriangleMesh;

namespace TycoonTerrain.TerrainGenerators
{
    public class TerrainGenerator : ITerrainGenerator
    {
        private readonly ILogger _logger = LoggerFactory.Create<TerrainGenerator>();

        private MatrixWithOffset<ResultHeightData> _resultsCache;

        private TerrainOptions _old_options;

        public IList<ITriangleMesh> GenerateMesh(TerrainOptions options)
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
                            inputData = options.ImageSampler.Sample(options.Image, cell, options.Time);
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
                mesh.FinilizeMesh();
            }

            options.Dispose();
            return meshes;
        }
        
        private MatrixWithOffset<ResultHeightData> GetCachedResults(TerrainOptions options)
        {
            var cellCount = options.CellInGroupCount;

            MatrixWithOffset<ResultHeightData> results;
            if (_resultsCache!=null && _resultsCache.IsSameSize(cellCount, Vector2i.One))
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