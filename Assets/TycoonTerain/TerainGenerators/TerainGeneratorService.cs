using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Profiling;

public class TerainGeneratorService : ITerainGeneratorService
{
    private const int MAX_CELL_COUNT = 60;
    private const int CELL_TO_TRIANGLE = 3 * 2;
    private const int CELL_TO_VERTEX = CELL_TO_TRIANGLE * 3;
    private const int MAX_VERTEX_COUNT = MAX_CELL_COUNT * MAX_CELL_COUNT * CELL_TO_VERTEX;

    private IDictionary<Vector2i, ITriangleMesh> _triangleMeshesCache = new Dictionary<Vector2i, ITriangleMesh>();
    private MatrixWithOffset<float> _samplesCache;
    private MatrixWithOffset<HeightData> _resultsCache;


    private TerainGeneratorOptions _old_options;

    public TerainGeneratorService()
    {
    }

    public IDictionary<Vector2i, ITriangleMesh> Sample(TerainGeneratorOptions options)
    {
        if (!options.IsValid)
        {
            return null;
        }
        if (_old_options != null && !options.IsChanged(_old_options))
        {
            return null;
        }

        var meshes = _triangleMeshesCache;
        var samples = GetCachedSamples(options);
        var results = GetCachedResults(options);

        //sample image
        //end

        //Debug.Log(string.Format("Cell count {0}x{1}", options.CellCount.x, options.CellCount.y));

        //int groupCount_x = options.GroupCount.x;
        //int groupCount_y = options.GroupCount.y;
        int cellInGroupCount_x = options.CellInGroupCount.x;
        int cellInGroupCount_y = options.CellInGroupCount.y;
        float cellSize_x = options.CellSize.x;
        float cellSize_y = options.CellSize.y;
        float groupSize_x = options.GroupSize.x;
        float groupSize_y = options.GroupSize.y;
        int trianglesInMesh = options.CellInGroupCount.AreaSum * CELL_TO_TRIANGLE;
        var bounds_center = options.GroupBounds.center;
        var bounds_size = options.GroupBounds.size;

        //int cell_x = -1;
        //int group_i = -1;
        foreach (var group in options.GroupsToUpdate)
        {
            //for (int group_x = 0; group_x < groupCount_x; group_x++)
            //{
            //int cell_y = 0;
            //for (int group_y = 0; group_y < groupCount_y; group_y++)
            //{
            //int group_i = group.x * groupCount_y + group.y;

            var group_area = new Rect(group.x * groupSize_x, group.y * groupSize_y, groupSize_x, groupSize_y);
            options.Sampler.Sample(samples, options.Image, group_area, options.Time);

            ITriangleMesh mesh;
            if (!meshes.TryGetValue(group, out mesh))
            {
                mesh = new FixedTriangleMesh(trianglesInMesh);
                meshes[group] = mesh;
            }
            var bounds = new Bounds(new Vector3
             (
                 bounds_center.x + group.x * groupSize_x,
                 bounds_center.y + group.y * groupSize_y,
                 bounds_center.z
             ), bounds_size);
            mesh.Clear(bounds);


            int quadIndex = 0;

            //int cell_y_inLastGroup = cell_y;

            for (int cellInGroup_x = -1; cellInGroup_x < cellInGroupCount_x; cellInGroup_x++)
            {
                //cell_x++;

                //cell_y = cell_y_inLastGroup;

                int cell_x = group.x * cellInGroupCount_x + cellInGroup_x;

                float x = cell_x * cellSize_x;
                for (int cellInGroup_y = -1; cellInGroup_y < cellInGroupCount_y; cellInGroup_y++)
                {
                    int cell_y = group.y * cellInGroupCount_y + cellInGroup_y;
                    float y = cell_y * cellSize_y;

                    //Debug.Log(string.Format("Processing cell {0}x{1}", cell_x, cell_y));

                    float x0y0 = samples[cellInGroup_x, cellInGroup_y];
                    float x0y1 = samples[cellInGroup_x, cellInGroup_y + 1];
                    float x1y0 = samples[cellInGroup_x + 1, cellInGroup_y];
                    float x1y1 = samples[cellInGroup_x + 1, cellInGroup_y + 1];
                    HeightData data = new HeightData(x0y0, x0y1, x1y0, x1y1);

                    HeightData heightData = options.MeshGenerator.Process(data);
                    results[cellInGroup_x, cellInGroup_y] = heightData;

                    if (cellInGroup_x >= 0 && cellInGroup_y >= 0)
                    {
                        var cell_area = new Rect(x, y, cellSize_x, cellSize_y);

                        mesh.AddQuad(quadIndex, cell_area, heightData, !heightData.KeepSides != options.FlipTriangles);
                        quadIndex++;

                        float minusXres_x1y1;
                        float minusXres_x1y0;
                        //if (cellInGroup_x == 0)
                        //{
                        //    minusXres_x1y1 = heightData.x0y1;
                        //    minusXres_x1y0 = heightData.x0y0;
                        //}
                        //else
                        //{
                        var minusXres = results[cellInGroup_x - 1, cellInGroup_y];
                        minusXres_x1y1 = minusXres.x1y1;
                        minusXres_x1y0 = minusXres.x1y0;
                        // }
                        mesh.AddQuad(quadIndex,
                            new Vector3(x, y, heightData.x0y0),
                            new Vector3(x, y + cellSize_y, heightData.x0y1),
                            new Vector3(x, y + cellSize_y, minusXres_x1y1),
                            new Vector3(x, y, minusXres_x1y0), false);
                        quadIndex++;


                        float minusYres_x0y1;
                        float minusYres_x1y1;
                        //if (cellInGroup_y == 0)
                        //{
                        //    minusYres_x0y1 = heightData.x0y0;
                        //    minusYres_x1y1 = heightData.x1y0;
                        //}
                        //else
                        //{
                        var minusYres = results[cellInGroup_x, cellInGroup_y - 1];
                        minusYres_x0y1 = minusYres.x0y1;
                        minusYres_x1y1 = minusYres.x1y1;
                        //}
                        mesh.AddQuad(quadIndex,
                            new Vector3(x + cellSize_x, y, heightData.x1y0),
                            new Vector3(x, y, heightData.x0y0),
                            new Vector3(x, y, minusYres_x0y1),
                            new Vector3(x + cellSize_x, y, minusYres_x1y1), false);
                        quadIndex++;

                    }
                    //cell_y++;
                }
            }
            //}
        }

        _old_options = options;
        return meshes;
    }

