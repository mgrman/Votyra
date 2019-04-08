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
    public class TerrainMesher3b : ITerrainMesher3b
    {
        public static readonly Vector3f CenterZeroCell = new Vector3f(0.5f, 0.5f, 0.5f);
        private static readonly List<SampledData3b> DataWithoutTriangles = new List<SampledData3b>();

        private static readonly IReadOnlyDictionary<SampledData3b, IReadOnlyCollection<Triangle3f>> DataToTriangles = SampledData3b.AllValues.ToDictionary(o => o, o => ChooseTrianglesForCell(o), SampledData3b.NormallessComparer);

        private readonly Vector3i _cellInGroupCount;

        private readonly IImageSampler3 _imageSampler;
        protected Vector3i groupPosition;
        protected Vector3i groupSize;
        protected IGeneralMesh pooledMesh;

        public TerrainMesher3b(ITerrainConfig terrainConfig, IImageSampler3 imageSampler)
        {
            _imageSampler = imageSampler;
            _cellInGroupCount = terrainConfig.CellInGroupCount;
        }

        protected IImage3b Image { get; private set; }

        public void Initialize(IImage3b image)
        {
            Image = image;
        }

        public void AddCell(Vector3i cellInGroup)
        {
            var cell = cellInGroup + groupPosition;

            var data = _imageSampler.Sample(Image, cell);

            var finalTris = DataToTriangles[data];

            foreach (var tri in finalTris)
            {
                pooledMesh.AddTriangle(cell + tri.A, cell + tri.B, cell + tri.C);
            }

            // TODO find a way to not generate thin planes
            // probablby has to check the other side if there is something?
            // or maybe postProcessing?
        }

        public void InitializeGroup(Vector3i group, IGeneralMesh cleanPooledMesh)
        {
            var bounds = Range3i.FromMinAndSize(group * _cellInGroupCount, _cellInGroupCount)
                .ToArea3f();

            groupPosition = _cellInGroupCount * group;

            pooledMesh = cleanPooledMesh;
            pooledMesh.Reset(Area3f.FromMinAndSize((group * _cellInGroupCount).ToVector3f(), _cellInGroupCount.ToVector3f()));
        }

        public IGeneralMesh GetResultingMesh() => pooledMesh;

        private static IReadOnlyCollection<Triangle3f> ChooseTrianglesForCell(SampledData3b data)
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

            Matrix4x4f matrix;
            if (data.Data == 0 || data.Data == byte.MaxValue)
                return Array.Empty<Triangle3f>();

            if (SampledData3b.ParseCube(@"
              0-----0
             /|    /|
            0-+---0 |
            | 1---+-1
            |/    |/
            1-----1
            ")
                .EqualsRotationInvariant(data, out matrix))
            {
                matrix = matrix.Inverse;
                var isInverted = matrix.Determinant < 0;

                return TerrainMeshUtilities.GetQuadTriangles(matrix.MultiplyPoint(pos_x0y0z0), matrix.MultiplyPoint(pos_x0y1z0), matrix.MultiplyPoint(pos_x1y0z0), matrix.MultiplyPoint(pos_x1y1z0), false)
                    .ChangeOrderIfTrue(isInverted)
                    .ToArray();
            }

            if (SampledData3b.ParseCube(@"
              1-----0
             /|    /|
            1-+---0 |
            | 1---+-1
            |/    |/
            1-----1
            ")
                .EqualsRotationInvariant(data, out matrix))
            {
                matrix = matrix.Inverse;
                var isInverted = matrix.Determinant < 0;
                return TerrainMeshUtilities.GetQuadTriangles(matrix.MultiplyPoint(pos_x0y0z1), matrix.MultiplyPoint(pos_x0y1z1), matrix.MultiplyPoint(pos_x1y0z0), matrix.MultiplyPoint(pos_x1y1z0), false)
                    .ChangeOrderIfTrue(isInverted)
                    .ToArray();
            }

            if (SampledData3b.ParseCube(@"
              0-----0
             /|    /|
            1-+---0 |
            | 1---+-0
            |/    |/
            1-----1
            ")
                .EqualsRotationInvariant(data, out matrix))
            {
                matrix = matrix.Inverse;
                var isInverted = matrix.Determinant < 0;

                return new Triangle3f(matrix.MultiplyPoint(pos_x1y0z0), matrix.MultiplyPoint(pos_x0y1z0), matrix.MultiplyPoint(pos_x0y0z1)).AsEnumerable()
                    .ChangeOrderIfTrue(isInverted)
                    .ToArray();
            }

            if (SampledData3b.ParseCube(@"
              0-----0
             /|    /|
            1-+---0 |
            | 1---+-1
            |/    |/
            1-----1
            ")
                .EqualsRotationInvariant(data, out matrix))
            {
                matrix = matrix.Inverse;
                var isInverted = matrix.Determinant < 0;

                return new[] {new Triangle3f(matrix.MultiplyPoint(pos_x1y0z0), matrix.MultiplyPoint(pos_x0y1z0), matrix.MultiplyPoint(pos_x0y0z1)), new Triangle3f(matrix.MultiplyPoint(pos_x1y0z0), matrix.MultiplyPoint(pos_x1y1z0), matrix.MultiplyPoint(pos_x0y1z0))}.ChangeOrderIfTrue(isInverted)
                    .ToArray();
            }

            if (SampledData3b.ParseCube(@"
              1-----0
             /|    /|
            1-+---1 |
            | 1---+-1
            |/    |/
            1-----1
            ")
                .EqualsRotationInvariant(data, out matrix))
            {
                matrix = matrix.Inverse;
                var isInverted = matrix.Determinant < 0;

                return new[] {new Triangle3f(matrix.MultiplyPoint(pos_x0y1z1), matrix.MultiplyPoint(pos_x1y0z1), matrix.MultiplyPoint(pos_x1y1z0))}.ChangeOrderIfTrue(isInverted)
                    .ToArray();
            }

            if (SampledData3b.ParseCube(@"
              0-----1
             /|    /|
            1-+---0 |
            | 1---+-1
            |/    |/
            1-----1
            ")
                .EqualsRotationInvariant(data, out matrix))
            {
                matrix = matrix.Inverse;
                var isInverted = matrix.Determinant < 0;

                return new[] {new Triangle3f(matrix.MultiplyPoint(pos_x0y0z1), matrix.MultiplyPoint(pos_x1y0z0), matrix.MultiplyPoint(pos_x1y1z1)), new Triangle3f(matrix.MultiplyPoint(pos_x0y0z1), matrix.MultiplyPoint(pos_x1y1z1), matrix.MultiplyPoint(pos_x0y1z0))}.ChangeOrderIfTrue(isInverted)
                    .ToArray();
            }

            if (SampledData3b.ParseCube(@"
              0-----0
             /|    /|
            1-+---0 |
            | 1---+-1
            |/    |/
            1-----0
            ")
                .EqualsRotationInvariant(data, out matrix))
            {
                matrix = matrix.Inverse;
                var isInverted = matrix.Determinant < 0;
                return new[] {new Triangle3f(matrix.MultiplyPoint(pos_x0y0z0), matrix.MultiplyPoint(pos_x1y1z0), matrix.MultiplyPoint(pos_x0y0z1)), new Triangle3f(matrix.MultiplyPoint(pos_x1y1z0), matrix.MultiplyPoint(pos_x0y1z0), matrix.MultiplyPoint(pos_x0y0z1))}.ChangeOrderIfTrue(isInverted)
                    .ToArray();
            }

            if (SampledData3b.ParseCube(@"
              0-----1
             /|    /|
            1-+---0 |
            | 1---+-1
            |/    |/
            1-----0
            ")
                .EqualsRotationInvariant(data, out matrix))
            {
                matrix = matrix.Inverse;
                var isInverted = matrix.Determinant < 0;

                return new[] {new Triangle3f(matrix.MultiplyPoint(pos_x0y1z0), matrix.MultiplyPoint(pos_x0y0z1), matrix.MultiplyPoint(pos_x1y1z1)), new Triangle3f(matrix.MultiplyPoint(pos_x0y0z0), matrix.MultiplyPoint(pos_x1y1z0), matrix.MultiplyPoint(pos_x1y1z1)), new Triangle3f(matrix.MultiplyPoint(pos_x0y0z0), matrix.MultiplyPoint(pos_x1y1z1), matrix.MultiplyPoint(pos_x0y0z1))}.ChangeOrderIfTrue(isInverted)
                    .ToArray();
            }

            var template = SampledData3b.ParseCube(@"
                      0-----0
                     /|    /|
                    0-+---0 |
                    | 1---+-0
                    |/    |/
                    1-----1
                    ");
            var triangles = template.GetAllRotationSubsets(data)
                .Select(m =>
                {
                    var mInv = m.Inverse;
                    return new Triangle3f(mInv.MultiplyPoint(pos_x0y0z0), mInv.MultiplyPoint(pos_x0y1z0), mInv.MultiplyPoint(pos_x1y0z0));
                })
                .Distinct(Triangle3f.OrderInvariantComparer)
                .Select(t => t.EnsureCCW(CenterZeroCell))
                .ToArray();

            if (triangles.Length == 0)
                DataWithoutTriangles.Add(data);
            return triangles.ToArray();
        }
    }
}