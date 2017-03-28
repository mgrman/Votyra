using System.Collections.Generic;
using TycoonTerrain.Common.Models;

namespace TycoonTerrain.TerrainMeshers.TriangleMesh
{
    public interface ITriangleMesh
    {
        int PointCount { get; }
        int TriangleCount { get; }
        Bounds MeshBounds { get; }
        IList<Vector3> Vertices { get; }
        IList<Vector3> Normals { get; }
        IList<Vector2> UV { get; }
        IList<int> Indices { get; }

        void Clear(int triangleCount);

        void Clear(Bounds meshBounds);

        void Add(int index, Vector3 posA, Vector3 posB, Vector3 posC);

        void FinilizeMesh();

        //void UpdateMesh(Mesh mesh);
    }
}