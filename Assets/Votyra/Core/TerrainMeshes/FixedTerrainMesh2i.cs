using System;
using System.Collections.Generic;
using Votyra.Core.Models;
using Votyra.Core.Utils;

namespace Votyra.Core.TerrainMeshes
{
    public class FixedTerrainMesh2I : ITerrainMesh2F
    {
        private const int SubCellToTriangles = 2;
        private const int TriangleToPoints = 3;
        private readonly Vector2i cellInGroupCount;

        private readonly int[] indices;
        private readonly Vector2i meshSubdivision;
        private readonly Vector3f[] normals;
        private readonly Vector2f[] uv;
        private readonly Func<Vector2f, Vector2f> uVAdjustor;
        private readonly Func<Vector3f, Vector3f> vertexPostProcessor;
        private readonly Vector3f[] vertices;
        private Vector2f groupStartPosition;
        private float maxZ;
        private Area2f meshBoundsXy;
        private float minZ;

        public FixedTerrainMesh2I(Vector2i meshSubdivision, Vector2i cellInGroupCount, Func<Vector3f, Vector3f> vertexPostProcessor, Func<Vector2f, Vector2f> uVAdjustor)
        {
            this.meshSubdivision = meshSubdivision;
            this.cellInGroupCount = cellInGroupCount;
            this.vertexPostProcessor = vertexPostProcessor;
            this.uVAdjustor = uVAdjustor;

            var cellCount = cellInGroupCount.AreaSum();
            var subCellCount = cellCount * meshSubdivision.AreaSum();
            var triangleCount = subCellCount * SubCellToTriangles;
            var pointCount = triangleCount * TriangleToPoints;

            this.vertices = new Vector3f[pointCount];
            this.uv = new Vector2f[pointCount];
            this.indices = new int[pointCount];
            for (var i = 0; i < this.indices.Length; i++)
            {
                this.indices[i] = i;
            }

            this.normals = new Vector3f[pointCount];
        }

        public Area3f MeshBounds => Area3f.FromMinAndMax(this.meshBoundsXy.Min.ToVector3f(this.minZ), this.meshBoundsXy.Max.ToVector3f(this.maxZ));

        public IReadOnlyList<Vector3f> Vertices => this.vertices;

        public IReadOnlyList<Vector3f> Normals => this.normals;

        public IReadOnlyList<Vector2f> Uv => this.uv;

        public IReadOnlyList<int> Indices => this.indices;

        public uint TriangleCount => this.VertexCount / 3;

        public uint VertexCount => (uint)this.vertices.Length;

        public uint TriangleCapacity => this.TriangleCount;

        public void Reset(Area3f area)
        {
            this.meshBoundsXy = Area2f.FromMinAndMax(area.Min.XY(), area.Max.XY());
            this.groupStartPosition = area.Min.XY();
        }

        public Vector3f Raycast(Ray3f cameraRay)
        {
            // TODO
            // Use Cell traversal
            for (var i = 0; i < this.Vertices.Count; i += 3)
            {
                var a = this.Vertices[i];
                var b = this.Vertices[i + 1];
                var c = this.Vertices[i + 2];
                var triangle = new Triangle3f(a, b, c);
                var res = triangle.Intersect(cameraRay);
                if (res.NoNan())
                {
                    return res;
                }
            }

            return Vector3f.NaN;
        }

        public float Raycast(Vector2f posXy)
        {
            if (this.vertices == null)
            {
                return Vector1f.NaN;
            }

            var posRelative = posXy - this.meshBoundsXy.Min.XY();

            var cellInGroup = posRelative.FloorToVector2i();
            var subCell = ((posRelative - cellInGroup) * this.meshSubdivision).FloorToVector2i();

            var subCellIndex = this.SubCellIndex(cellInGroup, subCell);

            var triangleIndex = subCellIndex * SubCellToTriangles;
            var pointIndex = triangleIndex * TriangleToPoints;

            var a = this.vertices[pointIndex];
            var b = this.vertices[pointIndex + 1];
            var c = this.vertices[pointIndex + 2];
            var triangle = new Triangle3f(a, b, c);
            var res = triangle.BarycentricCoords(posXy);
            if (res.NoNan())
            {
                return res;
            }

            a = this.vertices[pointIndex + 3];
            b = this.vertices[pointIndex + 4];
            c = this.vertices[pointIndex + 5];
            triangle = new Triangle3f(a, b, c);
            res = triangle.BarycentricCoords(posXy);
            if (res.NoNan())
            {
                return res;
            }

            return Vector1f.NaN;
        }

