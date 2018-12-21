using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Votyra.Core.TerrainMeshes
{
    public class UnityMesh
    {
        public UnityMesh(Bounds meshBounds, Vector3[] vertices, Vector3[] normals, Vector2[] uv, int[] indices)
        {
            if (vertices.Length != normals.Length && vertices.Length != uv.Length && vertices.Length != indices.Length)
            {
                throw new ArgumentException();
            }
            MeshBounds = meshBounds;
            Vertices = vertices;
            Normals = normals;
            UV = uv;
            Indices = indices;
        }

        public Bounds MeshBounds { get; }
        public Vector3[] Vertices { get; }
        public Vector3[] Normals { get; }
        public Vector2[] UV { get; }
        public int[] Indices { get; }
        public int VertexCount => Vertices.Length;
    }
}