    private MatrixWithOffset<float> GetCachedSamples(TerainGeneratorOptions options)
    {
        var pointCount = options.CellInGroupCount + 1;

        MatrixWithOffset<float> samples;
        if (_samplesCache != null && _samplesCache.IsSameSize(pointCount, Vector2i.One))
        {
            samples = _samplesCache;
        }
        else
        {
            samples = new MatrixWithOffset<float>(pointCount, Vector2i.One);
            _samplesCache = samples;
            //Debug.Log("Creating new Samples[,]");
        }
        return samples;
    }

    private MatrixWithOffset<HeightData> GetCachedResults(TerainGeneratorOptions options)
    {
        var cellCount = options.CellInGroupCount;

        MatrixWithOffset<HeightData> results;
        if (_resultsCache != null && _resultsCache.IsSameSize(cellCount, Vector2i.One))
        {
            results = _resultsCache;
        }
        else
        {
            results = new MatrixWithOffset<HeightData>(cellCount, Vector2i.One);
            _resultsCache = results;
            //Debug.Log("Creating new HeightData[,]");
        }
        return results;
    }

    //private IDictionary<Vector2i, ITriangleMesh> GetMeshesCache(TerainGeneratorOptions options)
    //{
    //    int meshCount = options.GroupCount.AreaSum;

    //    int trianglesInMesh = options.CellInGroupCount.AreaSum * CELL_TO_TRIANGLE;

    //    var meshes = _triangleMeshesCache;
    //    if (meshes == null || meshes.Length != meshCount || meshes[0,0].TriangleCount != trianglesInMesh)
    //    {
    //        meshes = new FixedTriangleMesh[options.GroupCount.x, options.GroupCount.y];

    //        int groupCount_x = options.GroupCount.x;
    //        int groupCount_y = options.GroupCount.y;

    //        var templateBounds = options.GroupBounds;

    //        for (int group_x = 0; group_x < groupCount_x; group_x++)
    //        {
    //            for (int group_y = 0; group_y < groupCount_y; group_y++)
    //            {
    //                var bounds = templateBounds;
    //                bounds.center = new Vector3
    //                    (
    //                        bounds.center.x + group_x * options.GroupSize.x,
    //                        bounds.center.y + group_y * options.GroupSize.y,
    //                        bounds.center.z
    //                    );

    //                meshes[group_x, group_y] = new FixedTriangleMesh(bounds, trianglesInMesh);
    //            }
    //        }
    //    }
    //    else if (options.IsBoundsChanged(_old_options))
    //    {
    //        int groupCount_x = options.GroupCount.x;
    //        int groupCount_y = options.GroupCount.y;

    //        var templateBounds = options.GroupBounds;

    //        for (int group_x = 0; group_x < groupCount_x; group_x++)
    //        {
    //            for (int group_y = 0; group_y < groupCount_y; group_y++)
    //            {
    //                var bounds = templateBounds;
    //                bounds.center = new Vector3
    //                    (
    //                        bounds.center.x + group_x * options.GroupSize.x,
    //                        bounds.center.y + group_y * options.GroupSize.y,
    //                        bounds.center.z
    //                    );

    //                meshes[group_x, group_y].Clear(bounds);
    //            }
    //        }
    //    }
    //    _triangleMeshesCache = meshes;
    //    return meshes;
    //}

    public void Dispose()
    {
    }
}
