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
    private IDictionary<Vector2i, ITriangleMesh> _triangleMeshesCache = new Dictionary<Vector2i, ITriangleMesh>();

    private MatrixWithOffset<float> _samplesCache;
    private MatrixWithOffset<HeightData> _resultsCache;

    private TerainOptions _old_options;
    
    public IEnumerable<ITriangleMesh> Generate(TerainOptions options)
    {
        if (!options.IsValid)
        {
            yield break;
        }
        if (_old_options != null && !options.IsChanged(_old_options))
        {
            yield break;
        }
        _old_options = options;

        int cellInGroupCount_x = options.CellInGroupCount.x;
        int cellInGroupCount_y = options.CellInGroupCount.y;
        float groupSize_x = options.GroupSize.x;
        float groupSize_y = options.GroupSize.y;
        foreach (var group in options.GroupsToUpdate)
        {
            var samples = GetCachedSamples(options);
            var results = GetCachedResults(options);
            
            var group_area = new Rect(group.x * groupSize_x, group.y * groupSize_y, groupSize_x, groupSize_y);

            //take image and sample it
            options.ImageSampler.Sample(samples, options.Image, group_area, options.Time);

            options.TerainMesher.Initialize(options, group);

            for (int cellInGroup_x = -1; cellInGroup_x < cellInGroupCount_x; cellInGroup_x++)
            {
                for (int cellInGroup_y = -1; cellInGroup_y < cellInGroupCount_y; cellInGroup_y++)
                {
                    Vector2i cellInGroup = new Vector2i(cellInGroup_x, cellInGroup_y);

                    //compute cell using alg
                    float x0y0 = samples[cellInGroup_x, cellInGroup_y];
                    float x0y1 = samples[cellInGroup_x, cellInGroup_y + 1];
                    float x1y0 = samples[cellInGroup_x + 1, cellInGroup_y];
                    float x1y1 = samples[cellInGroup_x + 1, cellInGroup_y + 1];
                    HeightData data = new HeightData(x0y0, x0y1, x1y0, x1y1);
                    data = options.TerainAlgorithm.Process(data);
                    results[cellInGroup_x, cellInGroup_y] = data;

                    //process cell to mesh      
                    options.TerainMesher.AddCell(data, results, cellInGroup);       
                }
            }
            yield return options.TerainMesher.Result;
        }
    }

    private MatrixWithOffset<float> GetCachedSamples(TerainOptions options)
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