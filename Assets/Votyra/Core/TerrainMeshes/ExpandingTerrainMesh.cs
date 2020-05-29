using System;
using System.Collections.Generic;
using Votyra.Core.Models;

namespace Votyra.Core.TerrainMeshes
{
    public class ExpandingTerrainMesh : IGeneralMesh
    {
        private readonly List<int> _indices;
        private readonly List<Vector3f> _normals;
        private readonly List<Vector2f> _uv;
        private readonly Func<Vector2f, Vector2f> _uVAdjustor;
        private readonly Func<Vector3f, Vector3f> _vertexPostProcessor;
        private readonly List<Vector3f> _vertices;
        private float _maxZ;

        private Area2f _meshBoundsXY;
        private float _minZ;

        public ExpandingTerrainMesh(Func<Vector3f, Vector3f> vertexPostProcessor, Func<Vector2f, Vector2f> uvAdjustor)
        {
            this._vertexPostProcessor = vertexPostProcessor;
            this._uVAdjustor = uvAdjustor;
            this._vertices = new List<Vector3f>();
            this._uv = new List<Vector2f>();
            this._indices = new List<int>();
            this._normals = new List<Vector3f>();
        }

        public Area3f MeshBounds => Area3f.FromMinAndMax(this._meshBoundsXY.Min.ToVector3f(this._minZ), this._meshBoundsXY.Max.ToVector3f(this._maxZ));

        public IReadOnlyList<Vector3f> Vertices => this._vertices;

        public IReadOnlyList<Vector3f> Normals => this._normals;

        public IReadOnlyList<Vector2f> UV => this._uv;

        public IReadOnlyList<int> Indices => this._indices;

        public uint TriangleCapacity => (uint)this._vertices.Capacity / 3;

        public uint TriangleCount => this.VertexCount / 3;

        public uint VertexCount => (uint)this._vertices.Count;

        public void Reset(Area3f area)
        {
            this._meshBoundsXY = Area2f.FromMinAndMax(area.Min.XY(), area.Max.XY());
            this._vertices.Clear();
            this._uv.Clear();
            this._indices.Clear();
            this._normals.Clear();
        }

        public void AddTriangle(Vector3f posA, Vector3f posB, Vector3f posC)
        {
            if (this._vertexPostProcessor != null)
            {
                posA = this._vertexPostProcessor(posA);
                posB = this._vertexPostProcessor(posB);
                posC = this._vertexPostProcessor(posC);
            }

            var uvA = posA.XY();
            var uvB = posB.XY();
            var uvC = posC.XY();
            if (this._uVAdjustor != null)
            {
                uvA = this._uVAdjustor(posA.XY());
                uvB = this._uVAdjustor(posB.XY());
                uvC = this._uVAdjustor(posC.XY());
            }

            var side1 = posB - posA;
            var side2 = posC - posA;
            var normal = Vector3fUtils.Cross(side1, side2)
                .Normalized();

            this._minZ = this.TriangleCount == 0 ? posA.Z : Math.Min(this._minZ, posA.Z);
            this._minZ = Math.Min(this._minZ, posB.Z);
            this._minZ = Math.Min(this._minZ, posC.Z);
            this._maxZ = this.TriangleCount == 0 ? posA.Z : Math.Max(this._maxZ, posA.Z);
            this._maxZ = Math.Max(this._maxZ, posB.Z);
            this._maxZ = Math.Max(this._maxZ, posC.Z);

            this._indices.Add((int)this.VertexCount);
            this._vertices.Add(posA);
            this._uv.Add(uvA);
            this._normals.Add(normal);

            this._indices.Add((int)this.VertexCount);
            this._vertices.Add(posB);
            this._uv.Add(uvB);
            this._normals.Add(normal);

            this._indices.Add((int)this.VertexCount);
            this._vertices.Add(posC);
            this._uv.Add(uvC);
            this._normals.Add(normal);
        }

        public void FinalizeMesh()
        {
        }

        public IEnumerable<Triangle3f> GetTriangles(Vector2i? limitToCellInGroup)
        {
            for (var i = 0; i < this._vertices.Count; i += 3)
            {
                var a = this._vertices[i];
                var b = this._vertices[i + 1];
                var c = this._vertices[i + 2];
                yield return new Triangle3f(a, b, c);
            }
        }
    }
}
