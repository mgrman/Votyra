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
    public static class TerrainMeshConverter
    {
        public static UnityMesh GetUnityMesh(this ITerrainMesh votyraMesh, UnityMesh existingUnityMesh)
        {
            var unityMesh = GetUnityMeshInner(votyraMesh, existingUnityMesh, () =>
            {
                if (votyraMesh is IPooledTerrainMesh pooledTerrainMesh)
                {
                    pooledTerrainMesh.Dispose();
                }
            });
            return unityMesh;
        }

        private static UnityMesh GetUnityMeshInner(ITerrainMesh votyraMesh, UnityMesh existingUnityMesh, Action onDispose)
        {
            if (votyraMesh is IPooledTerrainMesh)
            {
                return GetUnityMeshInner((votyraMesh as IPooledTerrainMesh).Mesh, existingUnityMesh, onDispose);
            }
            else if (votyraMesh is FixedTerrainMesh2i)
            {
                return GetUnityMeshInner(votyraMesh as FixedTerrainMesh2i, existingUnityMesh, onDispose);
            }
            else if (votyraMesh is ExpandingTerrainMesh)
            {
                return GetUnityMeshInner(votyraMesh as ExpandingTerrainMesh, existingUnityMesh, onDispose);
            }
            else if (votyraMesh is ExpandingUnityTerrainMesh)
            {
                return GetUnityMeshInner(votyraMesh as ExpandingUnityTerrainMesh, existingUnityMesh, onDispose);
            }
            else if (votyraMesh != null)
            {
                Debug.LogError($"Unsuported ITriangleMesh implementation '{votyraMesh.GetType().Name}'");
                return null;
            }
            else
            {
                return null;
            }
        }

        private static UnityMesh GetUnityMeshInner(ExpandingTerrainMesh triangleMesh, UnityMesh existingUnityMesh,Action onDispose)
        {
            bool recomputeTriangles = existingUnityMesh?.VertexCount != triangleMesh.VertexCount;
            if (recomputeTriangles)
            {
                existingUnityMesh = null;
            }

            var vertices = triangleMesh.Vertices.ToVector3Array();
            var normals = triangleMesh.Normals.ToVector3Array();
            var uvs = existingUnityMesh?.UV ?? triangleMesh.UV.ToVector2Array();
            var indices = existingUnityMesh?.Indices ?? triangleMesh.Indices.ToArray();
            var bounds = existingUnityMesh?.MeshBounds ?? triangleMesh.MeshBounds.ToBounds();
            
            return new UnityMesh(bounds, vertices, normals, uvs, indices,  onDispose);
        }

        private static UnityMesh GetUnityMeshInner(FixedTerrainMesh2i triangleMesh, UnityMesh existingUnityMesh, Action onDispose)
        {
            bool recomputeTriangles = existingUnityMesh?.VertexCount != triangleMesh.VertexCount;
            if (recomputeTriangles)
            {
                existingUnityMesh = null;
            }

            var vertices = triangleMesh.Vertices.ToVector3();
            var normals = triangleMesh.Normals.ToVector3();
            var uvs = existingUnityMesh?.UV ?? triangleMesh.UV.ToVector2();
            var indices = existingUnityMesh?.Indices ?? triangleMesh.Indices.ToArray();
            var bounds = existingUnityMesh?.MeshBounds ?? triangleMesh.MeshBounds.ToBounds();
            return new UnityMesh(bounds, vertices, normals, uvs, indices,  onDispose);
        }

        private static UnityMesh GetUnityMeshInner(ExpandingUnityTerrainMesh triangleMesh, UnityMesh existingUnityMesh,
            Action onDispose)
        {
            bool recomputeTriangles = existingUnityMesh?.VertexCount != triangleMesh.VertexCount;
            if (recomputeTriangles)
            {
                existingUnityMesh = null;
            }

            var vertices = triangleMesh.Vertices;
            var normals = triangleMesh.Normals;
            var uvs = existingUnityMesh?.UV ?? triangleMesh.UV;
            var indices = existingUnityMesh?.Indices ?? triangleMesh.Indices;
            var bounds = existingUnityMesh?.MeshBounds ?? triangleMesh.MeshBounds;
            return new UnityMesh(bounds, vertices, normals, uvs, indices, onDispose);
        }
    }
}