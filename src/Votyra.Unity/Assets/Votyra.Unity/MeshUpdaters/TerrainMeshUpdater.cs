using System.Collections.Generic;
using Votyra.Common.Profiling;
using Votyra.Pooling;
using Votyra.Unity.Utils;
using UnityEngine;
using Votyra.Common.Models;
using System.Linq;
using Votyra.TerrainMeshers.TriangleMesh;

namespace Votyra.Unity.MeshUpdaters
{
    public class TerrainMeshUpdater : IMeshUpdater
    {
        private Dictionary<Vector2i, MeshFilter> _meshFilters = new Dictionary<Vector2i, MeshFilter>();

        public TerrainMeshUpdater()
        {
        }

        public void UpdateMesh(MeshOptions options, IReadOnlyDictionary<Vector2i, ITriangleMesh> terrainMeshes)
        {
#if UNITY_EDITOR

            if (!Application.isPlaying)
            {
                _meshFilters.Clear();
                options.ParentContainer.DestroyAllChildren();
            }
#endif
            // if (options.ParentContainer.transform.childCount != _meshFilters.Count)
            // {
            //     options.ParentContainer.DestroyAllChildren();
            //     _meshFilters.Clear();
            // }

            if (terrainMeshes != null)
            {
                using (ProfilerFactory.Create("Setting Mesh"))
                {
                    int meshIndex = 0;
                    foreach (var terrainMesh in terrainMeshes)
                    {


                        MeshFilter meshFilter;
                        if (!_meshFilters.TryGetValue(terrainMesh.Key, out meshFilter))
                        {
                            meshFilter = CreateMeshObject(terrainMesh.Key, options);
                            _meshFilters[terrainMesh.Key] = meshFilter;
                        }



                        TerrainMeshers.TriangleMesh.ITriangleMesh triangleMesh = terrainMesh.Value;
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


                    var toDeleteGroups = _meshFilters.Keys.Except(terrainMeshes.Keys).ToArray();
                    foreach (var toDeleteGroup in toDeleteGroups)
                    {
                        _meshFilters[toDeleteGroup].gameObject.Destroy();
                        _meshFilters.Remove(toDeleteGroup);
                    }

                    // var key = terrainMeshes.Count == 0 ? 0 : (terrainMeshes.First().Value.TriangleCount);
                    foreach (var mesh in terrainMeshes.Values.Where(o => o != null))
                    {
                        Pool.Meshes.ReturnObject(mesh, mesh.TriangleCount);
                    }
                    (terrainMeshes as IDictionary<Vector2i, ITriangleMesh>).Clear();
                    // Pool.Meshes3.ReturnObject(terrainMeshes, new Pool.MeshKey2(key));

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

        private MeshFilter CreateMeshObject(Vector2i group, MeshOptions options)
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

            return meshFilter;
            // _meshFilters[group] = meshFilter;
        }
    }
}