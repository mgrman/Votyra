using System;
using System.Collections.Generic;
using System.Linq;
using Votyra.Core.Images;
using Votyra.Core.ImageSamplers;
using Votyra.Core.Models;
using Votyra.Core.Pooling;
using Votyra.Core.TerrainMeshes;
using Votyra.Core.Utils;

namespace Votyra.Core.TerrainGenerators.TerrainMeshers
{
    public class TerrainMesherWithWalls3b : ITerrainMesher3b
    {
        public static readonly Vector3f CenterZeroCell = new Vector3f(0.5f, 0.5f, 0.5f);
        protected Vector3i groupPosition;
        protected Vector3i groupSize;
        protected ITerrainMesh mesh;
        protected IPooledTerrainMesh pooledMesh;
        private static readonly List<SampledData3b> DataWithoutTriangles = new List<SampledData3b>();

        private static readonly IReadOnlyCollection<Triangle3f>[] MainPlaneTriangles = SampledData3b.AllValues
            .Select(o => ChooseTrianglesForCell(o).Item2).ToArray();

        private static readonly SampledData3b[] NormalizedSamples = SampledData3b.AllValues
            .Select(o => ChooseTrianglesForCell(o).Item1).ToArray();

        private static readonly IReadOnlyCollection<Triangle3f>[] XWallTriangles = SampledDataWithWall.AllValues
            .Select(o =>
           {
               // normalize data
               var data = NormalizedSamples[o.Data.Data];
               var dataXMinus = NormalizedSamples[o.Wall.Data];

               return ChooseXWallTriangles(data, dataXMinus);
           }).ToArray();

        private static readonly IReadOnlyCollection<Triangle3f>[] YWallTriangles = SampledDataWithWall.AllValues
            .Select(o =>
            {
                // normalize data
                var data = NormalizedSamples[o.Data.Data];
                var dataYMinus = NormalizedSamples[o.Wall.Data];

                return ChooseYWallTriangles(data, dataYMinus);
            }).ToArray();

        private static readonly IReadOnlyCollection<Triangle3f>[] ZWallTriangles = SampledDataWithWall.AllValues
            .Select(o =>
            {
                // normalize data
                var data = NormalizedSamples[o.Data.Data];
                var dataZMinus = NormalizedSamples[o.Wall.Data];

                return ChooseZWallTriangles(data, dataZMinus);
            }).ToArray();

        private readonly Vector3i _cellInGroupCount;
        private readonly IImageSampler3 _imageSampler;

        public TerrainMesherWithWalls3b(ITerrainConfig terrainConfig, IImageSampler3 imageSampler)
        {
            _imageSampler = imageSampler;
            _cellInGroupCount = terrainConfig.CellInGroupCount;
        }

        protected IImage3b Image { get; private set; }

        public void AddCell(Vector3i cellInGroup)
        {
            Vector3i cell = cellInGroup + groupPosition;

            SampledData3b data = _imageSampler.Sample(Image, cell);
            SampledData3b dataXMinus = _imageSampler.Sample(Image, cell + new Vector3i(-1, 0, 0));
            SampledData3b dataYMinus = _imageSampler.Sample(Image, cell + new Vector3i(0, -1, 0));
            SampledData3b dataZMinus = _imageSampler.Sample(Image, cell + new Vector3i(0, 0, -1));

            foreach (var tri in MainPlaneTriangles[data.Data])
            {
                mesh.AddTriangle(cell + tri.A, cell + tri.B, cell + tri.C);
            }
            foreach (var tri in XWallTriangles[SampledDataWithWall.GetIndexInAllValues(data, dataXMinus)])
            {
                mesh.AddTriangle(cell + tri.A, cell + tri.B, cell + tri.C);
            }
            foreach (var tri in YWallTriangles[SampledDataWithWall.GetIndexInAllValues(data, dataYMinus)])
            {
                mesh.AddTriangle(cell + tri.A, cell + tri.B, cell + tri.C);
            }
            foreach (var tri in ZWallTriangles[SampledDataWithWall.GetIndexInAllValues(data, dataZMinus)])
            {
                mesh.AddTriangle(cell + tri.A, cell + tri.B, cell + tri.C);
            }
        }

        public IPooledTerrainMesh GetResultingMesh()
        {
            return pooledMesh;
        }

        public void Initialize(IImage3b image)
        {
            Image = image;
        }

        public void InitializeGroup(Vector3i group)
        {
            InitializeGroup(group, PooledTerrainMeshContainer<ExpandingTerrainMesh>.CreateDirty());
        }

