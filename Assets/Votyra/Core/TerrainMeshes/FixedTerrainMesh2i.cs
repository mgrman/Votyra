using System;
using System.Collections.Generic;
using Votyra.Core.Models;
using Votyra.Core.Utils;

namespace Votyra.Core.TerrainMeshes
{
    public class FixedTerrainMesh2i : ITerrainMesh2f
    {
        private const int SubCellToTriangles = 2;
        private const int TriangleToPoints = 3;
        private readonly Vector2i _cellInGroupCount;

        private readonly int[] _indices;
        private readonly Vector2i _meshSubdivision;
        private readonly Vector3f[] _normals;
        private readonly Vector2f[] _uv;
        private readonly Func<Vector2f, Vector2f> _uVAdjustor;
        private readonly Func<Vector3f, Vector3f> _vertexPostProcessor;
        private readonly Vector3f[] _vertices;
        private Vector2f _groupStartPosition;
        private float _maxZ;
        private Area2f _meshBoundsXY;
        private float _minZ;

        public FixedTerrainMesh2i(Vector2i meshSubdivision, Vector2i cellInGroupCount, Func<Vector3f, Vector3f> vertexPostProcessor, Func<Vector2f, Vector2f> uVAdjustor)
        {
            this._meshSubdivision = meshSubdivision;
            this._cellInGroupCount = cellInGroupCount;
            this._vertexPostProcessor = vertexPostProcessor;
            this._uVAdjustor = uVAdjustor;

            var cellCount = cellInGroupCount.AreaSum();
            var subCellCount = cellCount * meshSubdivision.AreaSum();
            var triangleCount = subCellCount * SubCellToTriangles;
            var pointCount = triangleCount * TriangleToPoints;

            this._vertices = new Vector3f[pointCount];
            this._uv = new Vector2f[pointCount];
            this._indices = new int[pointCount];
            for (var i = 0; i < this._indices.Length; i++)
            {
                this._indices[i] = i;
            }

            this._normals = new Vector3f[pointCount];
        }

        public Area3f MeshBounds => Area3f.FromMinAndMax(this._meshBoundsXY.Min.ToVector3f(this._minZ), this._meshBoundsXY.Max.ToVector3f(this._maxZ));

        public IReadOnlyList<Vector3f> Vertices => this._vertices;

        public IReadOnlyList<Vector3f> Normals => this._normals;

        public IReadOnlyList<Vector2f> UV => this._uv;

        public IReadOnlyList<int> Indices => this._indices;

        public uint TriangleCount => this.VertexCount / 3;

        public uint VertexCount => (uint)this._vertices.Length;

        public uint TriangleCapacity => this.TriangleCount;

        public void Reset(Area3f area)
        {
            this._meshBoundsXY = Area2f.FromMinAndMax(area.Min.XY(), area.Max.XY());
            this._groupStartPosition = area.Min.XY();
        }

