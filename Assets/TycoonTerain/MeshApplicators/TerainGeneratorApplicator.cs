using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using UnityEngine.Profiling;

public class TerainGeneratorApplicator : IDisposable, ITerainGeneratorApplicator
{

    private LinkedList<MeshFilter> _meshes = new LinkedList<MeshFilter>();

    public TerainGeneratorApplicator()
    {
    }

    public void UpdateMesh(TerainGeneratorOptions options, IDictionary<Vector2i, ITriangleMesh> triangleMeshes)
    {
#if UNITY_EDITOR

        if (!Application.isPlaying)
        {
            _meshes.Clear();
            options.ParentContainer.DestroyAllChildren();
        }
#endif

        if (triangleMeshes != null)
        {
            Profiler.BeginSample("Setting Mesh");

            int validCount = options.GroupsToUpdate.Count;

            //Debug.LogFormat("Valid    meshes = {0}", validCount);
            //Debug.LogFormat("Existing meshes = {0}", _meshes.Count);
            while (_meshes.Count > validCount)
            {
                _meshes.Last.Value.gameObject.Destroy();

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

    public void Dispose()
    {
 
    }
}
