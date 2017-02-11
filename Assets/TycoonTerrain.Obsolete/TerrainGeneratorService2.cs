//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.Linq;
//using System.Threading;
//using UnityEngine;
//using UnityEngine.Profiling;

//public class TerrainGeneratorService2 : ITerainGeneratorService
//{
//    private const int MAX_CELL_COUNT = 60;
//    private const int CELL_TO_TRIANGLE = 3 * 2;
//    private const int CELL_TO_VERTEX = CELL_TO_TRIANGLE * 3;
//    private const int MAX_VERTEX_COUNT = MAX_CELL_COUNT * MAX_CELL_COUNT * CELL_TO_VERTEX;


//    private FixedTriangleMesh[] _triangleMeshesCache;
//    private int _triangleMeshesCache_cellCount = -1;
//    private Matrix<float> _samplesCache;
//    private Matrix<HeightData> _resultsCache;


//    private TerainGeneratorOptions _old_options;

//    public TerrainGeneratorService2()
//    {
//    }

//    public ITriangleMesh[] Sample(TerainGeneratorOptions options)
//    {
//        if (!options.IsValid)
//        {
//            return null;
//        }

//        if (_old_options != null && !options.IsChanged(_old_options))
//        {
//            return null;
//        }

//        _old_options = options;

//        Vector2i global_subdivision = options.Rectangle.size.DivideBy(options.Step).ToIntVector();
//        Vector2i global_pointCount = global_subdivision + 1;
//        Vector2i global_cellCount = global_pointCount - 1;
//        Vector2 step = options.Rectangle.size.DivideBy(global_cellCount);

//        Vector2i groupCount = global_cellCount / MAX_CELL_COUNT;

//        Matrix<float> samples = GetCachedSamples(global_pointCount);
//        Matrix<HeightData> results = GetCachedResults(global_cellCount);

//        Vector2 offset = options.Rectangle.position;

//        Vector3 boundsCenter = options.Rectangle.center.ToVector3(options.Image.RangeZ.Center);
//        Vector3 boundsSize = options.Rectangle.size.ToVector3(options.Image.RangeZ.Size);
//        Bounds meshBounds = new Bounds(boundsCenter, boundsSize);
        
//        ITriangleMesh[] meshes = GetFixedCachedTriangleMesh(global_cellCount, step, offset, meshBounds);

//        options.Sampler.Sample(samples, options.Image, options.Rectangle, options.Time);
//        //Profiler.EndSample();


//        //Profiler.BeginSample("Creating mesh");
//        //int quadIndex = 0;

//        for (int group_x = 0; group_x < groupCount.x; group_x++)
//        {
//            for (int group_y = 0; group_y < groupCount.y; group_y++)
//            {
//                Vector2i group = new Vector2i(group_x, group_y);
//                int group_lin = Linearize(group, groupCount);

//                var cellCount = GetCellCountInGroup(group, groupCount, global_cellCount);

//                var rectangle = new Rect(group_x * MAX_CELL_COUNT * step.x+ offset.x, group_y * MAX_CELL_COUNT * step.y+ offset.y, cellCount.x * step.x, cellCount.y * step.y);


//                int quadIndex = 0;
//                for (int cellIndex_x = 0; cellIndex_x < cellCount.x; cellIndex_x++)
//                {
//                    float position_x = rectangle.xMin + cellIndex_x * step.x;
//                    for (int cellIndex_y = 0; cellIndex_y < cellCount.y; cellIndex_y++)
//                    {
//                        float position_y = rectangle.yMin + cellIndex_y * step.y;
//                        Vector2 position = new Vector2(position_x, position_y);
//                        Vector2i cellIndex = new Vector2i(cellIndex_x, cellIndex_y );
//                        Vector2i global_cellIndex = new Vector2i(cellIndex_x + group_x * MAX_CELL_COUNT, cellIndex_y + group_y * MAX_CELL_COUNT);

//                        Rect cellArea = new Rect(position, step);
//                        SampleData sampledCell = new SampleData(samples, global_cellIndex, cellArea);
//                        ITriangleMesh mesh = meshes[group_lin];

//                        HeightData heightData = options.MeshGenerator.Generate(sampledCell);
//                        results[global_cellIndex] = heightData;


//                        if (global_cellIndex.Positive)
//                        {
//                            mesh.AddQuad(quadIndex, cellArea, heightData, !heightData.KeepSides != options.FlipTriangles);
//                            quadIndex++;

//                            var minusXres = results[global_cellIndex.x - 1, global_cellIndex.y];

//                            mesh.AddQuad(quadIndex,
//                                new Vector3(position_x, position_y, heightData.x0y0),
//                                new Vector3(position_x, position_y + step.y, heightData.x0y1),
//                                new Vector3(position_x, position_y + step.y, minusXres.x1y1),
//                                new Vector3(position_x, position_y, minusXres.x1y0), false);
//                            quadIndex++;
                            
