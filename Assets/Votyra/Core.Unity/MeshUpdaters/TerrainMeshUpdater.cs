using UnityEngine;
using UnityEngine.Rendering;
using Votyra.Core.TerrainMeshes;

namespace Votyra.Core.MeshUpdaters
{
    public static class TerrainMeshUpdater
    {
        public static void SetUnityMesh(this ITerrainMesh triangleMesh, GameObject unityData)
        {
            var meshFilter = unityData.GetComponent<MeshFilter>();
            SetUnityMesh(triangleMesh, meshFilter.sharedMesh);

            var meshCollider = unityData.GetComponent<MeshCollider>();
            if (meshCollider != null)
            {
                meshCollider.sharedMesh = meshFilter.sharedMesh;
            }

            var boxCollider = unityData.GetComponent<BoxCollider>();
            if (boxCollider != null)
            {
                SetBoxCollider(triangleMesh, boxCollider);
            }
        }

        private static void SetUnityMesh(ITerrainMesh triangleMesh, Mesh mesh)
        {
            SetMeshFormat(mesh, triangleMesh.VertexCount);

            var reinitializeMesh = mesh.vertexCount != triangleMesh.VertexCount;
            if (reinitializeMesh)
                mesh.Clear();

            if (triangleMesh is FixedUnityTerrainMesh2i fixedMesh)
            {
                mesh.vertices = fixedMesh.Vertices;
                mesh.normals = fixedMesh.Normals;
                if (reinitializeMesh)
                {
                    mesh.uv = fixedMesh.UV;
                    mesh.SetTriangles(fixedMesh.Indices, 0);
                }

                mesh.bounds = fixedMesh.MeshBounds;
            }
            else if (triangleMesh is ExpandingUnityTerrainMesh expandingMesh)
            {
                mesh.SetVertices(expandingMesh.Vertices);
                mesh.SetNormals(expandingMesh.Normals);
                if (reinitializeMesh)
                {
                    mesh.SetUVs(0, expandingMesh.UV);
                    mesh.SetTriangles(expandingMesh.Indices, 0);
                }

                mesh.bounds = expandingMesh.MeshBounds;
            }
        }

        private static void SetBoxCollider(ITerrainMesh triangleMesh, BoxCollider collider)
        {
            if (triangleMesh is FixedUnityTerrainMesh2i fixedMesh)
            {
                collider.center = fixedMesh.MeshBounds.center;
                collider.size = fixedMesh.MeshBounds.size;

            }
            else if (triangleMesh is ExpandingUnityTerrainMesh expandingMesh)
            {
                collider.center = expandingMesh.MeshBounds.center;
                collider.size = expandingMesh.MeshBounds.size;
            }
        }

        private static void SetMeshFormat(Mesh mesh, int vertexCount)
        {
            if (vertexCount > 65000 && mesh.indexFormat != IndexFormat.UInt32)
                mesh.indexFormat = IndexFormat.UInt32;
            else if (vertexCount < 65000 && mesh.indexFormat != IndexFormat.UInt16)
                mesh.indexFormat = IndexFormat.UInt16;
        }
    }
}