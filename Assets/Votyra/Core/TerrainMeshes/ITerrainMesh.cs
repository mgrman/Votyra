using System;
using Votyra.Core.Models;

namespace Votyra.Core.TerrainMeshes
{
    public interface ITerrainMesh
    {
        int VertexCount { get; }

        int TriangleCount { get; }

        void Initialize(Func<Vector3f, Vector3f> vertexPostProcessor, Func<Vector2f, Vector2f> uvAdjustor);
        
        void Reset();

        void AddTriangle(Vector3f a, Vector3f b, Vector3f c);

        void FinalizeMesh();
    }
}