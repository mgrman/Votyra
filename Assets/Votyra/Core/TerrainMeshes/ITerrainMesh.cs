using System;
using System.Collections.Generic;
using Votyra.Core.Models;

namespace Votyra.Core.TerrainMeshes
{
    public interface ITerrainMesh
    {
        int VertexCount { get; }

        int TriangleCount { get; }

        IReadOnlyList<Vector3f> Vertices { get; }
        IReadOnlyList<Vector3f> Normals { get; }
        IReadOnlyList<Vector2f> UV { get; }
        IReadOnlyList<int> Indices { get; }
        Area3f MeshBounds { get; }

        void Initialize(Func<Vector3f, Vector3f> vertexPostProcessor, Func<Vector2f, Vector2f> uvAdjustor);

        void Reset(Area3f area);

        void AddTriangle(Vector3f a, Vector3f b, Vector3f c);

        void FinalizeMesh();

        IEnumerable<Triangle3f> GetTriangles(Vector2i? limitToCellInGroup);
    }
}