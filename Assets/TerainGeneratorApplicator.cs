using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using UnityEngine.Profiling;

public class TerainGeneratorApplicator : IDisposable
{
    private ITerainGeneratorService _serviceCache;

    private LinkedList<MeshFilter> _meshes = new LinkedList<MeshFilter>();

    public TerainGeneratorApplicator()
    {
    }

    public void UpdateMesh(TerainGeneratorOptions options)
    {
        //Debug.Log("Updating mesh!");


        Profiler.BeginSample("Sampling mesh");
        var service = GetTerainGeneratorService(options);
        IDictionary<Vector2i, ITriangleMesh> triangleMeshes = service.Sample(options);
        Profiler.EndSample();

        if (triangleMeshes != null)
        {
            Profiler.BeginSample("Setting Mesh");

            int validCount = options.GroupsToUpdate.Count;

            //Debug.LogFormat("Valid meshes = {0}", validCount);
            while (_meshes.Count > validCount)
            {
                GameObject.Destroy(_meshes.Last.Value.gameObject);
                _meshes.RemoveLast();
            }

            while (_meshes.Count < validCount)
            {
                string name = string.Format("group_{0}", _meshes.Count);
                var tile = new GameObject(name);
                tile.transform.SetParent(options.ParentContainer.transform, false);
                var meshFilter = tile.GetComponent<MeshFilter>();
                if (meshFilter == null)
                {
                    meshFilter = tile.AddComponent<MeshFilter>();
                }
                var meshRenderer = tile.GetComponent<MeshRenderer>();
                if (meshRenderer == null)
                {
                    meshRenderer = tile.AddComponent<MeshRenderer>();
                }

                var drawBounds = tile.AddComponent<DrawBounds>();

                meshRenderer.material = options.Material;

                if (meshFilter.sharedMesh == null)
                {
                    meshFilter.mesh = new Mesh();
                }
                var mesh = meshFilter.sharedMesh;
                mesh.MarkDynamic();

                _meshes.AddLast(meshFilter);
            }

            LinkedListNode<MeshFilter> item = _meshes.First;

            foreach (var group in options.GroupsToUpdate)
            {
                ITriangleMesh triangleMesh;
                if (triangleMeshes.TryGetValue(group, out triangleMesh) && triangleMesh != null)
                {
                    triangleMesh.UpdateMesh(item.Value.sharedMesh);
                }
                item = item.Next;
            }
            Profiler.EndSample();
        }
    }


    private ITerainGeneratorService GetTerainGeneratorService(TerainGeneratorOptions options)
    {
        if (options.ComputeAsync)
        {
            if (_serviceCache == null)
            {
                //Debug.Log("Creating new AsyncTerainGeneratorService");
                _serviceCache = new AsyncTerainGeneratorService();
            }
            else if (!(_serviceCache is AsyncTerainGeneratorService))
            {
                //Debug.Log("Overriding with AsyncTerainGeneratorService");
                _serviceCache.Dispose();
                _serviceCache = new AsyncTerainGeneratorService();
            }
        }
        else
        {
            if (_serviceCache == null)
            {
                //Debug.Log("Creating new TerainGeneratorService");
                _serviceCache = new TerainGeneratorService();
            }
            else if (!(_serviceCache is TerainGeneratorService))
            {
                //Debug.Log("Overriding with TerainGeneratorService");
                _serviceCache.Dispose();
                _serviceCache = new TerainGeneratorService();
            }
        }
        return _serviceCache;
    }

    public void Dispose()
    {
        if (_serviceCache != null)
        {
            _serviceCache.Dispose();
            _serviceCache = null;
        }
    }
}
