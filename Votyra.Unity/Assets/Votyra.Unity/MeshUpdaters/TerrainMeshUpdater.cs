using System.Collections.Generic;
using Votyra.Common.Profiling;
using Votyra.Pooling;
using Votyra.Unity.Utils;
using UnityEngine;

namespace Votyra.Unity.MeshUpdaters
{
    public class TerrainMeshUpdater : IMeshUpdater
    {
        private List<MeshFilter> _meshFilters = new List<MeshFilter>();

        public TerrainMeshUpdater()
        {
        }

        public void UpdateMesh(MeshOptions options, IList<TerrainMeshers.TriangleMesh.ITriangleMesh> terrainMeshes)
        {
#if UNITY_EDITOR

            if (!Application.isPlaying)
            {
                _meshFilters.Clear();
                options.ParentContainer.DestroyAllChildren();
            }
#endif
            if (options.ParentContainer.transform.childCount != _meshFilters.Count)
            {
                options.ParentContainer.DestroyAllChildren();
                _meshFilters.Clear();
            }

            if (terrainMeshes != null)
            {
                using (ProfilerFactory.Create("Setting Mesh"))
                {
                    int meshIndex = 0;
                    foreach (var terrainMesh in terrainMeshes)
                    {
                        if (meshIndex >= _meshFilters.Count)
                        {
                            CreateMeshObject(options);
                        }

                        var meshFilter = _meshFilters[meshIndex];
                        TerrainMeshers.TriangleMesh.ITriangleMesh triangleMesh = terrainMesh;
                        if (triangleMesh is TerrainMeshers.TriangleMesh.FixedTriangleMesh)
                        {
                            UpdateMesh(triangleMesh, meshFilter.sharedMesh);
                        }
                        else if (triangleMesh != null)
                        {
                            UpdateMesh(triangleMesh, meshFilter.sharedMesh);
                        }
                        var collider = meshFilter.gameObject.GetComponent<MeshCollider>();
                        collider.sharedMesh = null;
                        collider.sharedMesh = meshFilter.sharedMesh;

                        meshIndex++;
                    }

                    Pool.Meshes2.ReturnObject(terrainMeshes, new Pool.MeshKey(terrainMeshes.Count, terrainMeshes.Count == 0 ? 0 : terrainMeshes[0].TriangleCount));

                    int terrainMeshCount = meshIndex;
                    for (int toDeleteIndex = _meshFilters.Count - 1; toDeleteIndex >= terrainMeshCount; toDeleteIndex--)
                    {
                        _meshFilters[toDeleteIndex].gameObject.Destroy();
                        _meshFilters.RemoveAt(toDeleteIndex);
                    }
                }
            }

            options.Dispose();
        }

        private void UpdateMesh(TerrainMeshers.TriangleMesh.ITriangleMesh triangleMesh, Mesh mesh)
        {
            if (mesh.vertexCount != triangleMesh.PointCount)
            {
                mesh.Clear();
                mesh.vertices = triangleMesh.Vertices;
                //mesh.normals = this._normals;
                mesh.uv = triangleMesh.UV;
                mesh.triangles = triangleMesh.Indices;
            }
            else
            {
                mesh.vertices = triangleMesh.Vertices;
                //mesh.normals = this._normals;
                mesh.uv = triangleMesh.UV;
            }
            //Profiler.BeginSample("RecalculateNormals");
            mesh.RecalculateNormals();
            //Profiler.EndSample();

            mesh.bounds = triangleMesh.MeshBounds;

            
            
        }

        //private void UpdateMesh(TerrainMeshers.TriangleMesh.FixedTriangleMesh triangleMesh, Mesh mesh)
        //{
        //    if (mesh.vertexCount != triangleMesh.PointCount)
        //    {
        //        mesh.Clear();
        //        mesh.vertices = triangleMesh.Vertices;
        //        //mesh.normals = this._normals;
        //        mesh.uv = triangleMesh.UV;
        //        mesh.triangles = triangleMesh.Indices;

        //    }
        //    else
        //    {
        //        mesh.vertices = triangleMesh.Vertices;
        //        //mesh.normals = this._normals;
        //        mesh.uv = triangleMesh.UV;
        //    }
        //    //Profiler.BeginSample("RecalculateNormals");
        //    mesh.RecalculateNormals();
        //    //Profiler.EndSample();

        //    mesh.bounds = this.MeshBounds;
        //}

        private void CreateMeshObject(MeshOptions options)
        {
            //TODO: use prefab so custom scripts can be attached! Like OnMouseDown, ...

            string name = string.Format("group_{0}", _meshFilters.Count);
            var tile = options.GameObjectFactory != null ? options.GameObjectFactory() : new GameObject();
            tile.name = name;
            tile.hideFlags = HideFlags.DontSave;
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
            var meshCollider = tile.GetComponent<MeshCollider>();
            if (meshCollider == null)
            {
                meshCollider = tile.AddComponent<MeshCollider>();
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
}