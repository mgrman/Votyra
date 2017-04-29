using UnityEngine;

namespace Votyra.TerrainMeshers.TriangleMesh
{
    public interface ITriangleMesh
    {
        int PointCount { get; }
        int TriangleCount { get; }
        Bounds MeshBounds { get; }
        Vector3[] Vertices { get; }
        Vector3[] Normals { get; }
        Vector2[] UV { get; }
        int[] Indices { get; }

        void Clear(int triangleCount);

        void Clear(Bounds meshBounds);

        void Add(int index, Vector3 posA, Vector3 posB, Vector3 posC);

        void FinalizeMesh();

        //void UpdateMesh(Mesh mesh);
    }
}