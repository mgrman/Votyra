using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public interface ITriangleMesh
{
    int TriangleCount { get; }
    Bounds MeshBounds { get; }
    IList<Vector3> Vertices { get; }
    IList<Vector3> Normals { get; }
    IList<Vector2> UV { get; }
    IList<int> Indices { get; }

    void Clear(int triangleCount);

    void Clear(Bounds meshBounds);

    void Add(int index, Vector3 posA, Vector3 posB, Vector3 posC);

    void UpdateMesh(Mesh mesh);
}
