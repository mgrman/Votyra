using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Votyra.Core.TerrainMeshes
{
    public class UnityMesh:IDisposable
    {
        private readonly Action _onDisposed;

        public UnityMesh(Bounds meshBounds, IReadOnlyList<Vector3> vertices, IReadOnlyList<Vector3> normals,
            IReadOnlyList<Vector2> uv, IReadOnlyList<int> indices,Action onDisposed)
        {
            if (vertices.Count != normals.Count && vertices.Count != uv.Count && vertices.Count != indices.Count)
            {
                throw new ArgumentException();
            }

            _onDisposed = onDisposed;
            MeshBounds = meshBounds;
            Vertices = vertices;
            Normals = normals;
            UV = uv;
            Indices = indices;
        }

        public Bounds MeshBounds { get; }
        public IReadOnlyList<Vector3>Vertices { get; }
        public IReadOnlyList<Vector3> Normals { get; }
        public IReadOnlyList<Vector2> UV { get; }
        public IReadOnlyList<int> Indices { get; }
        public int VertexCount => Vertices.Count;
        public void Dispose()
        {
            _onDisposed();
        }
    }
}