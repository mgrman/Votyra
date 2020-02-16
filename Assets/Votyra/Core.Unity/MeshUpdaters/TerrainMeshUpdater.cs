using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using Votyra.Core.Logging;
using Votyra.Core.Models;
using Votyra.Core.TerrainMeshes;
using Votyra.Core.Unity;
using Votyra.Core.Utils;

namespace Votyra.Core.MeshUpdaters
{
    public static class TerrainMeshUpdater
    {
        public static void SetUnityMesh(this IMesh triangleMesh, ITerrainGameObject unityData)
        {
            var meshFilter = unityData.MeshFilter;
            SetUnityMesh(triangleMesh, meshFilter.sharedMesh);

            var meshCollider = unityData.MeshCollider;
            if (meshCollider != null)
                meshCollider.sharedMesh = meshFilter.sharedMesh;

            var boxCollider = unityData.BoxCollider;
            if (boxCollider != null)
                SetBoxCollider(triangleMesh, boxCollider);
        }

        private static void SetUnityMesh(IMesh triangleMesh, Mesh mesh)
        {
            SetMeshFormat(mesh, triangleMesh.VertexCount);

            var reinitializeMesh = mesh.vertexCount != triangleMesh.VertexCount;
            if (reinitializeMesh)
                mesh.Clear();

            SetVertices(triangleMesh, mesh);
            SetNormals(triangleMesh, mesh);
            SetUVs(triangleMesh, mesh);

            if (reinitializeMesh)
            {
                SetTriangles(triangleMesh, mesh);
            }

            mesh.bounds = triangleMesh.MeshBounds.ToBounds();
        }

        private static void SetTriangles(IMesh triangleMesh, Mesh mesh)
        {
            if (triangleMesh.Indices is int[] indexArray)
            {
                mesh.SetTriangles(indexArray, 0);
            }
            else if (triangleMesh.Indices is List<int> indexList)
            {
                mesh.SetTriangles(indexList, 0);
            }
            else
            {
                mesh.SetTriangles(triangleMesh.Indices.ToList(), 0);
                StaticLogger.LogWarning("Using unsuported IReadOnlyList type for Indices, slow conversion is used");
            }
        }

        private static void SetUVs(IMesh triangleMesh, Mesh mesh)
        {
            if (triangleMesh.UV is Vector2f[] uvArray)
            {
                mesh.uv = uvArray.ToVector2();
            }
            else if (triangleMesh.UV is List<Vector2f> uvList)
            {
                mesh.SetUVs(0, uvList.ToVector2List());
            }
            else
            {
                mesh.SetUVs(0,
                    triangleMesh.UV.Select(o => o.ToVector2())
                        .ToList());
                StaticLogger.LogWarning("Using unsuported IReadOnlyList type for UV, slow conversion is used");
            }
        }

        private static void SetVertices(IMesh triangleMesh, Mesh mesh)
        {
            if (triangleMesh.Vertices is Vector3f[] verticesArray)
            {
                mesh.vertices = verticesArray.ToVector3();
            }
            else if (triangleMesh.Vertices is List<Vector3f> verticesList)
            {
                mesh.SetVertices(verticesList.ToVector3List());
            }
            else
            {
                mesh.SetVertices(triangleMesh.Vertices.Select(o => o.ToVector3())
                    .ToList());
                StaticLogger.LogWarning("Using unsuported IReadOnlyList type for Vertices, slow conversion is used");
            }
        }

        private static void SetNormals(IMesh triangleMesh, Mesh mesh)
        {
            if (triangleMesh.Normals is Vector3f[] normalsArray)
            {
                mesh.normals = normalsArray.ToVector3();
            }
            else if (triangleMesh.Normals is List<Vector3f> normalsList)
            {
                mesh.SetNormals(normalsList.ToVector3List());
            }
            else
            {
                mesh.SetNormals(triangleMesh.Normals.Select(o => o.ToVector3())
                    .ToList());
                StaticLogger.LogWarning("Using unsuported IReadOnlyList type for Normals, slow conversion is used");
            }
        }

        private static void SetBoxCollider(IMesh triangleMesh, BoxCollider collider)
        {
            collider.center = triangleMesh.MeshBounds.Center.ToVector3();
            collider.size = triangleMesh.MeshBounds.Size.ToVector3();
        }

        private static void SetMeshFormat(Mesh mesh, uint vertexCount)
        {
            if (vertexCount > 65000 && mesh.indexFormat != IndexFormat.UInt32)
                mesh.indexFormat = IndexFormat.UInt32;
            else if (vertexCount < 65000 && mesh.indexFormat != IndexFormat.UInt16)
                mesh.indexFormat = IndexFormat.UInt16;
        }
    }
}