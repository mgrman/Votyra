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
    public class TerrainMeshUpdater : IMeshUpdater
    {
        private readonly IProfiler _profiler;
        private readonly Func<GameObject> _gameObjectFactory;

        public TerrainMeshUpdater(Func<GameObject> gameObjectFactory, IProfiler profiler)
        {
            _gameObjectFactory = gameObjectFactory;
            _profiler = profiler;
        }

        public GameObject UpdateMesh(UnityMesh triangleMesh,GameObject unityData)
        {
            if (unityData == null)
            {

                    unityData = CreateMeshObject();
            }

            UpdateMesh(triangleMesh, unityData.GetComponent<MeshFilter>().sharedMesh);

            unityData.GetComponent<MeshCollider>().sharedMesh = unityData.GetComponent<MeshFilter>().sharedMesh;

            return unityData;
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

        private GameObject CreateMeshObject()
        {
            string name = string.Format("group_{0}", Guid.NewGuid());
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

            return gameObject;
        }
    }
}