        public Vector3f Raycast(Ray3f cameraRay)
        {
            //TODO
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

        public float Raycast(Vector2f posXY)
        {
            if (this._vertices == null)
            {
                return Vector1f.NaN;
            }

            var posRelative = posXY - this._meshBoundsXY.Min.XY();

            var cellInGroup = posRelative.FloorToVector2i();
            var subCell = ((posRelative - cellInGroup) * this._meshSubdivision).FloorToVector2i();

            var subCellIndex = this.SubCellIndex(cellInGroup, subCell);

            var triangleIndex = subCellIndex * SubCellToTriangles;
            var pointIndex = triangleIndex * TriangleToPoints;

            var a = this._vertices[pointIndex];
            var b = this._vertices[pointIndex + 1];
            var c = this._vertices[pointIndex + 2];
            var triangle = new Triangle3f(a, b, c);
            var res = triangle.BarycentricCoords(posXY);
            if (res.NoNan())
            {
                return res;
            }

            a = this._vertices[pointIndex + 3];
            b = this._vertices[pointIndex + 4];
            c = this._vertices[pointIndex + 5];
            triangle = new Triangle3f(a, b, c);
            res = triangle.BarycentricCoords(posXY);
            if (res.NoNan())
            {
                return res;
            }

            return Vector1f.NaN;
        }

        public void AddCell(Vector2i cellInGroup, Vector2i subCell, SampledData2f data)
        {
            Vector2f positionMin;
            Vector2f positionMax;
            if (this._meshSubdivision == 1)
            {
                positionMin = this._groupStartPosition + cellInGroup.ToVector2f();
                positionMax = positionMin + Vector2f.One;
            }
            else
            {
                positionMin = this._groupStartPosition + cellInGroup.ToVector2f() + (subCell.ToVector2f() / this._meshSubdivision);
                positionMax = positionMin + (Vector2f.One / this._meshSubdivision);
            }

            var subCellIndex = this.SubCellIndex(cellInGroup, subCell);

            var triangleIndex = subCellIndex * SubCellToTriangles;

            var x0y0 = data.x0y0.IsNotNaN() ? new Vector2f(positionMin.X, positionMin.Y).ToVector3f(data.x0y0) : (Vector3f?)null;
            var x0y1 = data.x0y1.IsNotNaN() ? new Vector2f(positionMin.X, positionMax.Y).ToVector3f(data.x0y1) : (Vector3f?)null;
            var x1y0 = data.x1y0.IsNotNaN() ? new Vector2f(positionMax.X, positionMin.Y).ToVector3f(data.x1y0) : (Vector3f?)null;
            var x1y1 = data.x1y1.IsNotNaN() ? new Vector2f(positionMax.X, positionMax.Y).ToVector3f(data.x1y1) : (Vector3f?)null;

            var holeCount = (x0y0.HasValue ? 0 : 1) + (x0y1.HasValue ? 0 : 1) + (x1y0.HasValue ? 0 : 1) + (x1y1.HasValue ? 0 : 1);

            if (holeCount == 1)
            {
                if (!x0y0.HasValue)
                {
                    this.AddTriangle(triangleIndex, x1y0.Value, x1y1.Value, x0y1.Value);
                    triangleIndex++;
                    this.AddEmptyTriangle(triangleIndex);
                }
                else if (!x0y1.HasValue)
                {
                    // TODO: edge case of edge fliping logic
                    // TODO: make for customizable triangle flipping logic
                    this.AddEmptyTriangle(triangleIndex);
                    // AddTriangle(triangleIndex, x0y0.Value, x1y0.Value, x1y1.Value);
                    triangleIndex++;
                    this.AddEmptyTriangle(triangleIndex);
                }
                else if (!x1y0.HasValue)
                {
                    // TODO: edge case of edge fliping logic
                    // TODO: make for customizable triangle flipping logic
                    this.AddEmptyTriangle(triangleIndex);
                    // AddTriangle(triangleIndex, x1y1.Value, x0y1.Value, x0y0.Value);
                    triangleIndex++;
                    this.AddEmptyTriangle(triangleIndex);
                }
                else if (!x1y1.HasValue)
                {
                    this.AddTriangle(triangleIndex, x0y0.Value, x1y0.Value, x0y1.Value);
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
                if (this.IsFlipped(x0y0.Value, x0y1.Value, x1y0.Value, x1y1.Value))
                {
                    this.AddTriangle(triangleIndex, x0y0.Value, x1y0.Value, x1y1.Value);
                    triangleIndex++;
                    this.AddTriangle(triangleIndex, x1y1.Value, x0y1.Value, x0y0.Value);
                }
                else
                {
                    this.AddTriangle(triangleIndex, x0y0.Value, x1y0.Value, x0y1.Value);
                    triangleIndex++;
                    this.AddTriangle(triangleIndex, x1y0.Value, x1y1.Value, x0y1.Value);
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
            var cellIndex = (uint)(cellInGroup.X + (cellInGroup.Y * this._cellInGroupCount.X));

            uint subCellIndex;
            if (this._meshSubdivision == 1)
            {
                subCellIndex = cellIndex;
            }
            else
            {
                subCellIndex = (uint)((cellIndex * this._meshSubdivision.AreaSum()) + subdivisionCell.X + (subdivisionCell.Y * this._meshSubdivision.X));
            }

            return subCellIndex;
        }

        private void AddEmptyTriangle(uint triangleIndex)
        {
            var pointIndex = triangleIndex * TriangleToPoints;
            this._vertices[pointIndex] = Vector3f.Zero;
            this._uv[pointIndex] = Vector2f.Zero;
            this._normals[pointIndex] = Vector3f.Zero;
            pointIndex++;
            this._vertices[pointIndex] = Vector3f.Zero;
            this._uv[pointIndex] = Vector2f.Zero;
            this._normals[pointIndex] = Vector3f.Zero;
            pointIndex++;
            this._vertices[pointIndex] = Vector3f.Zero;
            this._uv[pointIndex] = Vector2f.Zero;
            this._normals[pointIndex] = Vector3f.Zero;
        }

        private bool IsFlipped(Vector3f x0y0, Vector3f x0y1, Vector3f x1y0, Vector3f x1y1) => false;

        private void AddTriangle(uint triangleIndex, Vector3f posA, Vector3f posB, Vector3f posC)
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
            var normal = Vector3fUtils.Cross(side1, side2);

            var pointIndex = triangleIndex * TriangleToPoints;
            this._minZ = pointIndex == 0 ? posA.Z : Math.Min(this._minZ, posA.Z);
            this._minZ = Math.Min(this._minZ, posB.Z);
            this._minZ = Math.Min(this._minZ, posC.Z);
            this._maxZ = pointIndex == 0 ? posA.Z : Math.Max(this._maxZ, posA.Z);
            this._maxZ = Math.Max(this._maxZ, posB.Z);
            this._maxZ = Math.Max(this._maxZ, posC.Z);

            this._vertices[pointIndex] = posA;
            this._uv[pointIndex] = uvA;
            this._normals[pointIndex] = normal;
            pointIndex++;

            this._vertices[pointIndex] = posB;
            this._uv[pointIndex] = uvB;
            this._normals[pointIndex] = normal;
            pointIndex++;

            this._vertices[pointIndex] = posC;
            this._uv[pointIndex] = uvC;
            this._normals[pointIndex] = normal;
            pointIndex++;
        }
    }
}
