using System;
using System.Collections.Generic;
using Votyra.Core.Models;

namespace Votyra.Core.TerrainMeshes
{
    public class ExpandingUnityTerrainMesh : ITerrainMesh
    {
        private readonly List<int> _indices;
        private readonly List<Vector3f> _normals;
        private readonly List<Vector2f> _uv;
        private readonly List<Vector3f> _vertices;
        private float _maxZ;

        private float _minZ;

        public ExpandingUnityTerrainMesh()
        {
            _vertices = new List<Vector3f>();
            _uv = new List<Vector2f>();
            _indices = new List<int>();
            _normals = new List<Vector3f>();
        }

        public Area2f MeshBoundsXY { get; private set; }
        private Func<Vector3f, Vector3f> VertexPostProcessor { get; set; }
        private Func<Vector2f, Vector2f> UVAdjustor { get; set; }

        public Area3f MeshBounds => Area3f.FromMinAndMax(MeshBoundsXY.Min.ToVector3f(_minZ), MeshBoundsXY.Max.ToVector3f(_maxZ));

        public IReadOnlyList<Vector3f> Vertices => _vertices;

        public IReadOnlyList<Vector3f> Normals => _normals;

        public IReadOnlyList<Vector2f> UV => _uv;

        public IReadOnlyList<int> Indices => _indices;

        public int TriangleCount { get; private set; }
        public int VertexCount { get; private set; }

        public void Initialize(Func<Vector3f, Vector3f> vertexPostProcessor, Func<Vector2f, Vector2f> uvAdjustor)
        {
            VertexPostProcessor = vertexPostProcessor;
            UVAdjustor = uvAdjustor;
        }

        public void Reset(Area3f area)
        {
            MeshBoundsXY = Area2f.FromMinAndMax(area.Min.XY(), area.Max.XY());
            TriangleCount = 0;
            VertexCount = 0;
            _vertices.Clear();
            _uv.Clear();
            _indices.Clear();
            _normals.Clear();
        }

        public void AddTriangle(Vector3f posA, Vector3f posB, Vector3f posC)
        {
            if (VertexPostProcessor != null)
            {
                posA = VertexPostProcessor(posA);
                posB = VertexPostProcessor(posB);
                posC = VertexPostProcessor(posC);
            }

            var uvA = posA.XY();
            var uvB = posB.XY();
            var uvC = posC.XY();
            if (UVAdjustor != null)
            {
                uvA = UVAdjustor(posA.XY());
                uvB = UVAdjustor(posB.XY());
                uvC = UVAdjustor(posC.XY());
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
            VertexCount++;

            _indices.Add(VertexCount);
            _vertices.Add(posB);
            _uv.Add(uvB);
            _normals.Add(normal);
            VertexCount++;

            _indices.Add(VertexCount);
            _vertices.Add(posC);
            _uv.Add(uvC);
            _normals.Add(normal);
            VertexCount++;

            TriangleCount++;
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
    }
}