        public void AddCell(Vector2i cellInGroup, Vector2i subCell, SampledData2F data)
        {
            Vector2f positionMin;
            Vector2f positionMax;
            if (this.meshSubdivision == 1)
            {
                positionMin = this.groupStartPosition + cellInGroup.ToVector2f();
                positionMax = positionMin + Vector2f.One;
            }
            else
            {
                positionMin = this.groupStartPosition + cellInGroup.ToVector2f() + (subCell.ToVector2f() / this.meshSubdivision);
                positionMax = positionMin + (Vector2f.One / this.meshSubdivision);
            }

            var subCellIndex = this.SubCellIndex(cellInGroup, subCell);

            var triangleIndex = subCellIndex * SubCellToTriangles;

            var x0Y0 = data.X0Y0.IsNotNaN() ? new Vector2f(positionMin.X, positionMin.Y).ToVector3f(data.X0Y0) : (Vector3f?)null;
            var x0Y1 = data.X0Y1.IsNotNaN() ? new Vector2f(positionMin.X, positionMax.Y).ToVector3f(data.X0Y1) : (Vector3f?)null;
            var x1Y0 = data.X1Y0.IsNotNaN() ? new Vector2f(positionMax.X, positionMin.Y).ToVector3f(data.X1Y0) : (Vector3f?)null;
            var x1Y1 = data.X1Y1.IsNotNaN() ? new Vector2f(positionMax.X, positionMax.Y).ToVector3f(data.X1Y1) : (Vector3f?)null;

            var holeCount = (x0Y0.HasValue ? 0 : 1) + (x0Y1.HasValue ? 0 : 1) + (x1Y0.HasValue ? 0 : 1) + (x1Y1.HasValue ? 0 : 1);

            if (holeCount == 1)
            {
                if (!x0Y0.HasValue)
                {
                    this.AddTriangle(triangleIndex, x1Y0.Value, x1Y1.Value, x0Y1.Value);
                    triangleIndex++;
                    this.AddEmptyTriangle(triangleIndex);
                }
                else if (!x0Y1.HasValue)
                {
                    // TODO: edge case of edge fliping logic
                    // TODO: make for customizable triangle flipping logic
                    this.AddEmptyTriangle(triangleIndex);

                    // AddTriangle(triangleIndex, x0y0.Value, x1y0.Value, x1y1.Value);
                    triangleIndex++;
                    this.AddEmptyTriangle(triangleIndex);
                }
                else if (!x1Y0.HasValue)
                {
                    // TODO: edge case of edge fliping logic
                    // TODO: make for customizable triangle flipping logic
                    this.AddEmptyTriangle(triangleIndex);
                    // AddTriangle(triangleIndex, x1y1.Value, x0y1.Value, x0y0.Value);
                    triangleIndex++;
                    this.AddEmptyTriangle(triangleIndex);
                }
                else if (!x1Y1.HasValue)
                {
                    this.AddTriangle(triangleIndex, x0Y0.Value, x1Y0.Value, x0Y1.Value);
                    triangleIndex++;
                    this.AddEmptyTriangle(triangleIndex);
                }
                else
                {
                    this.AddEmptyTriangle(triangleIndex);
                    triangleIndex++;
                    this.AddEmptyTriangle(triangleIndex);
                }
            }
            else if (holeCount == 0)
            {
                if (this.IsFlipped(x0Y0.Value, x0Y1.Value, x1Y0.Value, x1Y1.Value))
                {
                    this.AddTriangle(triangleIndex, x0Y0.Value, x1Y0.Value, x1Y1.Value);
                    triangleIndex++;
                    this.AddTriangle(triangleIndex, x1Y1.Value, x0Y1.Value, x0Y0.Value);
                }
                else
                {
                    this.AddTriangle(triangleIndex, x0Y0.Value, x1Y0.Value, x0Y1.Value);
                    triangleIndex++;
                    this.AddTriangle(triangleIndex, x1Y0.Value, x1Y1.Value, x0Y1.Value);
                }
            }
            else
            {
                this.AddEmptyTriangle(triangleIndex);
                triangleIndex++;
                this.AddEmptyTriangle(triangleIndex);
            }
        }

        public void FinalizeMesh()
        {
        }

        private uint SubCellIndex(Vector2i cellInGroup, Vector2i subdivisionCell)
        {
            var cellIndex = (uint)(cellInGroup.X + (cellInGroup.Y * this.cellInGroupCount.X));

            uint subCellIndex;
            if (this.meshSubdivision == 1)
            {
                subCellIndex = cellIndex;
            }
            else
            {
                subCellIndex = (uint)((cellIndex * this.meshSubdivision.AreaSum()) + subdivisionCell.X + (subdivisionCell.Y * this.meshSubdivision.X));
            }

            return subCellIndex;
        }

        private void AddEmptyTriangle(uint triangleIndex)
        {
            var pointIndex = triangleIndex * TriangleToPoints;
            this.vertices[pointIndex] = Vector3f.Zero;
            this.uv[pointIndex] = Vector2f.Zero;
            this.normals[pointIndex] = Vector3f.Zero;
            pointIndex++;
            this.vertices[pointIndex] = Vector3f.Zero;
            this.uv[pointIndex] = Vector2f.Zero;
            this.normals[pointIndex] = Vector3f.Zero;
            pointIndex++;
            this.vertices[pointIndex] = Vector3f.Zero;
            this.uv[pointIndex] = Vector2f.Zero;
            this.normals[pointIndex] = Vector3f.Zero;
        }

        private bool IsFlipped(Vector3f x0Y0, Vector3f x0Y1, Vector3f x1Y0, Vector3f x1Y1) => false;

        private void AddTriangle(uint triangleIndex, Vector3f posA, Vector3f posB, Vector3f posC)
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
            var normal = Vector3fUtils.Cross(side1, side2);

            var pointIndex = triangleIndex * TriangleToPoints;
            this.minZ = pointIndex == 0 ? posA.Z : Math.Min(this.minZ, posA.Z);
            this.minZ = Math.Min(this.minZ, posB.Z);
            this.minZ = Math.Min(this.minZ, posC.Z);
            this.maxZ = pointIndex == 0 ? posA.Z : Math.Max(this.maxZ, posA.Z);
            this.maxZ = Math.Max(this.maxZ, posB.Z);
            this.maxZ = Math.Max(this.maxZ, posC.Z);

            this.vertices[pointIndex] = posA;
            this.uv[pointIndex] = uvA;
            this.normals[pointIndex] = normal;
            pointIndex++;

            this.vertices[pointIndex] = posB;
            this.uv[pointIndex] = uvB;
            this.normals[pointIndex] = normal;
            pointIndex++;

            this.vertices[pointIndex] = posC;
            this.uv[pointIndex] = uvC;
            this.normals[pointIndex] = normal;
            pointIndex++;
        }
    }
}
