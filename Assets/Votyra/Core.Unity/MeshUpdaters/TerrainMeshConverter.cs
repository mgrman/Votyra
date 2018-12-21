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
    public class TerrainMeshConverter : ITerrainMeshConverter
    {
        public UnityMesh GetUnityMesh(ITerrainMesh votyraMesh, UnityMesh existingUnityMesh)
        {
            if (votyraMesh is IPooledTerrainMesh)
            {
                return GetUnityMesh((votyraMesh as IPooledTerrainMesh).Mesh, existingUnityMesh);
            }
            else if (votyraMesh is FixedTerrainMesh2i)
            {
                return GetUnityMesh(votyraMesh as FixedTerrainMesh2i, existingUnityMesh);
            }
            else if (votyraMesh is ExpandingTerrainMesh)
            {
                return GetUnityMesh(votyraMesh as ExpandingTerrainMesh, existingUnityMesh);
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

        private UnityMesh GetUnityMesh(ExpandingTerrainMesh triangleMesh, UnityMesh existingUnityMesh)
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
            return new UnityMesh(bounds, vertices, normals, uvs, indices);
        }

        private UnityMesh GetUnityMesh(FixedTerrainMesh2i triangleMesh, UnityMesh existingUnityMesh)
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
            return new UnityMesh(bounds, vertices, normals, uvs, indices);
        }
    }
}