        public void InitializeGroup(Vector3i group, IPooledTerrainMesh cleanPooledMesh)
        {
            var bounds = Range3i.FromMinAndSize(group * _cellInGroupCount, _cellInGroupCount).ToRange3f();

            this.groupPosition = _cellInGroupCount * group;

            this.pooledMesh = cleanPooledMesh;
            this.mesh = this.pooledMesh.Mesh;
            mesh.Clear(bounds);
        }

        private static Tuple<SampledData3b, IReadOnlyCollection<Triangle3f>> ChooseTrianglesForCell(SampledData3b data)
        {
            var pos_x0y0z0 = new Vector3f(0, 0, 0);
            var pos_x0y0z1 = new Vector3f(0, 0, 1);
            var pos_x0y1z0 = new Vector3f(0, 1, 0);
            var pos_x0y1z1 = new Vector3f(0, 1, 1);
            var pos_x1y0z0 = new Vector3f(1, 0, 0);
            var pos_x1y0z1 = new Vector3f(1, 0, 1);
            var pos_x1y1z0 = new Vector3f(1, 1, 0);
            var pos_x1y1z1 = new Vector3f(1, 1, 1);

            //TODO slopes might be usefull to also do only subset, since there can be some small outlier like:
            //   1-----1
            //  /|    /|
            // 1-+---1 |
            // | 0---+-1
            // |/    |/
            // 1-----0

            SampledData3b rotatedTemplate;

            Matrix4x4f matrix;
            if (data.Data == 0 || data.Data == byte.MaxValue)
            {
                return Tuple.Create(data, Array.Empty<Triangle3f>() as IReadOnlyCollection<Triangle3f>);
            }
            // else if (SampledData3b.ParseCube(@"
            //   0-----0
            //  /|    /|
            // 0-+---0 |
            // | 1---+-1
            // |/    |/
            // 1-----1
            // ").EqualsRotationInvariant(data, out matrix, false, false, true, false)) // plane
            // {
            //     matrix = matrix.Inverse;
            //     var isInverted = matrix.Determinant < 0;

            //     return Tuple.Create(data, TerrainMeshExtensions
            //         .GetQuadTriangles(
            //             matrix.MultiplyPoint(pos_x0y0z0),
            //             matrix.MultiplyPoint(pos_x0y1z0),
            //             matrix.MultiplyPoint(pos_x1y0z0),
            //             matrix.MultiplyPoint(pos_x1y1z0), false)
            //          .ChangeOrderIfTrue(isInverted)
            //          .ToArray() as IReadOnlyCollection<Triangle3f>);
            // }
            else if (SampledData3b.ParseCube(@"
              1-----0
             /|    /|
            1-+---0 |
            | 1---+-1
            |/    |/
            1-----1
            ").EqualsRotationInvariant(data, out matrix, false, false, true, false)) //simple slope
            {
                matrix = matrix.Inverse;
                var isInverted = matrix.Determinant < 0;
                return Tuple.Create(data, TerrainMeshExtensions
                    .GetQuadTriangles(
                        matrix.MultiplyPoint(pos_x0y0z1),
                        matrix.MultiplyPoint(pos_x0y1z1),
                        matrix.MultiplyPoint(pos_x1y0z0),
                        matrix.MultiplyPoint(pos_x1y1z0), false)
                    .ChangeOrderIfTrue(isInverted)
                    .ToArray() as IReadOnlyCollection<Triangle3f>);
            }
            else if (SampledData3b.ParseCube(@"
              1-----0
             /|    /|
            1-+---1 |
            | 1---+-0
            |/    |/
            1-----1
            ").EqualsRotationInvariant(data, out matrix, false, false, true, false)) //diagonal plane
            {
                matrix = matrix.Inverse;
                var isInverted = matrix.Determinant < 0;
                return Tuple.Create(data, new Triangle3f[]{
                    new Triangle3f(
                        matrix.MultiplyPoint(pos_x1y0z0),
                        matrix.MultiplyPoint(pos_x0y1z0),
                        matrix.MultiplyPoint(pos_x0y1z1)),
                    new Triangle3f(
                        matrix.MultiplyPoint(pos_x1y0z1),
                        matrix.MultiplyPoint(pos_x1y0z0),
                        matrix.MultiplyPoint(pos_x0y1z1)),
                   }
                    .ChangeOrderIfTrue(isInverted)
                    .ToArray() as IReadOnlyCollection<Triangle3f>);
            }
            else if (SampledData3b.ParseCube(@"
              0-----1
             /|    /|
            1-+---0 |
            | 1---+-1
            |/    |/
            1-----1
            ").EqualsRotationInvariant(data, out matrix, false, false, true, false)) //dual diagonal slope
            {
                matrix = matrix.Inverse;
                var isInverted = matrix.Determinant < 0;

                return Tuple.Create(data, new Triangle3f[]{
                    new Triangle3f(
                        matrix.MultiplyPoint(pos_x0y0z1),
                        matrix.MultiplyPoint(pos_x1y0z0),
                        matrix.MultiplyPoint(pos_x1y1z1)),
                    new Triangle3f(
                        matrix.MultiplyPoint(pos_x0y0z1),
                        matrix.MultiplyPoint(pos_x1y1z1),
                        matrix.MultiplyPoint(pos_x0y1z0)),
                   }
                   .ChangeOrderIfTrue(isInverted)
                   .ToArray() as IReadOnlyCollection<Triangle3f>);
            }
            else if (SampledData3b.ParseCube(@"
              1-----0
             /|    /|
            1-+---1 |
            | 1---+-1
            |/    |/
            1-----1
            ").IsContainedInRotationInvariant(data, out matrix, out rotatedTemplate, false, false, true, false)) // diagonal slope upper
            {
                matrix = matrix.Inverse;
                var isInverted = matrix.Determinant < 0;

                return Tuple.Create(rotatedTemplate, new Triangle3f[]{
                    new Triangle3f(
                        matrix.MultiplyPoint(pos_x0y1z1),
                        matrix.MultiplyPoint(pos_x1y0z1),
                        matrix.MultiplyPoint(pos_x1y1z0))
                   }
                   .ChangeOrderIfTrue(isInverted)
                   .ToArray() as IReadOnlyCollection<Triangle3f>);
            }
            // else if (SampledData3b.ParseCube(@"
            //   0-----0
            //  /|    /|
            // 1-+---0 |
            // | 1---+-1
            // |/    |/
            // 1-----1
            // ").IsContainedInRotationInvariant(data, out matrix, out rotatedTemplate, false, false, true, false)) // diagonal slope, with bottom
            // {
            //     matrix = matrix.Inverse;
            //     var isInverted = matrix.Determinant < 0;

            //     return Tuple.Create(rotatedTemplate, new Triangle3f[]{
            //         new Triangle3f(
            //             matrix.MultiplyPoint(pos_x1y0z0),
            //             matrix.MultiplyPoint(pos_x0y1z0),
            //             matrix.MultiplyPoint(pos_x0y0z1)),
            //         new Triangle3f(
            //             matrix.MultiplyPoint(pos_x1y0z0),
            //             matrix.MultiplyPoint(pos_x1y1z0),
            //             matrix.MultiplyPoint(pos_x0y1z0))
            //        }.ChangeOrderIfTrue(isInverted)
            //        .ToArray() as IReadOnlyCollection<Triangle3f>);
            // }
            else if (SampledData3b.ParseCube(@"
              0-----0
             /|    /|
            1-+---0 |
            | 1---+-0
            |/    |/
            1-----1
            ").IsContainedInRotationInvariant(data, out matrix, out rotatedTemplate, false, false, true, false)) // diagonal slope, no bottom
            {
                matrix = matrix.Inverse;
                var isInverted = matrix.Determinant < 0;

                return Tuple.Create(rotatedTemplate, new Triangle3f(
                        matrix.MultiplyPoint(pos_x1y0z0),
                        matrix.MultiplyPoint(pos_x0y1z0),
                        matrix.MultiplyPoint(pos_x0y0z1))
                   .AsEnumerable()
                   .ChangeOrderIfTrue(isInverted)
                   .ToArray() as IReadOnlyCollection<Triangle3f>);
            }
            // else if (SampledData3b.ParseCube(@"
            //   0-----0
            //  /|    /|
            // 0-+---0 |
            // | 1---+-0
            // |/    |/
            // 1-----1
            // ").IsContainedInRotationInvariant(data, out matrix, out rotatedTemplate, false, false, true, false)) // partial plane
            // {
            //     matrix = matrix.Inverse;
            //     var isInverted = matrix.Determinant < 0;

            //     return Tuple.Create(rotatedTemplate, new Triangle3f(
            //             matrix.MultiplyPoint(pos_x0y0z0),

            //             /* Unmerged change from project 'Zenject.Editor'
            //             Before:
            //                                     matrix.MultiplyPoint(pos_x0y0z0),
            //                                     matrix.MultiplyPoint(pos_x1y0z0),
            //                                     matrix.MultiplyPoint(pos_x0y1z0))
            //             After:
            //                                     matrix.MultiplyPoint(pos_x1y0z0),
            //                                     matrix.MultiplyPoint(pos_x0y1z0))
            //             */
            //                        }
            else
            {
                return Tuple.Create(new SampledData3b(), Array.Empty<Triangle3f>() as IReadOnlyCollection<Triangle3f>);
            }
        }

        private static IReadOnlyCollection<Triangle3f> ChooseXWallTriangles(SampledData3b data, SampledData3b dataXMinus)
        {
            var triangles = new List<Triangle3f>();

            bool centerXMinus = ((dataXMinus.Data_x1y0z0 ? 1 : 0)
                               + (dataXMinus.Data_x1y0z1 ? 1 : 0)
                               + (dataXMinus.Data_x1y1z0 ? 1 : 0)
                               + (dataXMinus.Data_x1y1z1 ? 1 : 0)) > 2;
            bool centerX = ((data.Data_x0y0z0 ? 1 : 0)
                          + (data.Data_x0y0z1 ? 1 : 0)
                          + (data.Data_x0y1z0 ? 1 : 0)
                          + (data.Data_x0y1z1 ? 1 : 0)) > 2;

            bool isBottomTri = data.Data_x0y0z0 && data.Data_x0y1z0 && centerX;
            bool isBottomTriMinus = dataXMinus.Data_x1y0z0 && dataXMinus.Data_x1y1z0 && centerXMinus;
            if (isBottomTri != isBottomTriMinus)
            {
                triangles.AddTriangle(new Vector3f(0, 0, 0), new Vector3f(0, 1, 0), new Vector3f(0, 0.5f, 0.5f), isBottomTri);
            }

            bool isTopTri = data.Data_x0y0z1 && data.Data_x0y1z1 && centerX;
            bool isTopTriMinus = dataXMinus.Data_x1y0z1 && dataXMinus.Data_x1y1z1 && centerXMinus;
            if (isTopTri != isTopTriMinus)
            {
                triangles.AddTriangle(new Vector3f(0, 1, 1), new Vector3f(0, 0, 1), new Vector3f(0, 0.5f, 0.5f), isTopTri);
            }

            bool isLeftTri = data.Data_x0y0z0 && data.Data_x0y0z1 && centerX;
            bool isLeftTriMinus = dataXMinus.Data_x1y0z0 && dataXMinus.Data_x1y0z1 && centerXMinus;
            if (isLeftTri != isLeftTriMinus)
            {
                triangles.AddTriangle(new Vector3f(0, 0, 1), new Vector3f(0, 0, 0), new Vector3f(0, 0.5f, 0.5f), isLeftTri);
            }

            bool isRightTri = data.Data_x0y1z0 && data.Data_x0y1z1 && centerX;
            bool isRightTriMinus = dataXMinus.Data_x1y1z0 && dataXMinus.Data_x1y1z1 && centerXMinus;
            if (isRightTri != isRightTriMinus)
            {
                triangles.AddTriangle(new Vector3f(0, 1, 0), new Vector3f(0, 1, 1), new Vector3f(0, 0.5f, 0.5f), isRightTri);
            }

            return triangles;
        }

        private static IReadOnlyCollection<Triangle3f> ChooseYWallTriangles(SampledData3b data, SampledData3b dataYMinus)
        {
            var triangles = new List<Triangle3f>();

            bool centerYMinus = ((dataYMinus.Data_x0y1z0 ? 1 : 0)
                               + (dataYMinus.Data_x0y1z1 ? 1 : 0)
                               + (dataYMinus.Data_x1y1z0 ? 1 : 0)
                               + (dataYMinus.Data_x1y1z1 ? 1 : 0)) > 2;
            bool centerY = ((data.Data_x0y0z0 ? 1 : 0)
                          + (data.Data_x0y0z1 ? 1 : 0)
                          + (data.Data_x1y0z0 ? 1 : 0)
                          + (data.Data_x1y0z1 ? 1 : 0)) > 2;

            bool isBottomTri = data.Data_x0y0z0 && data.Data_x1y0z0 && centerY;
            bool isBottomTriMinus = dataYMinus.Data_x0y1z0 && dataYMinus.Data_x1y1z0 && centerYMinus;
            if (isBottomTri != isBottomTriMinus)
            {
                triangles.AddTriangle(new Vector3f(0, 0, 0), new Vector3f(1, 0, 0), new Vector3f(0.5f, 0, 0.5f), isBottomTriMinus);
            }

            bool isTopTri = data.Data_x0y0z1 && data.Data_x1y0z1 && centerY;
            bool isTopTriMinus = dataYMinus.Data_x0y1z1 && dataYMinus.Data_x1y1z1 && centerYMinus;
            if (isTopTri != isTopTriMinus)
            {
                triangles.AddTriangle(new Vector3f(1, 0, 1), new Vector3f(0, 0, 1), new Vector3f(0.5f, 0, 0.5f), isTopTriMinus);
            }

            bool isLeftTri = data.Data_x0y0z0 && data.Data_x0y0z1 && centerY;
            bool isLeftTriMinus = dataYMinus.Data_x0y1z0 && dataYMinus.Data_x0y1z1 && centerYMinus;
            if (isLeftTri != isLeftTriMinus)
            {
                triangles.AddTriangle(new Vector3f(0, 0, 1), new Vector3f(0, 0, 0), new Vector3f(0.5f, 0, 0.5f), isLeftTriMinus);
            }
            bool isRightTri = data.Data_x1y0z0 && data.Data_x1y0z1 && centerY;
            bool isRightTriMinus = dataYMinus.Data_x1y1z0 && dataYMinus.Data_x1y1z1 && centerYMinus;
            if (isRightTri != isRightTriMinus)
            {
                triangles.AddTriangle(new Vector3f(1, 0, 0), new Vector3f(1, 0, 1), new Vector3f(0.5f, 0, 0.5f), isRightTriMinus);
            }

            return triangles;
        }

        private static IReadOnlyCollection<Triangle3f> ChooseZWallTriangles(SampledData3b data, SampledData3b dataZMinus)
        {
            var triangles = new List<Triangle3f>();

            bool centerZMinus = ((dataZMinus.Data_x0y0z1 ? 1 : 0)
                               + (dataZMinus.Data_x0y1z1 ? 1 : 0)
                               + (dataZMinus.Data_x1y0z1 ? 1 : 0)
                               + (dataZMinus.Data_x1y1z1 ? 1 : 0)) > 2;
            bool centerZ = ((data.Data_x0y0z0 ? 1 : 0)
                          + (data.Data_x0y1z0 ? 1 : 0)
                          + (data.Data_x1y0z0 ? 1 : 0)
                          + (data.Data_x1y1z0 ? 1 : 0)) > 2;

            bool isBottomTri = data.Data_x0y0z0 && data.Data_x1y0z0 && centerZ;
            bool isBottomTriMinus = dataZMinus.Data_x0y0z1 && dataZMinus.Data_x1y0z1 && centerZMinus;
            if (isBottomTri != isBottomTriMinus)
            {
                triangles.AddTriangle(new Vector3f(0, 0, 0), new Vector3f(1, 0, 0), new Vector3f(0.5f, 0.5f, 0), isBottomTri);
            }

            bool isTopTri = data.Data_x0y1z0 && data.Data_x1y1z0 && centerZ;
            bool isTopTriMinus = dataZMinus.Data_x0y1z1 && dataZMinus.Data_x1y1z1 && centerZMinus;
            if (isTopTri != isTopTriMinus)
            {
                triangles.AddTriangle(new Vector3f(1, 1, 0), new Vector3f(0, 1, 0), new Vector3f(0.5f, 0.5f, 0), isTopTri);
            }

            bool isLeftTri = data.Data_x0y0z0 && data.Data_x0y1z0 && centerZ;
            bool isLeftTriMinus = dataZMinus.Data_x0y0z1 && dataZMinus.Data_x0y1z1 && centerZMinus;
            if (isLeftTri != isLeftTriMinus)
            {
                triangles.AddTriangle(new Vector3f(0, 1, 0), new Vector3f(0, 0, 0), new Vector3f(0.5f, 0.5f, 0), isLeftTri);
            }
            bool isRightTri = data.Data_x1y0z0 && data.Data_x1y1z0 && centerZ;
            bool isRightTriMinus = dataZMinus.Data_x1y0z1 && dataZMinus.Data_x1y1z1 && centerZMinus;
            if (isRightTri != isRightTriMinus)
            {
                triangles.AddTriangle(new Vector3f(1, 0, 0), new Vector3f(1, 1, 0), new Vector3f(0.5f, 0.5f, 0), isRightTri);
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

            public static int GetIndexInAllValues(SampledData3b data, SampledData3b wall)
            {
                return (data.Data << 8) | wall.Data;
            }
        }
    }
}