using System;
using System.Linq;
using UnityEngine;
using Votyra.Core.Models;
using Votyra.Core.Utils;

namespace Votyra.Core.TerrainMeshes
{
    public class FixedUnityTerrainMesh2i : ITerrainMeshWithFixedCapacity
    {
        private int _counter;

        public Bounds MeshBounds { get; private set; }

        public Func<Vector3f, Vector3f> VertexPostProcessor { get; private set; }
        
        public Func<Vector2f, Vector2f> UVAdjustor { get; private set; }
        
        public Vector3[] Vertices { get; private set; }
        
        public Vector3[] Normals { get; private set; }
        
        public Vector2[] UV { get; private set; }
        
        public int[] Indices { get; private set; }

        public int TriangleCount => _counter / 3;

        public int VertexCount => Vertices.Length;

        public int TriangleCapacity { get; private set; }

        public virtual void Initialize(int triangleCapacity)
        {
            TriangleCapacity = triangleCapacity;

            var pointCount = triangleCapacity * 3;

            Vertices = new Vector3[pointCount];
            UV = new Vector2[pointCount];
            Indices = Enumerable.Range(0, pointCount)
                .ToArray();
            Normals = new Vector3[pointCount];
        }

        public void Initialize(Func<Vector3f, Vector3f> vertexPostProcessor, Func<Vector2f, Vector2f> uvAdjustor)
        {
            VertexPostProcessor = vertexPostProcessor;
            UVAdjustor = uvAdjustor;
        }

        public void Reset(Area3f area)
        {
            MeshBounds = area.ToBounds();
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

            var uvA= posA.XY;
            var uvB= posB.XY;
            var uvC= posC.XY;
            if (UVAdjustor != null)
            {
                uvA = UVAdjustor(posA.XY);
                uvB = UVAdjustor(posB.XY);
                uvC = UVAdjustor(posC.XY);
            }

            var side1 = posB - posA;
            var side2 = posC - posA;
            var normal = Vector3f.Cross(side1, side2);

            unsafe
            {
                Vertices[_counter] = *(Vector3*) &posA;
                UV[_counter] = *(Vector2*) &uvA;
                Normals[_counter] = *(Vector3*) &normal;
                _counter++;

                Vertices[_counter] = *(Vector3*) &posB;
                UV[_counter] = *(Vector2*) &uvB;
                Normals[_counter] = *(Vector3*) &normal;
                _counter++;

                Vertices[_counter] = *(Vector3*) &posC;
                UV[_counter] = *(Vector2*) &uvC;
                Normals[_counter] = *(Vector3*) &normal;
                _counter++;
            }
        }

        public void FinalizeMesh()
        {
            if (_counter != VertexCount)
            {
                for (var i = _counter; i < VertexCount; i++)
                {
                    Vertices[i] = Vector3.zero;
                    UV[i] = Vector2.zero;
                    Normals[i] = Vector3.zero;
                }

                Debug.LogWarning($"Mesh was not fully filled. Expected {VertexCount} points, got {_counter} points!");
            }
        }
    }
}