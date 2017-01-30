using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using UnityEngine.Profiling;

public class TerainMeshUpdater : IMeshUpdater
{

    private List<MeshFilter> _meshFilters = new List<MeshFilter>();

    public TerainMeshUpdater()
    {
    }

    public void UpdateMesh(MeshOptions options, IEnumerable<ITriangleMesh> terainMeshes)
    {
#if UNITY_EDITOR

        if (!Application.isPlaying)
        {
            _meshFilters.Clear();
            options.ParentContainer.DestroyAllChildren();
        }
#endif

        if (terainMeshes != null)
        {
            Profiler.BeginSample("Setting Mesh");
            int meshIndex = 0;
            foreach(var terainMesh in terainMeshes)
            {
                if(meshIndex >= _meshFilters.Count)
                {
                    CreateMeshObject(options);
                }

                var meshFilter = _meshFilters[meshIndex];
                ITriangleMesh triangleMesh = terainMesh;
                if(triangleMesh!=null)
                {
                    triangleMesh.UpdateMesh(meshFilter.sharedMesh);
                }

                meshIndex++;
            }

            int terainMeshCount = meshIndex;
            for (int toDeleteIndex= _meshFilters.Count-1; toDeleteIndex >= terainMeshCount; toDeleteIndex++)
            {
                _meshFilters[toDeleteIndex].gameObject.Destroy();
                _meshFilters.RemoveAt(toDeleteIndex);
            }
            
            Profiler.EndSample();
        }
    }

    private void CreateMeshObject(MeshOptions options)
    {
        string name = string.Format("group_{0}", _meshFilters.Count);
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
        if (options.DrawBounds)
        {
            tile.AddComponent<DrawBounds>();
        }

        meshRenderer.material = options.Material;

        if (meshFilter.sharedMesh == null)
        {
            meshFilter.mesh = new Mesh();
        }
        var mesh = meshFilter.sharedMesh;
        mesh.MarkDynamic();

        _meshFilters.Add(meshFilter);
    }
}
