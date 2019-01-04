using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using Votyra.Core.Models;
using Votyra.Core.Pooling;
using Votyra.Core.Profiling;
using Votyra.Core.TerrainMeshes;
using Votyra.Core.Utils;

namespace Votyra.Core.MeshUpdaters
{
    public class TerrainMeshUpdater<TKey> : IMeshUpdater<TKey>
    {
        private readonly IProfiler _profiler;
        private readonly Func<GameObject> _gameObjectFactory;
        private SetDictionary<TKey, MeshUnityData> _meshFilters = new SetDictionary<TKey, MeshUnityData>();

        public TerrainMeshUpdater(Func<GameObject> gameObjectFactory, IProfiler profiler)
        {
            _gameObjectFactory = gameObjectFactory;
            _profiler = profiler;
        }

        public IReadOnlySet<TKey> ExistingGroups => _meshFilters;

        public void UpdateMesh(IReadOnlyDictionary<TKey, UnityMesh> terrainMeshes, IReadOnlySet<TKey> toKeepGroups)
        {
            if (terrainMeshes != null)
            {
                using (_profiler.Start("Setting Mesh"))
                {
                    var toDeleteGroups = _meshFilters.Keys.Except(terrainMeshes.Keys).Except(toKeepGroups).ToList();

                    int meshIndex = 0;
                    foreach (var terrainMesh in terrainMeshes)
                    {
                        if (terrainMesh.Value == null || terrainMesh.Value.VertexCount == 0)
                        {
                            if (_meshFilters.ContainsKey(terrainMesh.Key))
                            {
                                _meshFilters[terrainMesh.Key]?.Dispose();
                            }
                            _meshFilters[terrainMesh.Key] = null;
                            continue;
                        }
                        var unityData = _meshFilters.TryGetValue(terrainMesh.Key);
                        if (unityData == null)
                        {
                            if (toDeleteGroups.Count > 0)
                            {
                                int toDeleteIndex = toDeleteGroups.Count - 1;
                                var toDeleteKey = toDeleteGroups[toDeleteIndex];
                                unityData = _meshFilters[toDeleteKey];
                                _meshFilters.Remove(toDeleteKey);
                                _meshFilters[terrainMesh.Key] = unityData;

                                toDeleteGroups.RemoveAt(toDeleteIndex);
                            }
                            if (unityData == null)
                            {
                                unityData = CreateMeshObject();
                                _meshFilters[terrainMesh.Key] = unityData;
                            }
                        }

                        var triangleMesh = terrainMesh.Value;
                        UpdateMesh(triangleMesh, unityData.MeshFilter.sharedMesh);

                        unityData.MeshCollider.sharedMesh = unityData.MeshFilter.sharedMesh;

                        meshIndex++;
                    }

                    foreach (var toDeleteGroup in toDeleteGroups)
                    {
                        _meshFilters[toDeleteGroup]?.Dispose();
                        _meshFilters.Remove(toDeleteGroup);
                    }
                }
            }
        }

        private void UpdateMesh(UnityMesh triangleMesh, Mesh mesh)
        {
            SetMeshFormat(mesh, triangleMesh.VertexCount);

            bool reinitializeMesh = mesh.vertexCount != triangleMesh.VertexCount;
            if (reinitializeMesh)
            {
                mesh.Clear();
            }

            switch (triangleMesh.Vertices)
            {
                case Vector3[] array:
                    mesh.vertices = array;
                    break;
                case List<Vector3> list:
                    mesh.SetVertices(list);
                    break;
                default:
                    mesh.vertices = triangleMesh.Vertices.ToArray();
                    break;
            }

            switch (triangleMesh.Normals)
            {
                case Vector3[] array:
                    mesh.normals =array;
                    break;
                case List<Vector3> list:
                    mesh.SetNormals(list);
                    break;
                default:
                    mesh.normals = triangleMesh.Normals.ToArray();
                    break;
            }

            switch (triangleMesh.UV)
            {
                case Vector2[] array:
                    mesh.uv = array;
                    break;
                case List<Vector2> list:
                    mesh.SetUVs(0, list);
                    break;
                default:
                    mesh.uv = triangleMesh.UV.ToArray();
                    break;
            }

            switch (triangleMesh.Indices)
            {
                case int[] array:
                    mesh.SetTriangles(array, 0, false);
                    break;
                case List<int> list:
                    mesh.SetTriangles(list, 0, false);
                    break;
                default:
                    mesh.SetTriangles(triangleMesh.Indices.ToArray(), 0, false);
                    break;
            }

            mesh.bounds = triangleMesh.MeshBounds;
            
        }

        private void SetMeshFormat(Mesh mesh, int vertexCount)
        {
            if (vertexCount > 65000 && mesh.indexFormat != IndexFormat.UInt32)
            {
                mesh.indexFormat = IndexFormat.UInt32;
            }
            else if (vertexCount < 65000 && mesh.indexFormat != IndexFormat.UInt16)
            {
                mesh.indexFormat = IndexFormat.UInt16;
            }
        }

        private MeshUnityData CreateMeshObject()
        {
            string name = string.Format("group_{0}", _meshFilters.Count);
            var gameObject = _gameObjectFactory();
            gameObject.name = name;
            gameObject.hideFlags = HideFlags.DontSave;

            var meshFilter = gameObject.GetOrAddComponent<MeshFilter>();
            gameObject.AddComponentIfMissing<MeshRenderer>();
            gameObject.AddComponentIfMissing<MeshCollider>();

            if (meshFilter.sharedMesh == null)
            {
                meshFilter.mesh = new Mesh();
            }
            var mesh = meshFilter.sharedMesh;
            mesh.MarkDynamic();

            return new MeshUnityData(gameObject);
        }

        private class MeshUnityData : IDisposable
        {
            public MeshUnityData(GameObject gameObject)
            {
                MeshFilter = gameObject.GetComponent<MeshFilter>();
                MeshCollider = gameObject.GetComponent<MeshCollider>();
                GameObject = gameObject;
            }

            public MeshFilter MeshFilter { get; }
            public MeshCollider MeshCollider { get; }
            public GameObject GameObject { get; }

            public void Dispose()
            {
                GameObject.Destroy();
            }
        }
    }
}