//                            var minusYres = results[global_cellIndex.x, global_cellIndex.y - 1];

//                            mesh.AddQuad(quadIndex,
//                                new Vector3(position_x + step.x, position_y, heightData.x1y0),
//                                new Vector3(position_x, position_y, heightData.x0y0),
//                                new Vector3(position_x, position_y, minusYres.x0y1),
//                                new Vector3(position_x + step.x, position_y, minusYres.x1y1), false);
//                            quadIndex++;

//                        }
//                    }
//                }
//            }
//        }


//        return meshes;
//    }


//    private Vector2i GetCellCountInGroup(Vector2i groupIndex, Vector2i groupCount, Vector2i global_cellCount)
//    {
//        int countX = MAX_CELL_COUNT;
//        if (groupIndex.x == groupCount.x - 1)
//        {
//            countX = global_cellCount.x.RemainderUp(MAX_CELL_COUNT);
//        }

//        int countY = MAX_CELL_COUNT;
//        if (groupIndex.y == groupCount.y - 1)
//        {
//            countY = global_cellCount.y.RemainderUp(MAX_CELL_COUNT);
//        }
//        return new Vector2i(countX, countY);
//    }


//    private int Linearize(Vector2i index, Vector2i size)
//    {
//        return index.x * size.y + index.y;
//    }

//    private Matrix<float> GetCachedSamples(Vector2i pointCount)
//    {
//        Matrix<float> samples;
//        if (_samplesCache != null && _samplesCache.IsSameSize(pointCount, Vector2i.Zero))
//        {
//            samples = _samplesCache;
//        }
//        else
//        {
//            samples = new Matrix<float>(pointCount, Vector2i.Zero);
//            _samplesCache = samples;
//            //Debug.Log("Creating new Samples[,]");
//        }
//        return samples;
//    }

//    private Matrix<HeightData> GetCachedResults(Vector2i cellCount)
//    {
//        Matrix<HeightData> results;
//        if (_resultsCache != null && _resultsCache.IsSameSize(cellCount, Vector2i.Zero))
//        {
//            results = _resultsCache;
//        }
//        else
//        {
//            results = new Matrix<HeightData>(cellCount, Vector2i.Zero);
//            _resultsCache = results;
//            //Debug.Log("Creating new HeightData[,]");
//        }
//        return results;
//    }

//    private FixedTriangleMesh[] GetFixedCachedTriangleMesh(Vector2i global_cellCount,Vector2 step,Vector2 offset, Bounds meshBounds)
//    {

//        Vector2i groupCount = global_cellCount / MAX_CELL_COUNT;
//        int groupCountArea = groupCount.AreaSum;

//        FixedTriangleMesh[] meshes;

//        if (_triangleMeshesCache == null || _triangleMeshesCache_cellCount != global_cellCount.AreaSum)
//        {
//            meshes = new FixedTriangleMesh[groupCountArea];

//            for (int group_x = 0; group_x < groupCount.x; group_x++)
//            {
//                for (int group_y = 0; group_y < groupCount.y; group_y++)
//                {
//                    Vector2i group = new Vector2i(group_x, group_y);
//                    int group_lin = Linearize(group, groupCount);

//                    var cellCount = GetCellCountInGroup(group, groupCount, global_cellCount);

//                    int expectedTriangleCount = cellCount.AreaSum * 3 * 2;

//                    var rectangle = new Rect(group_x * MAX_CELL_COUNT * step.x + offset.x, group_y * MAX_CELL_COUNT * step.y + offset.y, cellCount.x * step.x, cellCount.y * step.y);
//                    var bounds = new Bounds(rectangle.center.ToVector3(meshBounds.center.z), rectangle.size.ToVector3(meshBounds.size.z));

//                    var mesh = new FixedTriangleMesh(bounds, expectedTriangleCount);

//                    meshes[group_lin] = mesh;
//                }
//            }
//        }
//        else
//        {
//            meshes = _triangleMeshesCache;
//            for (int group_x = 0; group_x < groupCount.x; group_x++)
//            {
//                for (int group_y = 0; group_y < groupCount.y; group_y++)
//                {
//                    Vector2i group = new Vector2i(group_x, group_y);
//                    int group_lin = Linearize(group, groupCount);

//                    var cellCount = GetCellCountInGroup(group, groupCount, global_cellCount);

//                    var rectangle = new Rect(group_x * MAX_CELL_COUNT * step.x + offset.x, group_y * MAX_CELL_COUNT * step.y + offset.y, cellCount.x * step.x, cellCount.y * step.y);
//                    var bounds = new Bounds(rectangle.center.ToVector3(meshBounds.center.z), rectangle.size.ToVector3(meshBounds.size.z));

//                    meshes[group_lin].Clear(bounds);
//                }
//            }
//        }


//        _triangleMeshesCache = meshes;
//        return meshes;
//    }

//    public void Dispose()
//    {
//    }
//}
