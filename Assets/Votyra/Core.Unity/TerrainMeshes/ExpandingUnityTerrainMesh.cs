using System;
using System.Collections.Generic;
using UnityEngine;
using Votyra.Core.Models;
using Votyra.Core.Utils;

namespace Votyra.Core.TerrainMeshes
{
    public class ExpandingUnityTerrainMesh : ITerrainMesh
    {
        public ExpandingUnityTerrainMesh()
        {
            Vertices = new List<Vector3>();
            UV = new List<Vector2>();
            Indices = new List<int>();
            Normals = new List<Vector3>();
        }

        public Bounds MeshBounds { get; private set; }

        private Func<Vector3f, Vector3f> VertexPostProcessor { get; set; }
        private Func<Vector2f, Vector2f> UVAdjustor { get; set; }

        public List<Vector3> Vertices { get; }
        public List<Vector3> Normals { get; }
        public List<Vector2> UV { get; }
        public List<int> Indices { get; }

        public int TriangleCount { get; private set; }
        public int VertexCount { get; private set; }

        public void Initialize(Func<Vector3f, Vector3f> vertexPostProcessor, Func<Vector2f, Vector2f> uvAdjustor)
        {
            VertexPostProcessor = vertexPostProcessor;
            UVAdjustor = uvAdjustor;
        }

        public void Reset(Area3f area)
        {
            MeshBounds = area.ToBounds();
            TriangleCount = 0;
            VertexCount = 0;
            Vertices.Clear();
            UV.Clear();
            Indices.Clear();
            Normals.Clear();
        }
        
        public void AddTriangle(Vector3f posA, Vector3f posB, Vector3f posC)
        {
            Vector3 posAu;
            Vector3 posBu;
            Vector3 posCu;
            if (VertexPostProcessor != null)
            {
                posAu = VertexPostProcessor(posA)
                    .ToVector3();
                posBu = VertexPostProcessor(posB)
                    .ToVector3();
                posCu = VertexPostProcessor(posC)
                    .ToVector3();
            }
            else
            {
                posAu = posA.ToVector3();
                posBu = posB.ToVector3();
                posCu = posC.ToVector3();
            }

            Vector2 uvAu;
            Vector2 uvBu;
            Vector2 uvCu;
            if (UVAdjustor != null)
            {
                uvAu = UVAdjustor(posA.XY)
                    .ToVector2();
                uvBu = UVAdjustor(posB.XY)
                    .ToVector2();
                uvCu = UVAdjustor(posC.XY)
                    .ToVector2();
            }
            else
            {
                uvAu = posA.XY.ToVector2();
                uvBu = posB.XY.ToVector2();
                uvCu = posC.XY.ToVector2();
            }
            
            var side1 = posBu - posAu;
            var side2 = posCu - posAu;
            var normal = Vector3.Cross(side1, side2)
                .normalized;

            Indices.Add(VertexCount);
            Vertices.Add(posAu);
            UV.Add(uvAu);
            Normals.Add(normal);
            VertexCount++;

            Indices.Add(VertexCount);
            Vertices.Add(posBu);
            UV.Add(uvBu);
            Normals.Add(normal);
            VertexCount++;

            Indices.Add(VertexCount);
            Vertices.Add(posCu);
            UV.Add(uvCu);
            Normals.Add(normal);
            VertexCount++;

            TriangleCount++;
        }

        public void FinalizeMesh()
        {
        }
    }
}