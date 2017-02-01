using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using UnityEngine.Profiling;

public class TerrainMeshUpdater : IMeshUpdater
{

    private List<MeshFilter> _meshFilters = new List<MeshFilter>();

    public TerrainMeshUpdater()
    {
    }

    public void UpdateMesh(MeshOptions options, IList<ITriangleMesh> terrainMeshes)
    {
#if UNITY_EDITOR

        if (!Application.isPlaying)
        {
            _meshFilters.Clear();
            options.ParentContainer.DestroyAllChildren();
        }
#endif

        if (terrainMeshes != null)
        {
            Profiler.BeginSample("Setting Mesh");
            int meshIndex = 0;
            foreach(var terrainMesh in terrainMeshes)
            {
                if(meshIndex >= _meshFilters.Count)
                {
                    CreateMeshObject(options);
                }

                var meshFilter = _meshFilters[meshIndex];
                ITriangleMesh triangleMesh = terrainMesh;
                if(triangleMesh!=null)
                {
                    triangleMesh.UpdateMesh(meshFilter.sharedMesh);
                }

                meshIndex++;
            }

            Pool.Meshes2.ReturnObject(terrainMeshes,new Pool.MeshKey(terrainMeshes.Count,terrainMeshes.Count==0?0: terrainMeshes[0].TriangleCount));

            int terrainMeshCount = meshIndex;
            for (int toDeleteIndex= _meshFilters.Count-1; toDeleteIndex >= terrainMeshCount; toDeleteIndex++)
            {
                _meshFilters[toDeleteIndex].gameObject.Destroy();
                _meshFilters.RemoveAt(toDeleteIndex);
            }
            
            Profiler.EndSample();
        }

        options.Dispose();
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
