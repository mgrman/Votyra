using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Profiling;

public class TerainGenerator : ITerainGenerator
{
    private MatrixWithOffset<int> _samplesCache;
    private MatrixWithOffset<HeightData> _resultsCache;

    private TerainOptions _old_options;

    public IList<ITriangleMesh> Generate(TerainOptions options)
    {
        if (!options.IsValid)
        {
            return null;
        }
        if (_old_options != null && !options.IsChanged(_old_options))
        {
            return null;
        }
        _old_options = options;

        int cellInGroupCount_x = options.CellInGroupCount.x;
        int cellInGroupCount_y = options.CellInGroupCount.y;
        options.TerainMesher.Initialize(options);

        var meshes = Pool.Meshes2.GetObject(new Pool.MeshKey(options.GroupsToUpdate.Count, options.TerainMesher.TriangleCount));

        int groupIndex = -1;
        foreach (var group in options.GroupsToUpdate)
        {
            groupIndex++;

            if (Thread.CurrentThread == TerainGeneratorBehaviour.UnityThread)
            {
                Profiler.BeginSample("Other");
            }
            var samples = GetCachedSamples(options);
            var results = GetCachedResults(options);

            var group_area = new Rect(group.x * cellInGroupCount_x, group.y * cellInGroupCount_y, cellInGroupCount_x, cellInGroupCount_y);
            if (Thread.CurrentThread == TerainGeneratorBehaviour.UnityThread)
            {
                Profiler.BeginSample("ImageSampler.Sample()");
            }
            //take image and sample it
            options.ImageSampler.Sample(samples, options.Image, group_area, options.Time);

            if (Thread.CurrentThread == TerainGeneratorBehaviour.UnityThread)
            {
                Profiler.EndSample();
            }
            var mesh = meshes[groupIndex];
            options.TerainMesher.InitializeGroup(group,mesh);

            if (Thread.CurrentThread == TerainGeneratorBehaviour.UnityThread)
            {
                Profiler.EndSample();
            }
            for (int cellInGroup_x = -1; cellInGroup_x < cellInGroupCount_x; cellInGroup_x++)
            {
                for (int cellInGroup_y = -1; cellInGroup_y < cellInGroupCount_y; cellInGroup_y++)
                {
                    Vector2i cellInGroup = new Vector2i(cellInGroup_x, cellInGroup_y);

                    if (Thread.CurrentThread == TerainGeneratorBehaviour.UnityThread)
                    {
                        Profiler.BeginSample("TerainAlgorithm.Process()");
                    }
                    //compute cell using alg
                    int x0y0 = samples[cellInGroup_x, cellInGroup_y];
                    int x0y1 = samples[cellInGroup_x, cellInGroup_y + 1];
                    int x1y0 = samples[cellInGroup_x + 1, cellInGroup_y];
                    int x1y1 = samples[cellInGroup_x + 1, cellInGroup_y + 1];
                    HeightData data = new HeightData(x0y0, x0y1, x1y0, x1y1);
                    data = options.TerainAlgorithm.Process(data);
                    results[cellInGroup_x, cellInGroup_y] = data;

                    if (Thread.CurrentThread == TerainGeneratorBehaviour.UnityThread)
                    {
                        Profiler.EndSample();
                    }

                    if (Thread.CurrentThread == TerainGeneratorBehaviour.UnityThread)
                    {
                        Profiler.BeginSample("TerainMesher.AddCell()");
                    }
                    //process cell to mesh      
                    options.TerainMesher.AddCell(data, results, cellInGroup);

                    if (Thread.CurrentThread == TerainGeneratorBehaviour.UnityThread)
                    {
                        Profiler.EndSample();
                    }
                }
            }
            mesh.FinilizeMesh();
        }
        options.Dispose();
        return meshes;
    }

    private MatrixWithOffset<int> GetCachedSamples(TerainOptions options)
    {
        var pointCount = options.CellInGroupCount + 1;

        MatrixWithOffset<int> samples;
        if (_samplesCache != null && _samplesCache.IsSameSize(pointCount, Vector2i.One))
        {
            samples = _samplesCache;
        }
        else
        {
            samples = new MatrixWithOffset<int>(pointCount, Vector2i.One);
            _samplesCache = samples;
            //Debug.Log("Creating new Samples[,]");
        }
        return samples;
    }

    private MatrixWithOffset<HeightData> GetCachedResults(TerainOptions options)
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

}