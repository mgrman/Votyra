using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Profiling;

public class TerrainGenerator : ITerrainGenerator
{
    private MatrixWithOffset<ResultHeightData> _resultsCache;

    private TerrainOptions _old_options;

    public IList<ITriangleMesh> Generate(TerrainOptions options)
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
        {
            _old_options.Dispose();
        }
        _old_options = options.Clone();
        bool isUnityThread = Thread.CurrentThread == TerrainGeneratorBehaviour.UnityThread;

        if (isUnityThread)
        {
            Profiler.BeginSample("init");
        }
        int cellInGroupCount_x = options.CellInGroupCount.x;
        int cellInGroupCount_y = options.CellInGroupCount.y;
        options.TerrainMesher.Initialize(options);

        var meshes = Pool.Meshes2.GetObject(new Pool.MeshKey(options.GroupsToUpdate.Count, options.TerrainMesher.TriangleCount));

        if (isUnityThread)
        {
            Profiler.EndSample();
        }
        int groupIndex = -1;
        foreach (var group in options.GroupsToUpdate)
        {
            groupIndex++;

            if (isUnityThread)
            {
                Profiler.BeginSample("Other");
            }
            var results = GetCachedResults(options);

            Vector2i firstCell = options.CellInGroupCount * group;
            
            if (isUnityThread)
            {
                Profiler.BeginSample("ImageSampler.Sample()");
            }
            //take image and sample it

            if (isUnityThread)
            {
                Profiler.EndSample();
            }
            var mesh = meshes[groupIndex];
            options.TerrainMesher.InitializeGroup(group,mesh, results);

            if (isUnityThread)
            {
                Profiler.EndSample();
            }
            for (int cellInGroup_x = -1; cellInGroup_x < cellInGroupCount_x; cellInGroup_x++)
            {
                for (int cellInGroup_y = -1; cellInGroup_y < cellInGroupCount_y; cellInGroup_y++)
                {
                    Vector2i cellInGroup = new Vector2i(cellInGroup_x, cellInGroup_y);
                    Vector2i cell = cellInGroup + firstCell;

                    if (isUnityThread)
                    {
                        Profiler.BeginSample("TerrainAlgorithm.Process()");
                    }
                    //compute cell using alg
                    HeightData inputData = options.ImageSampler.Sample(options.Image, cell, options.Time);
                    ResultHeightData outputData = options.TerrainAlgorithm.Process(inputData);
                    results[cellInGroup_x, cellInGroup_y] = outputData;
                    if (isUnityThread)
                    {
                        Profiler.EndSample();
                    }
                    
                    if (isUnityThread)
                    {
                        Profiler.BeginSample("TerrainMesher.AddCell()");
                    }
                    //process cell to mesh      
                    options.TerrainMesher.AddCell(cellInGroup);

                    if (isUnityThread)
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