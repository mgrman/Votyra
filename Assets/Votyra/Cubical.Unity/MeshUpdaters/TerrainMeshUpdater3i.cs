using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Votyra.Core.Models;
using Votyra.Core.Pooling;
using Votyra.Core.TerrainMeshes;
using Votyra.Core.Utils;

namespace Votyra.Cubical.MeshUpdaters
{
    public class TerrainMeshUpdater3i : IMeshUpdater3i
    {
        private SetDictionary<Vector3i, MeshFilter> _meshFilters = new SetDictionary<Vector3i, MeshFilter>();

        public IReadOnlySet<Vector3i> ExistingGroups => _meshFilters;

        public TerrainMeshUpdater3i() { }

        public void UpdateMesh(IMeshContext options, IReadOnlyDictionary<Vector3i, ITerrainMesh> terrainMeshes, IEnumerable<Vector3i> toKeepGroups)
        {
            if (terrainMeshes != null)
            {
                using(options.ProfilerFactory("Setting Mesh", this))
                {
                    var toDeleteGroups = _meshFilters.Keys.Except(terrainMeshes.Keys).Except(toKeepGroups).ToList();

                    int meshIndex = 0;
                    foreach (var terrainMesh in terrainMeshes)
                    {
                        if (terrainMesh.Value.TriangleCount == 0)
                        {
                            continue;
                        }

                        MeshFilter meshFilter;
                        if (!_meshFilters.TryGetValue(terrainMesh.Key, out meshFilter))
                        {
                            if (toDeleteGroups.Count > 0)
                            {
                                int toDeleteIndex = toDeleteGroups.Count - 1;
                                var toDeleteKey = toDeleteGroups[toDeleteIndex];
                                meshFilter = _meshFilters[toDeleteKey];
                                _meshFilters.Remove(toDeleteKey);
                                _meshFilters[terrainMesh.Key] = meshFilter;

                                toDeleteGroups.RemoveAt(toDeleteIndex);
                            }
                            else
                            {
                                meshFilter = CreateMeshObject(options);
                                _meshFilters[terrainMesh.Key] = meshFilter;
                            }
                        }

                        ITerrainMesh triangleMesh = terrainMesh.Value;
                        UpdateMesh(triangleMesh, meshFilter.sharedMesh);

                        var collider = meshFilter.gameObject.GetComponent<MeshCollider>();
                        collider.sharedMesh = null;
                        collider.sharedMesh = meshFilter.sharedMesh;

                        meshIndex++;
                    }

                    foreach (var toDeleteGroup in toDeleteGroups)
                    {
                        _meshFilters[toDeleteGroup].gameObject.Destroy();
                        _meshFilters.Remove(toDeleteGroup);
                    }
                }
            }
        }

        private void UpdateMesh(ITerrainMesh triangleMesh, Mesh mesh)
        {
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

        private void UpdateMesh(ExpandingTerrainMesh triangleMesh, Mesh mesh)
        {
            bool recomputeTriangles = mesh.vertexCount != triangleMesh.PointCount;
            if (recomputeTriangles)
            {
                mesh.Clear();
            }

            mesh.vertices = triangleMesh.Vertices.ToVector3Array();
            mesh.SetNormalsOrRecompute(triangleMesh.Normals.ToVector3Array());
            mesh.uv = triangleMesh.UV.ToVector2Array();
            if (recomputeTriangles)
            {
                mesh.SetTriangles(triangleMesh.Indices, 0, false);
            }
            mesh.bounds = triangleMesh.MeshBounds.ToBounds();

            // mesh.SetVertices(triangleMesh.Vertices.ToVector3List());
            // mesh.SetNormalsOrRecompute(triangleMesh.Normals.ToVector3List());
            // mesh.SetUVs(0, triangleMesh.UV.ToVector2List());
            // if (recomputeTriangles)
            // {
            //     mesh.SetTriangles(triangleMesh.Indices, 0, false);
            // }
            // mesh.bounds = triangleMesh.MeshBounds.ToBounds();
        }

        private void UpdateMesh(FixedTerrainMesh2i triangleMesh, Mesh mesh)
        {
            bool recomputeTriangles = mesh.vertexCount != triangleMesh.PointCount;
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

        private MeshFilter CreateMeshObject(IMeshContext options)
        {
            string name = string.Format("group_{0}", _meshFilters.Count);
            var tile = options.GameObjectFactory != null ? options.GameObjectFactory() : new GameObject();
            tile.name = name;
            tile.hideFlags = HideFlags.DontSave;

            var meshFilter = tile.GetOrAddComponent<MeshFilter>();
            tile.AddComponentIfMissing<MeshRenderer>();
            tile.AddComponentIfMissing<MeshCollider>();

            if (meshFilter.sharedMesh == null)
            {
                meshFilter.mesh = new Mesh();
            }
            var mesh = meshFilter.sharedMesh;
            mesh.MarkDynamic();

            return meshFilter;
        }
    }
}