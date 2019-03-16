using System;
using System.Collections.Generic;
using Votyra.Core.Models;
using Votyra.Core.Pooling;

namespace Votyra.Core.TerrainMeshes
{
    public class ExpandingTerrainMesh : IPooledTerrainMesh
    {
        private readonly Func<Vector3f, Vector3f> _vertexPostProcessor;
        private readonly Func<Vector2f, Vector2f> _uVAdjustor;
        private readonly List<int> _indices;
        private readonly List<Vector3f> _normals;
        private readonly List<Vector2f> _uv;
        private readonly List<Vector3f> _vertices;
        private float _maxZ;
        private float _minZ;

        private Area2f _meshBoundsXY;
        public Area3f MeshBounds => Area3f.FromMinAndMax(_meshBoundsXY.Min.ToVector3f(_minZ), _meshBoundsXY.Max.ToVector3f(_maxZ));
        public IReadOnlyList<Vector3f> Vertices => _vertices;
        public IReadOnlyList<Vector3f> Normals => _normals;
        public IReadOnlyList<Vector2f> UV => _uv;
        public IReadOnlyList<int> Indices => _indices;
        public int TriangleCapacity => _vertices.Capacity / 3;
        public int TriangleCount => VertexCount / 3;
        public int VertexCount => _vertices.Count;

        public ExpandingTerrainMesh(Func<Vector3f, Vector3f> vertexPostProcessor, Func<Vector2f, Vector2f> uvAdjustor)
        {
            _vertexPostProcessor = vertexPostProcessor;
            _uVAdjustor = uvAdjustor;
            _vertices = new List<Vector3f>();
            _uv = new List<Vector2f>();
            _indices = new List<int>();
            _normals = new List<Vector3f>();
        }

        public void Reset(Area3f area)
        {
            _meshBoundsXY = Area2f.FromMinAndMax(area.Min.XY(), area.Max.XY());
            _vertices.Clear();
            _uv.Clear();
            _indices.Clear();
            _normals.Clear();
        }

        public void AddTriangle(Vector3f posA, Vector3f posB, Vector3f posC)
        {
            if (_vertexPostProcessor != null)
            {
                posA = _vertexPostProcessor(posA);
                posB = _vertexPostProcessor(posB);
                posC = _vertexPostProcessor(posC);
            }

            var uvA = posA.XY();
            var uvB = posB.XY();
            var uvC = posC.XY();
            if (_uVAdjustor != null)
            {
                uvA = _uVAdjustor(posA.XY());
                uvB = _uVAdjustor(posB.XY());
                uvC = _uVAdjustor(posC.XY());
            }

            var side1 = posB - posA;
            var side2 = posC - posA;
            var normal = Vector3fUtils.Cross(side1, side2)
                .Normalized();

            _minZ = TriangleCount == 0 ? posA.Z : Math.Min(_minZ, posA.Z);
            _minZ = Math.Min(_minZ, posB.Z);
            _minZ = Math.Min(_minZ, posC.Z);
            _maxZ = TriangleCount == 0 ? posA.Z : Math.Max(_maxZ, posA.Z);
            _maxZ = Math.Max(_maxZ, posB.Z);
            _maxZ = Math.Max(_maxZ, posC.Z);

            _indices.Add(VertexCount);
            _vertices.Add(posA);
            _uv.Add(uvA);
            _normals.Add(normal);

            _indices.Add(VertexCount);
            _vertices.Add(posB);
            _uv.Add(uvB);
            _normals.Add(normal);

            _indices.Add(VertexCount);
            _vertices.Add(posC);
            _uv.Add(uvC);
            _normals.Add(normal);
        }

        public void FinalizeMesh()
        {
        }

        public IEnumerable<Triangle3f> GetTriangles(Vector2i? limitToCellInGroup)
        {
            for (var i = 0; i < _vertices.Count; i += 3)
            {
                var a = _vertices[i];
                var b = _vertices[i + 1];
                var c = _vertices[i + 2];
                yield return new Triangle3f(a, b, c);
            }
        }

        public void Return()
        {
            OnReturn?.Invoke(this);
        }

        public event Action<IPooledTerrainMesh> OnReturn;

        int IPoolable<IPooledTerrainMesh, int>.Key => TriangleCapacity;
    }
}