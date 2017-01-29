using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class FixedTriangleMesh : ITriangleMesh
{
    private Bounds _meshBounds;
    private Vector3[] _vertices;
    private Vector3[] _normals;
    private Vector2[] _uv;
    private int[] _indices;

    public int PointCount { get { return _vertices.Length; } }
    public int TriangleCount { get { return PointCount / 3; } }
    public Bounds MeshBounds { get { return _meshBounds; } }
    public IList<Vector3> Vertices { get { return _vertices; } }
    public IList<Vector3> Normals { get { return _normals; } }
    public IList<Vector2> UV { get { return _uv; } }
    public IList<int> Indices { get { return _indices; } }

    public FixedTriangleMesh(Bounds meshBounds, int triangleCount)
    {
        Clear(triangleCount);
        Clear( meshBounds);
    }
    public FixedTriangleMesh(int triangleCount)
    {
        Clear(triangleCount);
    }

    public FixedTriangleMesh(Bounds meshBounds, int triangleCount, ICollection<Vector3> vertices,
        ICollection<Vector3> normals,
        ICollection<Vector2> uv,
        ICollection<int> indices)
    {
        int pointCount = triangleCount * 3;
        if (pointCount != vertices.Count || pointCount != normals.Count || pointCount != uv.Count || pointCount != indices.Count)
        {
            throw new InvalidOperationException();
        }

        _meshBounds = meshBounds;
        _vertices = vertices.ToArray();
        _uv = uv.ToArray();
        _indices = indices.ToArray();
        _normals = normals.ToArray();
    }

    public void Clear(int triangleCount)
    {
        int pointCount = triangleCount * 3;

        _vertices = new Vector3[pointCount];
        _uv = new Vector2[pointCount];
        _indices = Enumerable.Range(0, pointCount).ToArray();
        _normals = new Vector3[pointCount];
        
    }

    public void Clear(Bounds meshBounds)
    {
        _meshBounds = meshBounds;
    }

    public void Add(int index, Vector3 posA, Vector3 posB, Vector3 posC)
    {
        _vertices[index * 3 + 0] = posA;
        _vertices[index * 3 + 1] = posB;
        _vertices[index * 3 + 2] = posC;

        var size = _meshBounds.size;
        _uv[index * 3 + 0] = new Vector2(posA.x / size.x, posA.y / size.y);
        _uv[index * 3 + 1] = new Vector2(posB.x / size.x, posB.y / size.y);
        _uv[index * 3 + 2] = new Vector2(posC.x / size.x, posC.y / size.y);
        
        var side1 = posB - posA;
        var side2 = posC - posA;
        var normal = Vector3.Cross(side1, side2).normalized;
        _normals[index * 3 + 0] = normal;
        _normals[index * 3 + 1] = normal;
        _normals[index * 3 + 2] = normal;
    }

    public void UpdateMesh(Mesh mesh)
    {
        if (mesh.vertexCount != this.PointCount)
        {
            mesh.Clear();
            mesh.vertices = this._vertices;
            mesh.normals = this._normals;
            mesh.uv = this._uv;
            mesh.triangles = this._indices;
        }
        else
        {
            mesh.vertices = this._vertices;
            mesh.normals = this._normals;
        }

        mesh.bounds = this.MeshBounds;
    }
}
