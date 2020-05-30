using System;
using System.Collections.Generic;
using Votyra.Core.Models;

namespace Votyra.Core.TerrainMeshes
{
    public class ExpandingTerrainMesh : IGeneralMesh
    {
        private readonly List<int> indices;
        private readonly List<Vector3f> normals;
        private readonly List<Vector2f> uv;
        private readonly Func<Vector2f, Vector2f> uVAdjustor;
        private readonly Func<Vector3f, Vector3f> vertexPostProcessor;
        private readonly List<Vector3f> vertices;
        private float maxZ;

        private Area2f meshBoundsXy;
        private float minZ;

        public ExpandingTerrainMesh(Func<Vector3f, Vector3f> vertexPostProcessor, Func<Vector2f, Vector2f> uvAdjustor)
        {
            this.vertexPostProcessor = vertexPostProcessor;
            this.uVAdjustor = uvAdjustor;
            this.vertices = new List<Vector3f>();
            this.uv = new List<Vector2f>();
            this.indices = new List<int>();
            this.normals = new List<Vector3f>();
        }

        public Area3f MeshBounds => Area3f.FromMinAndMax(this.meshBoundsXy.Min.ToVector3f(this.minZ), this.meshBoundsXy.Max.ToVector3f(this.maxZ));

        public IReadOnlyList<Vector3f> Vertices => this.vertices;

        public IReadOnlyList<Vector3f> Normals => this.normals;

        public IReadOnlyList<Vector2f> Uv => this.uv;

        public IReadOnlyList<int> Indices => this.indices;

        public uint TriangleCapacity => (uint)this.vertices.Capacity / 3;

        public uint TriangleCount => this.VertexCount / 3;

        public uint VertexCount => (uint)this.vertices.Count;

        public void Reset(Area3f area)
        {
            this.meshBoundsXy = Area2f.FromMinAndMax(area.Min.XY(), area.Max.XY());
            this.vertices.Clear();
            this.uv.Clear();
            this.indices.Clear();
            this.normals.Clear();
        }

        public void AddTriangle(Vector3f posA, Vector3f posB, Vector3f posC)
        {
            if (this.vertexPostProcessor != null)
            {
                posA = this.vertexPostProcessor(posA);
                posB = this.vertexPostProcessor(posB);
                posC = this.vertexPostProcessor(posC);
            }

            var uvA = posA.XY();
            var uvB = posB.XY();
            var uvC = posC.XY();
            if (this.uVAdjustor != null)
            {
                uvA = this.uVAdjustor(posA.XY());
                uvB = this.uVAdjustor(posB.XY());
                uvC = this.uVAdjustor(posC.XY());
            }

            var side1 = posB - posA;
            var side2 = posC - posA;
            var normal = Vector3fUtils.Cross(side1, side2)
                .Normalized();

            this.minZ = this.TriangleCount == 0 ? posA.Z : Math.Min(this.minZ, posA.Z);
            this.minZ = Math.Min(this.minZ, posB.Z);
            this.minZ = Math.Min(this.minZ, posC.Z);
            this.maxZ = this.TriangleCount == 0 ? posA.Z : Math.Max(this.maxZ, posA.Z);
            this.maxZ = Math.Max(this.maxZ, posB.Z);
            this.maxZ = Math.Max(this.maxZ, posC.Z);

            this.indices.Add((int)this.VertexCount);
            this.vertices.Add(posA);
            this.uv.Add(uvA);
            this.normals.Add(normal);

            this.indices.Add((int)this.VertexCount);
            this.vertices.Add(posB);
            this.uv.Add(uvB);
            this.normals.Add(normal);

            this.indices.Add((int)this.VertexCount);
            this.vertices.Add(posC);
            this.uv.Add(uvC);
            this.normals.Add(normal);
        }

        public void FinalizeMesh()
        {
        }

        public IEnumerable<Triangle3f> GetTriangles(Vector2i? limitToCellInGroup)
        {
            for (var i = 0; i < this.vertices.Count; i += 3)
            {
                var a = this.vertices[i];
                var b = this.vertices[i + 1];
                var c = this.vertices[i + 2];
                yield return new Triangle3f(a, b, c);
            }
        }
    }
}
