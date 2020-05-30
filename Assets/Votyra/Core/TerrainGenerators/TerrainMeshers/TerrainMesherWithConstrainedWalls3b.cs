using System;
using System.Collections.Generic;
using System.Linq;
using Votyra.Core.Images;
using Votyra.Core.ImageSamplers;
using Votyra.Core.Models;
using Votyra.Core.TerrainMeshes;
using Votyra.Core.Utils;

namespace Votyra.Core.TerrainGenerators.TerrainMeshers
{
    public class TerrainMesherWithConstrainedWalls3b : ITerrainMesher3b
    {
        public static readonly Vector3f CenterZeroCell = new Vector3f(0.5f, 0.5f, 0.5f);
        private static readonly List<SampledData3b> DataWithoutTriangles = new List<SampledData3b>();

        private static readonly SampledData3b[] NormalizedSamples = SampledData3b.AllValues.Select(o => ChooseTrianglesForCell(o)
                .Item1)
            .ToArray();

        private static readonly IReadOnlyCollection<Triangle3f>[] MainPlaneTriangles = SampledData3b.AllValues.Select(o => ChooseTrianglesForCell(o)
                .Item2)
            .ToArray();

        private static readonly IReadOnlyCollection<Triangle3f>[] XWallTriangles = SampledDataWithWall.AllValues.Select(o =>
            {
                // normalize data
                var data = NormalizedSamples[o.Data.Data];
                var dataXMinus = NormalizedSamples[o.Wall.Data];

                return ChooseXWallTriangles(data, dataXMinus);
            })
            .ToArray();

        private static readonly IReadOnlyCollection<Triangle3f>[] YWallTriangles = SampledDataWithWall.AllValues.Select(o =>
            {
                // normalize data
                var data = NormalizedSamples[o.Data.Data];
                var dataYMinus = NormalizedSamples[o.Wall.Data];

                return ChooseYWallTriangles(data, dataYMinus);
            })
            .ToArray();

        private readonly Vector3i cellInGroupCount;

        private readonly IImageSampler3 imageSampler;
        private Vector3i groupPosition;
        private Vector3i groupSize;
        private IGeneralMesh pooledMesh;

        public TerrainMesherWithConstrainedWalls3b(ITerrainConfig terrainConfig, IImageSampler3 imageSampler)
        {
            this.imageSampler = imageSampler;
            this.cellInGroupCount = terrainConfig.CellInGroupCount;
        }

        protected IImage3b Image { get; private set; }

        public void Initialize(IImage3b image)
        {
            this.Image = image;
        }

        public void AddCell(Vector3i cellInGroup)
        {
            var cell = cellInGroup + this.groupPosition;

            var data = this.imageSampler.Sample(this.Image, cell);
            var dataXMinus = this.imageSampler.Sample(this.Image, cell + new Vector3i(-1, 0, 0));
            var dataYMinus = this.imageSampler.Sample(this.Image, cell + new Vector3i(0, -1, 0));

            foreach (var tri in MainPlaneTriangles[data.Data])
            {
                this.pooledMesh.AddTriangle(cell + tri.A, cell + tri.B, cell + tri.C);
            }

            foreach (var tri in XWallTriangles[SampledDataWithWall.GetIndexInAllValues(data, dataXMinus)])
            {
                this.pooledMesh.AddTriangle(cell + tri.A, cell + tri.B, cell + tri.C);
            }

            foreach (var tri in YWallTriangles[SampledDataWithWall.GetIndexInAllValues(data, dataYMinus)])
            {
                this.pooledMesh.AddTriangle(cell + tri.A, cell + tri.B, cell + tri.C);
            }
        }

        public void InitializeGroup(Vector3i group, IGeneralMesh cleanPooledMesh)
        {
            var bounds = Range3i.FromMinAndSize(group * this.cellInGroupCount, this.cellInGroupCount)
                .ToArea3f();

            this.groupPosition = this.cellInGroupCount * group;

            this.pooledMesh = cleanPooledMesh;
            this.pooledMesh.Reset(Area3f.FromMinAndSize((group * this.cellInGroupCount).ToVector3f(), this.cellInGroupCount.ToVector3f()));
        }

