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

        public void UpdateMesh(IReadOnlyDictionary<TKey, ITerrainMesh> terrainMeshes, IReadOnlySet<TKey> toKeepGroups)
        {
            if (terrainMeshes != null)
            {
                using (_profiler.Start("Setting Mesh"))
                {
                    var toDeleteGroups = _meshFilters.Keys.Except(terrainMeshes.Keys).Except(toKeepGroups).ToList();

                    int meshIndex = 0;
                    foreach (var terrainMesh in terrainMeshes)
                    {
                        if (terrainMesh.Value.TriangleCount == 0)
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
                        unityData.Transform.localPosition = terrainMesh.Value.Offset.ToVector3();

                        ITerrainMesh triangleMesh = terrainMesh.Value;
                        UpdateMesh(triangleMesh, unityData.MeshFilter.sharedMesh);

                        unityData.MeshCollider.sharedMesh = null;
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

        private void UpdateMesh(ITerrainMesh triangleMesh, Mesh mesh)
        {
            SetMeshFormat(mesh, triangleMesh.VertexCount);

            if (triangleMesh is IPooledTerrainMesh)
            {
                UpdateMesh((triangleMesh as IPooledTerrainMesh).Mesh, mesh);
            }
            else if (triangleMesh is FixedTerrainMesh2i)
            {
                UpdateMesh(triangleMesh as FixedTerrainMesh2i, mesh);
            }
            else if (triangleMesh is ExpandingTerrainMesh)
            {
                UpdateMesh(triangleMesh as ExpandingTerrainMesh, mesh);
            }
            else if (triangleMesh != null)
            {
                Debug.LogError($"Unsuported ITriangleMesh implementation '{triangleMesh.GetType().Name}'");
            }
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

        private void UpdateMesh(ExpandingTerrainMesh triangleMesh, Mesh mesh)
        {
            bool recomputeTriangles = mesh.vertexCount != triangleMesh.VertexCount;
            if (recomputeTriangles)
            {
                mesh.Clear();
            }

            mesh.SetVertices(triangleMesh.Vertices.ToVector3List());
            mesh.SetNormalsOrRecompute(triangleMesh.Normals.ToVector3List());
            mesh.SetUVs(0, triangleMesh.UV.ToVector2List());
            if (recomputeTriangles)
            {
                mesh.SetTriangles(triangleMesh.Indices, 0, false);
            }
            mesh.bounds = triangleMesh.MeshBounds.ToBounds();
        }

        private void UpdateMesh(FixedTerrainMesh2i triangleMesh, Mesh mesh)
        {
            bool recomputeTriangles = mesh.vertexCount != triangleMesh.VertexCount;
            if (recomputeTriangles)
            {
                mesh.Clear();
            }

            mesh.vertices = triangleMesh.Vertices.ToVector3();
            mesh.SetNormalsOrRecompute(triangleMesh.Normals.ToVector3());
            mesh.uv = triangleMesh.UV.ToVector2();
            if (recomputeTriangles)
            {
                mesh.SetTriangles(triangleMesh.Indices, 0, false);
            }
            mesh.bounds = triangleMesh.MeshBounds.ToBounds();
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
                Transform = gameObject.transform;
            }

            public MeshFilter MeshFilter { get; }
            public MeshCollider MeshCollider { get; }
            public GameObject GameObject { get; }
            public Transform Transform { get; }

            public void Dispose()
            {
                GameObject.Destroy();
            }
        }
    }
}