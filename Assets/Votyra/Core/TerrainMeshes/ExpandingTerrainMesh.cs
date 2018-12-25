using System;
using System.Collections.Generic;
using Votyra.Core.Models;

namespace Votyra.Core.TerrainMeshes
{
    public class ExpandingTerrainMesh : ITerrainMesh
    {
        public ExpandingTerrainMesh()
        {
            Vertices = new List<Vector3f>();
            UV = new List<Vector2f>();
            Indices = new List<int>();
            Normals = new List<Vector3f>();
        }

        public Area3f MeshBounds { get; private set; }
        public Func<Vector3f, Vector3f> VertexPostProcessor { get; private set; }
        public Func<Vector2f, Vector2f> UVAdjustor { get; private set; }
        public List<Vector3f> Vertices { get; }
        public List<Vector3f> Normals { get; }
        public List<Vector2f> UV { get; }
        public List<int> Indices { get; }

        public int TriangleCount { get; private set; }
        public int VertexCount { get; private set; }

        public Vector3f this[int point] => Vertices[point];

        public void Clear(Area3f meshBounds, Func<Vector3f, Vector3f> vertexPostProcessor, Func<Vector2f, Vector2f> uvAdjustor)
        {
            MeshBounds = meshBounds;
            VertexPostProcessor = vertexPostProcessor;
            UVAdjustor = uvAdjustor;
            TriangleCount = 0;
            VertexCount = 0;
            Vertices.Clear();
            UV.Clear();
            Indices.Clear();
            Normals.Clear();
        }

        public void AddTriangle(Vector3f posA, Vector3f posB, Vector3f posC)
        {
            if (VertexPostProcessor != null)
            {
                posA = VertexPostProcessor(posA);
                posB = VertexPostProcessor(posB);
                posC = VertexPostProcessor(posC);
            }
            var side1 = posB - posA;
            var side2 = posC - posA;
            var normal = Vector3f.Cross(side1, side2).Normalized;

            Indices.Add(VertexCount);
            Vertices.Add(posA);
            UV.Add(UVAdjustor?.Invoke(posA.XY) ?? posA.XY);
            Normals.Add(normal);
            VertexCount++;

            Indices.Add(VertexCount);
            Vertices.Add(posB);
            UV.Add(UVAdjustor?.Invoke(posB.XY) ?? posB.XY);
            Normals.Add(normal);
            VertexCount++;

            Indices.Add(VertexCount);
            Vertices.Add(posC);
            UV.Add(UVAdjustor?.Invoke(posC.XY) ?? posC.XY);
            Normals.Add(normal);
            VertexCount++;

            TriangleCount++;
        }

        public void FinalizeMesh()
        {
        }
    }
}