//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using UnityEngine;


//public class AdaptiveTriangleMesh : ITriangleMesh
//{
//    private int _expectedTriangleCount;
//    private Rect _cell;
//    private Range2 _rangeZ;
//    private readonly List<Vector3> _vertices;
//    private readonly List<Vector3> _normals;
//    private readonly List<Vector2> _uv;
//    private readonly List<int> _indices;
//    private readonly List<Color> _colors;

//    public int ExpectedTriangleCount { get { return _expectedTriangleCount; } }
//    public Rect Cell { get { return _cell; } }
//    public Range2 RangeZ { get { return _rangeZ; } }
//    public IList<Vector3> Vertices { get { return _vertices; } }
//    public IList<Vector3> Normals { get { return _normals; } }
//    public IList<Vector2> UV { get { return _uv; } }
//    public IList<int> Indices { get { return _indices; } }

//    public AdaptiveTriangleMesh(Rect cell, Range2 rangeZ, int triangleCount)
//    {
//        _expectedTriangleCount = triangleCount;

//        int expectedPointCount = triangleCount * 3;

//        _vertices = new List<Vector3>(expectedPointCount);
//        _uv = new List<Vector2>(expectedPointCount);
//        _indices = new List<int>(expectedPointCount);
//        _normals = new List<Vector3>(expectedPointCount);
//        _colors = new List<Color>(expectedPointCount);

//        _cell = cell;
//        _rangeZ = rangeZ;
//    }

//    public void Clear(int triangleCount, Rect cell, Range2 rangeZ)
//    {
//        _expectedTriangleCount = triangleCount;
//        int expectedPointCount = triangleCount * 3;
//        _cell = cell;
//        _rangeZ = rangeZ;

//        _vertices.Clear();
//        _vertices.Capacity = expectedPointCount;

//        _uv.Clear();
//        _uv.Capacity = expectedPointCount;

//        _indices.Clear();
//        _indices.Capacity = expectedPointCount;

//        _normals.Clear();
//        _normals.Capacity = expectedPointCount;

//        _colors.Clear();
//        _colors.Capacity = expectedPointCount;
//    }

//    public void Clear(Rect cell,Range2 rangeZ)
//    {
//        _cell = cell;
//        _rangeZ = rangeZ;

//        _vertices.Clear();

//        _uv.Clear();

//        _indices.Clear();

//        _normals.Clear();

//        _colors.Clear();
//    }

//    public void Add(int index,Vector2i cell, Vector3 posA, Vector3 posB, Vector3 posC)
//    {
//        _vertices.Add(posA);
//        _vertices.Add(posB);
//        _vertices.Add(posC);

//        _uv.Add(new Vector2(posA.x / _cell.width, posA.y / _cell.height));
//        _uv.Add(new Vector2(posB.x / _cell.width, posB.y / _cell.height));
//        _uv.Add(new Vector2(posC.x / _cell.width, posC.y / _cell.height));

//        var side1 = posB - posA;
//        var side2 = posC - posA;
//        var normal = Vector3.Cross(side1, side2).normalized;
//        _normals.Add(normal);
//        _normals.Add(normal);
//        _normals.Add(normal);

//        _indices.Add(_indices.Count);
//        _indices.Add(_indices.Count);
//        _indices.Add(_indices.Count);
//    }

//    public void UpdateMesh(GameObject parentContainer)
//    {
//        var meshFilter = parentContainer.GetComponent<MeshFilter>() ?? parentContainer.AddComponent<MeshFilter>();

//        meshFilter.sharedMesh = meshFilter.sharedMesh ?? new Mesh();

//        var mesh = meshFilter.sharedMesh;

//        if (mesh.vertexCount != this.Vertices.Count)
//        {
//            mesh.Clear();
//            mesh.SetVertices(this._vertices);
//            mesh.SetNormals(this._normals);
//            mesh.SetTriangles(this._indices, 0);
//            mesh.SetUVs(0, this._uv);
//        }
//        else
//        {
//            mesh.SetVertices(this._vertices);
//            mesh.SetNormals(this._normals);
//        }

//        mesh.UpdateBounds(this._cell, this._rangeZ);
//    }
//}
