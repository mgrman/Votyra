using System;
using System.Collections.Generic;
using Votyra.Core.Models;

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
            _meshSubdivision = meshSubdivision;
            _cellInGroupCount = cellInGroupCount;
            _vertexPostProcessor = vertexPostProcessor;
            _uVAdjustor = uVAdjustor;

            var cellCount = cellInGroupCount.AreaSum();
            var subCellCount = cellCount * meshSubdivision.AreaSum();
            var triangleCount = subCellCount * SubCellToTriangles;
            var pointCount = triangleCount * TriangleToPoints;

            _vertices = new Vector3f[pointCount];
            _uv = new Vector2f[pointCount];
            _indices = new int[pointCount];
            for (var i = 0; i < _indices.Length; i++)
            {
                _indices[i] = i;
            }

            _normals = new Vector3f[pointCount];
        }

        public Area3f MeshBounds => Area3f.FromMinAndMax(_meshBoundsXY.Min.ToVector3f(_minZ), _meshBoundsXY.Max.ToVector3f(_maxZ));

        public IReadOnlyList<Vector3f> Vertices => _vertices;

        public IReadOnlyList<Vector3f> Normals => _normals;

        public IReadOnlyList<Vector2f> UV => _uv;

        public IReadOnlyList<int> Indices => _indices;

        public uint TriangleCount => VertexCount / 3;

        public uint VertexCount => (uint) _vertices.Length;

        public uint TriangleCapacity => TriangleCount;

        public void Reset(Area3f area)
        {
            _meshBoundsXY = Area2f.FromMinAndMax(area.Min.XY(), area.Max.XY());
            _groupStartPosition = area.Min.XY();
        }

        public Vector3f Raycast(Ray3f cameraRay)
        {
            //TODO
            // Use Cell traversal
            for (var i = 0; i < Vertices.Count; i += 3)
            {
                var a = Vertices[i];
                var b = Vertices[i + 1];
                var c = Vertices[i + 2];
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
            if (_vertices == null)
            {
                return Vector1f.NaN;
            }

            var posRelative = posXY - _meshBoundsXY.Min.XY();

            var cellInGroup = posRelative.FloorToVector2i();
            var subCell = ((posRelative - cellInGroup) * _meshSubdivision).FloorToVector2i();

            var subCellIndex = SubCellIndex(cellInGroup, subCell);

            var triangleIndex = subCellIndex * SubCellToTriangles;
            var pointIndex = triangleIndex * TriangleToPoints;

            var a = _vertices[pointIndex];
            var b = _vertices[pointIndex + 1];
            var c = _vertices[pointIndex + 2];
            var triangle = new Triangle3f(a, b, c);
            var res = triangle.BarycentricCoords(posXY);
            if (res.NoNan())
            {
                return res;
            }

            a = _vertices[pointIndex + 3];
            b = _vertices[pointIndex + 4];
            c = _vertices[pointIndex + 5];
            triangle = new Triangle3f(a, b, c);
            res = triangle.BarycentricCoords(posXY);
            if (res.NoNan())
            {
                return res;
            }

            return Vector1f.NaN;
        }

        public void AddCell(Vector2i cellInGroup, Vector2i subCell, SampledData2f data, SampledMask2e maskData)
        {
            Vector2f positionMin;
            Vector2f positionMax;
            if (_meshSubdivision == 1)
            {
                positionMin = _groupStartPosition + cellInGroup.ToVector2f();
                positionMax = positionMin + Vector2f.One;
            }
            else
            {
                positionMin = _groupStartPosition + cellInGroup.ToVector2f() + subCell.ToVector2f() / _meshSubdivision;
                positionMax = positionMin + Vector2f.One / _meshSubdivision;
            }

            var subCellIndex = SubCellIndex(cellInGroup, subCell);

            var triangleIndex = subCellIndex * SubCellToTriangles;

            var x0y0 = maskData.x0y0.IsNotHole() ? new Vector2f(positionMin.X, positionMin.Y).ToVector3f(data.x0y0) : (Vector3f?) null;
            var x0y1 = maskData.x0y1.IsNotHole() ? new Vector2f(positionMin.X, positionMax.Y).ToVector3f(data.x0y1) : (Vector3f?) null;
            var x1y0 = maskData.x1y0.IsNotHole() ? new Vector2f(positionMax.X, positionMin.Y).ToVector3f(data.x1y0) : (Vector3f?) null;
            var x1y1 = maskData.x1y1.IsNotHole() ? new Vector2f(positionMax.X, positionMax.Y).ToVector3f(data.x1y1) : (Vector3f?) null;

            var holeCount = (x0y0.HasValue ? 0 : 1) + (x0y1.HasValue ? 0 : 1) + (x1y0.HasValue ? 0 : 1) + (x1y1.HasValue ? 0 : 1);

            if (holeCount == 1)
            {
                if (!x0y0.HasValue)
                {
                    AddTriangle(triangleIndex, x1y0.Value, x1y1.Value, x0y1.Value);
                    triangleIndex++;
                    AddEmptyTriangle(triangleIndex);
                }
                else if (!x0y1.HasValue)
                {
                    AddTriangle(triangleIndex, x0y0.Value, x1y0.Value, x1y1.Value);
                    triangleIndex++;
                    AddEmptyTriangle(triangleIndex);
                }
                else if (!x1y0.HasValue)
                {
                    AddTriangle(triangleIndex, x1y1.Value, x0y1.Value, x0y0.Value);
                    triangleIndex++;
                    AddEmptyTriangle(triangleIndex);
                }
                else if (!x1y1.HasValue)
                {
                    AddTriangle(triangleIndex, x0y0.Value, x1y0.Value, x0y1.Value);
                    triangleIndex++;
                    AddEmptyTriangle(triangleIndex);
                }
                else
                {
                    AddEmptyTriangle(triangleIndex);
                    triangleIndex++;
                    AddEmptyTriangle(triangleIndex);
                }
            }
            else if (holeCount == 0)
            {
                if (IsFlipped(x0y0.Value, x0y1.Value, x1y0.Value, x1y1.Value))
                {
                    AddTriangle(triangleIndex, x0y0.Value, x1y0.Value, x1y1.Value);
                    triangleIndex++;
                    AddTriangle(triangleIndex, x1y1.Value, x0y1.Value, x0y0.Value);
                }
                else
                {
                    AddTriangle(triangleIndex, x0y0.Value, x1y0.Value, x0y1.Value);
                    triangleIndex++;
                    AddTriangle(triangleIndex, x1y0.Value, x1y1.Value, x0y1.Value);
                }
            }
            else
            {
                AddEmptyTriangle(triangleIndex);
                triangleIndex++;
                AddEmptyTriangle(triangleIndex);
            }
        }

        public void FinalizeMesh()
        {
        }

        private uint SubCellIndex(Vector2i cellInGroup, Vector2i subdivisionCell)
        {
            var cellIndex = (uint) (cellInGroup.X + cellInGroup.Y * _cellInGroupCount.X);

            uint subCellIndex;
            if (_meshSubdivision == 1)
            {
                subCellIndex = cellIndex;
            }
            else
            {
                subCellIndex = (uint) (cellIndex * _meshSubdivision.AreaSum() + subdivisionCell.X + subdivisionCell.Y * _meshSubdivision.X);
            }

            return subCellIndex;
        }

        private void AddEmptyTriangle(uint triangleIndex)
        {
            var pointIndex = triangleIndex * TriangleToPoints;
            _vertices[pointIndex] = Vector3f.Zero;
            _uv[pointIndex] = Vector2f.Zero;
            _normals[pointIndex] = Vector3f.Zero;
            pointIndex++;
            _vertices[pointIndex] = Vector3f.Zero;
            _uv[pointIndex] = Vector2f.Zero;
            _normals[pointIndex] = Vector3f.Zero;
            pointIndex++;
            _vertices[pointIndex] = Vector3f.Zero;
            _uv[pointIndex] = Vector2f.Zero;
            _normals[pointIndex] = Vector3f.Zero;
        }

        private bool IsFlipped(Vector3f x0y0, Vector3f x0y1, Vector3f x1y0, Vector3f x1y1) => false;

        private void AddTriangle(uint triangleIndex, Vector3f posA, Vector3f posB, Vector3f posC)
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
            var normal = Vector3fUtils.Cross(side1, side2);

            var pointIndex = triangleIndex * TriangleToPoints;
            _minZ = pointIndex == 0 ? posA.Z : Math.Min(_minZ, posA.Z);
            _minZ = Math.Min(_minZ, posB.Z);
            _minZ = Math.Min(_minZ, posC.Z);
            _maxZ = pointIndex == 0 ? posA.Z : Math.Max(_maxZ, posA.Z);
            _maxZ = Math.Max(_maxZ, posB.Z);
            _maxZ = Math.Max(_maxZ, posC.Z);

            _vertices[pointIndex] = posA;
            _uv[pointIndex] = uvA;
            _normals[pointIndex] = normal;
            pointIndex++;

            _vertices[pointIndex] = posB;
            _uv[pointIndex] = uvB;
            _normals[pointIndex] = normal;
            pointIndex++;

            _vertices[pointIndex] = posC;
            _uv[pointIndex] = uvC;
            _normals[pointIndex] = normal;
            pointIndex++;
        }
    }
}
