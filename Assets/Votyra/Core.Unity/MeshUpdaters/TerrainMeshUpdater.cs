using UnityEngine;
using UnityEngine.Rendering;
using Votyra.Core.TerrainMeshes;
using Votyra.Core.Utils;

namespace Votyra.Core.MeshUpdaters
{
    public class TerrainMeshUpdater : ITerrainMeshUpdater
    {
        public void SetUnityMesh(ITerrainMesh triangleMesh, GameObject unityData)
        {
            var meshFilter = unityData.GetComponent<MeshFilter>();
            SetUnityMesh(triangleMesh, meshFilter.sharedMesh);

            var meshCollider = unityData.GetComponent<MeshCollider>();
            meshCollider.sharedMesh = meshFilter.sharedMesh;
        }

        public void DestroyMesh(GameObject unityData)
        {
            var meshFilter = unityData.GetComponent<MeshFilter>();
            var mesh = meshFilter.sharedMesh;
            mesh.Destroy();
        }

        private void SetUnityMesh(ITerrainMesh triangleMesh, Mesh mesh)
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

        private static void SetMeshFormat(Mesh mesh, int vertexCount)
        {
            if (vertexCount > 65000 && mesh.indexFormat != IndexFormat.UInt32)
                mesh.indexFormat = IndexFormat.UInt32;
            else if (vertexCount < 65000 && mesh.indexFormat != IndexFormat.UInt16)
                mesh.indexFormat = IndexFormat.UInt16;
        }
    }
}