        private static Tuple<SampledData3b, IReadOnlyCollection<Triangle3f>> ChooseTrianglesForCell(SampledData3b data)
        {
            var posX0Y0Z0 = new Vector3f(0, 0, 0);
            var posX0Y0Z1 = new Vector3f(0, 0, 1);
            var posX0Y1Z0 = new Vector3f(0, 1, 0);
            var posX0Y1Z1 = new Vector3f(0, 1, 1);
            var posX1Y0Z0 = new Vector3f(1, 0, 0);
            var posX1Y0Z1 = new Vector3f(1, 0, 1);
            var posX1Y1Z0 = new Vector3f(1, 1, 0);
            var posX1Y1Z1 = new Vector3f(1, 1, 1);

            // TODO slopes might be usefull to also do only subset, since there can be some small outlier like:
            //   1-----1
            //  /|    /|
            // 1-+---1 |
            // | 0---+-1
            // |/    |/
            // 1-----0

            SampledData3b rotatedTemplate;

            Matrix4X4F matrix;
            if ((data.Data == 0) || (data.Data == byte.MaxValue))
            {
                return Tuple.Create(data, Array.Empty<Triangle3f>() as IReadOnlyCollection<Triangle3f>);
            }

            if (SampledData3b.ParseCube(@"
              0-----0
             /|    /|
            0-+---0 |
            | 1---+-1
            |/    |/
            1-----1
            ")
                .EqualsRotationInvariant(data, out matrix, false, false, true, false)) // plane
            {
                matrix = matrix.Inverse;
                var isInverted = matrix.Determinant < 0;

                return Tuple.Create(data,
                    TerrainMeshUtilities.GetQuadTriangles(matrix.MultiplyPoint(posX0Y0Z0), matrix.MultiplyPoint(posX0Y1Z0), matrix.MultiplyPoint(posX1Y0Z0), matrix.MultiplyPoint(posX1Y1Z0), false)
                        .ChangeOrderIfTrue(isInverted)
                        .ToArray() as IReadOnlyCollection<Triangle3f>);
            }

            if (SampledData3b.ParseCube(@"
              1-----0
             /|    /|
            1-+---0 |
            | 1---+-1
            |/    |/
            1-----1
            ")
                .EqualsRotationInvariant(data, out matrix, false, false, true, false)) // simple slope
            {
                matrix = matrix.Inverse;
                var isInverted = matrix.Determinant < 0;
                return Tuple.Create(data,
                    TerrainMeshUtilities.GetQuadTriangles(matrix.MultiplyPoint(posX0Y0Z1), matrix.MultiplyPoint(posX0Y1Z1), matrix.MultiplyPoint(posX1Y0Z0), matrix.MultiplyPoint(posX1Y1Z0), false)
                        .ChangeOrderIfTrue(isInverted)
                        .ToArray() as IReadOnlyCollection<Triangle3f>);
            }

            if (SampledData3b.ParseCube(@"
              1-----0
             /|    /|
            1-+---1 |
            | 1---+-0
            |/    |/
            1-----1
            ")
                .EqualsRotationInvariant(data, out matrix, false, false, true, false)) // diagonal plane
            {
                matrix = matrix.Inverse;
                var isInverted = matrix.Determinant < 0;
                return Tuple.Create(data,
                    new[]
                        {
                            new Triangle3f(matrix.MultiplyPoint(posX1Y0Z0), matrix.MultiplyPoint(posX0Y1Z0), matrix.MultiplyPoint(posX0Y1Z1)),
                            new Triangle3f(matrix.MultiplyPoint(posX1Y0Z1), matrix.MultiplyPoint(posX1Y0Z0), matrix.MultiplyPoint(posX0Y1Z1)),
                        }.ChangeOrderIfTrue(isInverted)
                        .ToArray() as IReadOnlyCollection<Triangle3f>);
            }

            if (SampledData3b.ParseCube(@"
              0-----1
             /|    /|
            1-+---0 |
            | 1---+-1
            |/    |/
            1-----1
            ")
                .EqualsRotationInvariant(data, out matrix, false, false, true, false)) // dual diagonal slope
            {
                matrix = matrix.Inverse;
                var isInverted = matrix.Determinant < 0;

                return Tuple.Create(data,
                    new[]
                        {
                            new Triangle3f(matrix.MultiplyPoint(posX0Y0Z1), matrix.MultiplyPoint(posX1Y0Z0), matrix.MultiplyPoint(posX1Y1Z1)),
                            new Triangle3f(matrix.MultiplyPoint(posX0Y0Z1), matrix.MultiplyPoint(posX1Y1Z1), matrix.MultiplyPoint(posX0Y1Z0)),
                        }.ChangeOrderIfTrue(isInverted)
                        .ToArray() as IReadOnlyCollection<Triangle3f>);
            }

            if (SampledData3b.ParseCube(@"
              1-----0
             /|    /|
            1-+---1 |
            | 1---+-1
            |/    |/
            1-----1
            ")
                .IsContainedInRotationInvariant(data, out matrix, out rotatedTemplate, false, false, true, false)) // diagonal slope upper
            {
                matrix = matrix.Inverse;
                var isInverted = matrix.Determinant < 0;

                return Tuple.Create(rotatedTemplate,
                    new[]
                        {
                            new Triangle3f(matrix.MultiplyPoint(posX0Y1Z1), matrix.MultiplyPoint(posX1Y0Z1), matrix.MultiplyPoint(posX1Y1Z0)),
                        }.ChangeOrderIfTrue(isInverted)
                        .ToArray() as IReadOnlyCollection<Triangle3f>);
            }

            if (SampledData3b.ParseCube(@"
              0-----0
             /|    /|
            1-+---0 |
            | 1---+-1
            |/    |/
            1-----1
            ")
                .IsContainedInRotationInvariant(data, out matrix, out rotatedTemplate, false, false, true, false)) // diagonal slope, with bottom
            {
                matrix = matrix.Inverse;
                var isInverted = matrix.Determinant < 0;

                return Tuple.Create(rotatedTemplate,
                    new[]
                        {
                            new Triangle3f(matrix.MultiplyPoint(posX1Y0Z0), matrix.MultiplyPoint(posX0Y1Z0), matrix.MultiplyPoint(posX0Y0Z1)),
                            new Triangle3f(matrix.MultiplyPoint(posX1Y0Z0), matrix.MultiplyPoint(posX1Y1Z0), matrix.MultiplyPoint(posX0Y1Z0)),
                        }.ChangeOrderIfTrue(isInverted)
                        .ToArray() as IReadOnlyCollection<Triangle3f>);
            }

            if (SampledData3b.ParseCube(@"
              0-----0
             /|    /|
            1-+---0 |
            | 1---+-0
            |/    |/
            1-----1
            ")
                .IsContainedInRotationInvariant(data, out matrix, out rotatedTemplate, false, false, true, false)) // diagonal slope, no bottom
            {
                matrix = matrix.Inverse;
                var isInverted = matrix.Determinant < 0;

                return Tuple.Create(rotatedTemplate,
                    new Triangle3f(matrix.MultiplyPoint(posX1Y0Z0), matrix.MultiplyPoint(posX0Y1Z0), matrix.MultiplyPoint(posX0Y0Z1)).AsEnumerable()
                        .ChangeOrderIfTrue(isInverted)
                        .ToArray() as IReadOnlyCollection<Triangle3f>);
            }

            if (SampledData3b.ParseCube(@"
              0-----0
             /|    /|
            0-+---0 |
            | 1---+-0
            |/    |/
            1-----1
            ")
                .IsContainedInRotationInvariant(data, out matrix, out rotatedTemplate, false, false, true, false)) // partial plane
            {
                matrix = matrix.Inverse;
                var isInverted = matrix.Determinant < 0;

                return Tuple.Create(rotatedTemplate,
                    new Triangle3f(matrix.MultiplyPoint(posX0Y0Z0), matrix.MultiplyPoint(posX1Y0Z0), matrix.MultiplyPoint(posX0Y1Z0)).AsEnumerable()
                        .ChangeOrderIfTrue(isInverted)
                        .ToArray() as IReadOnlyCollection<Triangle3f>);
            }

            return Tuple.Create(new SampledData3b(false, false, false, false, false, false, false, false), Array.Empty<Triangle3f>() as IReadOnlyCollection<Triangle3f>);
        }

        private static IReadOnlyCollection<Triangle3f> ChooseXWallTriangles(SampledData3b data, SampledData3b dataXMinus)
        {
            var triangles = new List<Triangle3f>();

            var centerXMinus = ((dataXMinus.DataX1Y0Z0 ? 1 : 0) + (dataXMinus.DataX1Y0Z1 ? 1 : 0) + (dataXMinus.DataX1Y1Z0 ? 1 : 0) + (dataXMinus.DataX1Y1Z1 ? 1 : 0)) > 2;
            var centerX = ((data.DataX0Y0Z0 ? 1 : 0) + (data.DataX0Y0Z1 ? 1 : 0) + (data.DataX0Y1Z0 ? 1 : 0) + (data.DataX0Y1Z1 ? 1 : 0)) > 2;

            var isBottomTri = data.DataX0Y0Z0 && data.DataX0Y1Z0 && centerX;
            var isBottomTriMinus = dataXMinus.DataX1Y0Z0 && dataXMinus.DataX1Y1Z0 && centerXMinus;
            if (isBottomTri != isBottomTriMinus)
            {
                triangles.AddTriangle(new Vector3f(0, 0, 0), new Vector3f(0, 1, 0), new Vector3f(0, 0.5f, 0.5f), isBottomTri);
            }

            var isTopTri = data.DataX0Y0Z1 && data.DataX0Y1Z1 && centerX;
            var isTopTriMinus = dataXMinus.DataX1Y0Z1 && dataXMinus.DataX1Y1Z1 && centerXMinus;
            if (isTopTri != isTopTriMinus)
            {
                triangles.AddTriangle(new Vector3f(0, 1, 1), new Vector3f(0, 0, 1), new Vector3f(0, 0.5f, 0.5f), isTopTri);
            }

            var isLeftTri = data.DataX0Y0Z0 && data.DataX0Y0Z1 && centerX;
            var isLeftTriMinus = dataXMinus.DataX1Y0Z0 && dataXMinus.DataX1Y0Z1 && centerXMinus;
            if (isLeftTri != isLeftTriMinus)
            {
                triangles.AddTriangle(new Vector3f(0, 0, 1), new Vector3f(0, 0, 0), new Vector3f(0, 0.5f, 0.5f), isLeftTri);
            }

            var isRightTri = data.DataX0Y1Z0 && data.DataX0Y1Z1 && centerX;
            var isRightTriMinus = dataXMinus.DataX1Y1Z0 && dataXMinus.DataX1Y1Z1 && centerXMinus;
            if (isRightTri != isRightTriMinus)
            {
                triangles.AddTriangle(new Vector3f(0, 1, 0), new Vector3f(0, 1, 1), new Vector3f(0, 0.5f, 0.5f), isRightTri);
            }

            return triangles;
        }

        private static IReadOnlyCollection<Triangle3f> ChooseYWallTriangles(SampledData3b data, SampledData3b dataYMinus)
        {
            var triangles = new List<Triangle3f>();

            var centerYMinus = ((dataYMinus.DataX0Y1Z0 ? 1 : 0) + (dataYMinus.DataX0Y1Z1 ? 1 : 0) + (dataYMinus.DataX1Y1Z0 ? 1 : 0) + (dataYMinus.DataX1Y1Z1 ? 1 : 0)) > 2;
            var centerY = ((data.DataX0Y0Z0 ? 1 : 0) + (data.DataX0Y0Z1 ? 1 : 0) + (data.DataX1Y0Z0 ? 1 : 0) + (data.DataX1Y0Z1 ? 1 : 0)) > 2;

            var isBottomTri = data.DataX0Y0Z0 && data.DataX1Y0Z0 && centerY;
            var isBottomTriMinus = dataYMinus.DataX0Y1Z0 && dataYMinus.DataX1Y1Z0 && centerYMinus;
            if (isBottomTri != isBottomTriMinus)
            {
                triangles.AddTriangle(new Vector3f(0, 0, 0), new Vector3f(1, 0, 0), new Vector3f(0.5f, 0, 0.5f), isBottomTriMinus);
            }

            var isTopTri = data.DataX0Y0Z1 && data.DataX1Y0Z1 && centerY;
            var isTopTriMinus = dataYMinus.DataX0Y1Z1 && dataYMinus.DataX1Y1Z1 && centerYMinus;
            if (isTopTri != isTopTriMinus)
            {
                triangles.AddTriangle(new Vector3f(1, 0, 1), new Vector3f(0, 0, 1), new Vector3f(0.5f, 0, 0.5f), isTopTriMinus);
            }

            var isLeftTri = data.DataX0Y0Z0 && data.DataX0Y0Z1 && centerY;
            var isLeftTriMinus = dataYMinus.DataX0Y1Z0 && dataYMinus.DataX0Y1Z1 && centerYMinus;
            if (isLeftTri != isLeftTriMinus)
            {
                triangles.AddTriangle(new Vector3f(0, 0, 1), new Vector3f(0, 0, 0), new Vector3f(0.5f, 0, 0.5f), isLeftTriMinus);
            }

            var isRightTri = data.DataX1Y0Z0 && data.DataX1Y0Z1 && centerY;
            var isRightTriMinus = dataYMinus.DataX1Y1Z0 && dataYMinus.DataX1Y1Z1 && centerYMinus;
            if (isRightTri != isRightTriMinus)
            {
                triangles.AddTriangle(new Vector3f(1, 0, 0), new Vector3f(1, 0, 1), new Vector3f(0.5f, 0, 0.5f), isRightTriMinus);
            }

            return triangles;
        }

        private struct SampledDataWithWall
        {
            public readonly SampledData3b Data;
            public readonly SampledData3b Wall;

            public SampledDataWithWall(SampledData3b data, SampledData3b wall)
            {
                this.Data = data;
                this.Wall = wall;
            }

            public static IEnumerable<SampledDataWithWall> AllValues
            {
                get
                {
                    foreach (var data in SampledData3b.AllValues)
                    {
                        foreach (var wall in SampledData3b.AllValues)
                        {
                            yield return new SampledDataWithWall(data, wall);
                        }
                    }
                }
            }

            public static int GetIndexInAllValues(SampledData3b data, SampledData3b wall) => (data.Data << 8) | wall.Data;
        }
    }
}