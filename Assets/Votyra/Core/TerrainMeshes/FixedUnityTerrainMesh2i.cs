using System;
using System.Collections.Generic;
using System.Linq;
using Votyra.Core.Logging;
using Votyra.Core.Models;

namespace Votyra.Core.TerrainMeshes
{
    public class FixedUnityTerrainMesh2i : ITerrainMeshWithFixedCapacity
    {
        private int _counter;
        private int[] _indices;

        private float _maxZ;

        private float _minZ;
        private Vector3f[] _normals;
        private Vector2f[] _uv;
        private Vector3f[] _vertices;

        public Area2f MeshBoundsXY { get; private set; }

        public Func<Vector3f, Vector3f> VertexPostProcessor { get; private set; }
        public Func<Vector2f, Vector2f> UVAdjustor { get; private set; }

        public Area3f MeshBounds => Area3f.FromMinAndMax(MeshBoundsXY.Min.ToVector3f(_minZ), MeshBoundsXY.Max.ToVector3f(_maxZ));

        public IReadOnlyList<Vector3f> Vertices => _vertices;

        public IReadOnlyList<Vector3f> Normals => _normals;

        public IReadOnlyList<Vector2f> UV => _uv;

        public IReadOnlyList<int> Indices => _indices;

        public int TriangleCount => _counter / 3;

        public int VertexCount => _vertices.Length;

        public int TriangleCapacity { get; private set; }

        public virtual void Initialize(int triangleCapacity)
        {
            TriangleCapacity = triangleCapacity;

            var pointCount = triangleCapacity * 3;

            _vertices = new Vector3f[pointCount];
            _uv = new Vector2f[pointCount];
            _indices = Enumerable.Range(0, pointCount)
                .ToArray();
            _normals = new Vector3f[pointCount];
        }

        public void Initialize(Func<Vector3f, Vector3f> vertexPostProcessor, Func<Vector2f, Vector2f> uvAdjustor)
        {
            VertexPostProcessor = vertexPostProcessor;
            UVAdjustor = uvAdjustor;
        }

        public void Reset(Area3f area)
        {
            MeshBoundsXY = Area2f.FromMinAndMax(area.Min.XY(), area.Max.XY());
            _counter = 0;
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
            var normal = Vector3fUtils.Cross(side1, side2);

            _minZ = _counter == 0 ? posA.Z : Math.Min(_minZ, posA.Z);
            _minZ = Math.Min(_minZ, posB.Z);
            _minZ = Math.Min(_minZ, posC.Z);
            _maxZ = _counter == 0 ? posA.Z : Math.Max(_maxZ, posA.Z);
            _maxZ = Math.Max(_maxZ, posB.Z);
            _maxZ = Math.Max(_maxZ, posC.Z);

            _vertices[_counter] = posA;
            _uv[_counter] = uvA;
            _normals[_counter] = normal;
            _counter++;

            _vertices[_counter] = posB;
            _uv[_counter] = uvB;
            _normals[_counter] = normal;
            _counter++;

            _vertices[_counter] = posC;
            _uv[_counter] = uvC;
            _normals[_counter] = normal;
            _counter++;
        }

        public void FinalizeMesh()
        {
            if (_counter != VertexCount)
            {
                for (var i = _counter; i < VertexCount; i++)
                {
                    _vertices[i] = Vector3f.Zero;
                    _uv[i] = Vector2f.Zero;
                    _normals[i] = Vector3f.Zero;
                }

                StaticLogger.LogWarning($"Mesh was not fully filled. Expected {VertexCount} points, got {_counter} points!");
            }
        }

        public IEnumerable<Triangle3f> GetTriangles(Vector2i? limitToCellInGroup)
        {
            for (var i = 0; i < _vertices.Length; i += 3)
            {
                var a = _vertices[i];
                var b = _vertices[i + 1];
                var c = _vertices[i + 2];
                yield return new Triangle3f(a, b, c);
            }
        }
    }
}