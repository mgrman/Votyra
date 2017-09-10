using Votyra.Models;
using Votyra.Utils;
using Votyra.TerrainGenerators.TerrainMeshers.TerrainMeshes;
using UnityEngine;
using Votyra.Unity.Assets.Votyra.Pooling;
using System.Linq;
using System.Collections.Generic;
using System;
using Votyra.ImageSamplers;
using Votyra.Images;

namespace Votyra.TerrainGenerators.TerrainMeshers
{
    public class TerrainMesher3b : ITerrainMesher3b
    {
        protected IImageSampler3b ImageSampler { get; private set; }
        protected IImage3f Image { get; private set; }
        public Vector3i CellInGroupCount { get; private set; }
        protected Vector3i groupPosition;
        protected Vector3i groupSize;
        protected IPooledTerrainMesh pooledMesh;
        protected ITerrainMesh mesh;

        public void Initialize(ITerrainGeneratorContext3b terrainOptions)
        {
            this.Initialize(terrainOptions.ImageSampler, terrainOptions.Image, terrainOptions.CellInGroupCount);
        }

        public void Initialize(IImageSampler3b imageSampler, IImage3f image, Vector3i cellInGroupCount)
        {
            ImageSampler = imageSampler;
            Image = image;
            this.CellInGroupCount = cellInGroupCount;


            // foreach (var data in DataWithoutTriangles.Distinct(SampledData3b.RotationInvariantComparer))
            // {
            //     Debug.LogError($"Not found any triangles for {data.ToCubeString()}");
            // }
        }

        public void InitializeGroup(Vector3i group)
        {
            InitializeGroup(group, PooledTerrainMeshContainer<ExpandingTerrainMesh>.CreateDirty());
        }
        public void InitializeGroup(Vector3i group, IPooledTerrainMesh cleanPooledMesh)
        {
            var bounds = new Rect3i(group * CellInGroupCount, CellInGroupCount).ToBounds();

            this.groupPosition = CellInGroupCount * group;

            this.pooledMesh = cleanPooledMesh;
            this.mesh = this.pooledMesh.Mesh;
            mesh.Clear(bounds);
        }

        public static readonly Vector3 CenterZeroCell = new Vector3(0.5f, 0.5f, 0.5f);

        public void AddCell(Vector3i cellInGroup)
        {
            Vector3i cell = cellInGroup + groupPosition;

            SampledData3b data = ImageSampler.Sample(Image, cell);

            var possibleTris = DataToTriangles[data];

            var finalTris = possibleTris.Count <= 1 ? possibleTris.FirstOrDefault() : possibleTris.MinByOrDefault(o => Vector3.Angle(data.Normal, o.Item1.Normal));

            // if (possibleTris.Count > 1)
            if (finalTris.Item2.Count > 0)
            {
                AddDeriv(cell.ToVector3() + Vector3Utils.Half, data.Normal * 2);
                //AddDeriv(cell.ToVector3() + Vector3Utils.Half, finalTris.Item1.Normal);
            }

            foreach (var tri in finalTris.Item2)
            {
                mesh.AddTriangle(cell + tri.a, cell + tri.b, cell + tri.c);
            }

            //TODO missing fill holes functionality
            //TODO also do choice by normal for angled slope, so if they are next to each other they fit
        }
        private static readonly List<SampledData3b> DataWithoutTriangles = new List<SampledData3b>();

        private static readonly IReadOnlyDictionary<SampledData3b, IReadOnlyCollection<Tuple<SampledData3b, IReadOnlyCollection<Triangle3>>>> DataToTriangles = SampledData3b.AllValues
            .ToDictionary(o => o, o => ChooseTrianglesForCell(o).ToArray() as IReadOnlyCollection<Tuple<SampledData3b, IReadOnlyCollection<Triangle3>>>, SampledData3b.NormallessComparer);


        private static IEnumerable<Tuple<SampledData3b, IReadOnlyCollection<Triangle3>>> ChooseTrianglesForCell(SampledData3b data)
        {
            var pos_x0y0z0 = new Vector3(0, 0, 0);
            var pos_x0y0z1 = new Vector3(0, 0, 1);
            var pos_x0y1z0 = new Vector3(0, 1, 0);
            var pos_x0y1z1 = new Vector3(0, 1, 1);
            var pos_x1y0z0 = new Vector3(1, 0, 0);
            var pos_x1y0z1 = new Vector3(1, 0, 1);
            var pos_x1y1z0 = new Vector3(1, 1, 0);
            var pos_x1y1z1 = new Vector3(1, 1, 1);

            //TODO slopes might be usefull to also do only subset, since there can be some small outlier like:
            //   1-----1
            //  /|    /|
            // 1-+---1 |
            // | 0---+-1
            // |/    |/
            // 1-----0

            Matrix4x4 matrix;
            if (data.Data == 0 || data.Data == byte.MaxValue)
            {
                yield return Tuple.Create<SampledData3b, IReadOnlyCollection<Triangle3>>(data, Array.Empty<Triangle3>());
            }
            else if (SampledData3b.ParseCube(@"
              0-----0
             /|    /|
            0-+---0 |
            | 1---+-1
            |/    |/
            1-----1
            ").EqualsRotationInvariant(data, out matrix))
            {
                matrix = matrix.inverse;
                var isInverted = matrix.determinant < 0;

                yield return Tuple.Create<SampledData3b, IReadOnlyCollection<Triangle3>>(data, TerrainMeshExtensions.GetQuadTriangles(
                     matrix.MultiplyPoint(pos_x0y0z0),
                     matrix.MultiplyPoint(pos_x0y1z0),
                     matrix.MultiplyPoint(pos_x1y0z0),
                     matrix.MultiplyPoint(pos_x1y1z0), false)
                     .ChangeOrderIfTrue(isInverted).ToArray());
            }
            else if (SampledData3b.ParseCube(@"
              1-----0
             /|    /|
            1-+---0 |
            | 1---+-1
            |/    |/
            1-----1
            ").EqualsRotationInvariant(data, out matrix))
            {
                matrix = matrix.inverse;
                var isInverted = matrix.determinant < 0;

                yield return Tuple.Create<SampledData3b, IReadOnlyCollection<Triangle3>>(new SampledData3b(data, matrix.MultiplyVector(new Vector3(1, 0, 1))), TerrainMeshExtensions.GetQuadTriangles(
                   matrix.MultiplyPoint(pos_x0y0z1),
                   matrix.MultiplyPoint(pos_x0y1z1),
                   matrix.MultiplyPoint(pos_x1y0z0),
                   matrix.MultiplyPoint(pos_x1y1z0), false)
                   .ChangeOrderIfTrue(isInverted).ToArray());


                yield return Tuple.Create<SampledData3b, IReadOnlyCollection<Triangle3>>(new SampledData3b(data, matrix.MultiplyVector(new Vector3(1, 0, 0))), TerrainMeshExtensions.GetQuadTriangles(
                   matrix.MultiplyPoint(pos_x0y0z0),
                     matrix.MultiplyPoint(pos_x0y1z0),
                     matrix.MultiplyPoint(pos_x1y0z0),
                     matrix.MultiplyPoint(pos_x1y1z0), true)
                   .Concat(TerrainMeshExtensions.GetQuadTriangles(
                   matrix.MultiplyPoint(pos_x0y0z0),
                   matrix.MultiplyPoint(pos_x0y0z1),
                   matrix.MultiplyPoint(pos_x0y1z0),
                   matrix.MultiplyPoint(pos_x0y1z1), true))
                   .ChangeOrderIfTrue(isInverted).ToArray());
                yield return Tuple.Create<SampledData3b, IReadOnlyCollection<Triangle3>>(new SampledData3b(data, matrix.MultiplyVector(new Vector3(0, 0, 1))), TerrainMeshExtensions.GetQuadTriangles(
                   matrix.MultiplyPoint(pos_x0y0z0),
                     matrix.MultiplyPoint(pos_x0y1z0),
                     matrix.MultiplyPoint(pos_x1y0z0),
                     matrix.MultiplyPoint(pos_x1y1z0), true)
                   .Concat(TerrainMeshExtensions.GetQuadTriangles(
                   matrix.MultiplyPoint(pos_x0y0z0),
                   matrix.MultiplyPoint(pos_x0y0z1),
                   matrix.MultiplyPoint(pos_x0y1z0),
                   matrix.MultiplyPoint(pos_x0y1z1), true))
                   .ChangeOrderIfTrue(isInverted).ToArray());

                // yield return Tuple.Create<SampledData3b, IReadOnlyCollection<Triangle3>>(new SampledData3b(data, matrix.MultiplyVector(new Vector3(0, 0, 1))), TerrainMeshExtensions.GetQuadTriangles(
                //    matrix.MultiplyPoint(pos_x0y0z0),
                //      matrix.MultiplyPoint(pos_x0y1z0),
                //      matrix.MultiplyPoint(pos_x1y0z0),
                //      matrix.MultiplyPoint(pos_x1y1z0), true)
                //    .Concat(TerrainMeshExtensions.GetQuadTriangles(
                //    matrix.MultiplyPoint(pos_x0y0z0),
                //    matrix.MultiplyPoint(pos_x0y0z1),
                //    matrix.MultiplyPoint(pos_x0y1z0),
                //    matrix.MultiplyPoint(pos_x0y1z1), true))
                //    .ChangeOrderIfTrue(isInverted).ToArray());
            }
            else if (SampledData3b.ParseCube(@"
              0-----0
             /|    /|
            1-+---0 |
            | 1---+-0
            |/    |/
            1-----1
            ").EqualsRotationInvariant(data, out matrix))
            {
                matrix = matrix.inverse;
                var isInverted = matrix.determinant < 0;

                yield return Tuple.Create<SampledData3b, IReadOnlyCollection<Triangle3>>(data, new Triangle3(
                        matrix.MultiplyPoint(pos_x1y0z0),
                        matrix.MultiplyPoint(pos_x0y1z0),
                        matrix.MultiplyPoint(pos_x0y0z1))
                   .AsEnumerable()
                   .ChangeOrderIfTrue(isInverted).ToArray());
            }
            else if (SampledData3b.ParseCube(@"
              0-----0
             /|    /|
            1-+---0 |
            | 1---+-1
            |/    |/
            1-----1
            ").EqualsRotationInvariant(data, out matrix))
            {
                matrix = matrix.inverse;
                var isInverted = matrix.determinant < 0;

                yield return Tuple.Create<SampledData3b, IReadOnlyCollection<Triangle3>>(data, new Triangle3[]{
                    new Triangle3(
                        matrix.MultiplyPoint(pos_x1y0z0),
                        matrix.MultiplyPoint(pos_x0y1z0),
                        matrix.MultiplyPoint(pos_x0y0z1)),
                    new Triangle3(
                        matrix.MultiplyPoint(pos_x1y0z0),
                        matrix.MultiplyPoint(pos_x1y1z0),
                        matrix.MultiplyPoint(pos_x0y1z0))
                   }.ChangeOrderIfTrue(isInverted).ToArray());
            }
            else if (SampledData3b.ParseCube(@"
              1-----0
             /|    /|
            1-+---1 |
            | 1---+-1
            |/    |/
            1-----1
            ").EqualsRotationInvariant(data, out matrix))
            {
                matrix = matrix.inverse;
                var isInverted = matrix.determinant < 0;

                yield return Tuple.Create<SampledData3b, IReadOnlyCollection<Triangle3>>(data, new Triangle3[]{
                    new Triangle3(
                        matrix.MultiplyPoint(pos_x0y1z1),
                        matrix.MultiplyPoint(pos_x1y0z1),
                        matrix.MultiplyPoint(pos_x1y1z0))
                   }.ChangeOrderIfTrue(isInverted).ToArray());
            }
            else if (SampledData3b.ParseCube(@"
              0-----1
             /|    /|
            1-+---0 |
            | 1---+-1
            |/    |/
            1-----1
            ").EqualsRotationInvariant(data, out matrix))
            {
                matrix = matrix.inverse;
                var isInverted = matrix.determinant < 0;

                yield return Tuple.Create<SampledData3b, IReadOnlyCollection<Triangle3>>(data, new Triangle3[]{
                    new Triangle3(
                        matrix.MultiplyPoint(pos_x0y0z1),
                        matrix.MultiplyPoint(pos_x1y0z0),
                        matrix.MultiplyPoint(pos_x1y1z1)),
                    new Triangle3(
                        matrix.MultiplyPoint(pos_x0y0z1),
                        matrix.MultiplyPoint(pos_x1y1z1),
                        matrix.MultiplyPoint(pos_x0y1z0)),
                   }.ChangeOrderIfTrue(isInverted).ToArray());
            }
            else if (SampledData3b.ParseCube(@"
              0-----0
             /|    /|
            1-+---0 |
            | 1---+-1
            |/    |/
            1-----0
            ").EqualsRotationInvariant(data, out matrix))
            {
                matrix = matrix.inverse;
                var isInverted = matrix.determinant < 0;
                yield return Tuple.Create<SampledData3b, IReadOnlyCollection<Triangle3>>(data, new Triangle3[]{
                    new Triangle3(
                        matrix.MultiplyPoint(pos_x0y0z0),
                        matrix.MultiplyPoint(pos_x1y1z0),
                        matrix.MultiplyPoint(pos_x0y0z1)),
                    new Triangle3(
                        matrix.MultiplyPoint(pos_x1y1z0),
                        matrix.MultiplyPoint(pos_x0y1z0),
                        matrix.MultiplyPoint(pos_x0y0z1)),
                   }.ChangeOrderIfTrue(isInverted).ToArray());
            }
            else if (SampledData3b.ParseCube(@"
              0-----1
             /|    /|
            1-+---0 |
            | 1---+-1
            |/    |/
            1-----0
            ").EqualsRotationInvariant(data, out matrix))
            {
                matrix = matrix.inverse;
                var isInverted = matrix.determinant < 0;

                yield return Tuple.Create<SampledData3b, IReadOnlyCollection<Triangle3>>(data, new Triangle3[]{
                    new Triangle3(
                        matrix.MultiplyPoint(pos_x0y1z0),
                        matrix.MultiplyPoint(pos_x0y0z1),
                        matrix.MultiplyPoint(pos_x1y1z1)),
                    new Triangle3(
                        matrix.MultiplyPoint(pos_x0y0z0),
                        matrix.MultiplyPoint(pos_x1y1z0),
                        matrix.MultiplyPoint(pos_x1y1z1)),
                    new Triangle3(
                        matrix.MultiplyPoint(pos_x0y0z0),
                        matrix.MultiplyPoint(pos_x1y1z1),
                        matrix.MultiplyPoint(pos_x0y0z1)),
                   }.ChangeOrderIfTrue(isInverted).ToArray());
            }
            else
            {
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
                        var mInv = m.inverse;
                        return new Triangle3(
                            mInv.MultiplyPoint(pos_x0y0z0),
                            mInv.MultiplyPoint(pos_x0y1z0),
                            mInv.MultiplyPoint(pos_x1y0z0));
                    })
                    .Distinct(Triangle3.OrderInvariantComparer)
                    .Select(t => t.EnsureCCW(CenterZeroCell))
                    .ToArray();

                if (triangles.Length == 0)
                {
                    DataWithoutTriangles.Add(data);
                }
                yield return Tuple.Create<SampledData3b, IReadOnlyCollection<Triangle3>>(data, triangles.ToArray());
            }
        }

        private void AddSmallDebugCube(Vector3 pos, float extents)
        {
            var pos_x0y0z0 = new Vector3(pos.x - extents, pos.y - extents, pos.z - extents);
            var pos_x0y0z1 = new Vector3(pos.x - extents, pos.y - extents, pos.z + extents);
            var pos_x0y1z0 = new Vector3(pos.x - extents, pos.y + extents, pos.z - extents);
            var pos_x0y1z1 = new Vector3(pos.x - extents, pos.y + extents, pos.z + extents);
            var pos_x1y0z0 = new Vector3(pos.x + extents, pos.y - extents, pos.z - extents);
            var pos_x1y0z1 = new Vector3(pos.x + extents, pos.y - extents, pos.z + extents);
            var pos_x1y1z0 = new Vector3(pos.x + extents, pos.y + extents, pos.z - extents);
            var pos_x1y1z1 = new Vector3(pos.x + extents, pos.y + extents, pos.z + extents);

            mesh.AddQuad(pos_x0y0z0, pos_x0y1z0, pos_x1y0z0, pos_x1y1z0, false);
            mesh.AddQuad(pos_x0y0z1, pos_x0y1z1, pos_x1y0z1, pos_x1y1z1, false);
            mesh.AddWall(pos_x0y0z1, pos_x0y1z1, pos_x0y1z0, pos_x0y0z0, false);
            mesh.AddWall(pos_x1y0z1, pos_x0y0z1, pos_x0y0z0, pos_x1y0z0, false);
            mesh.AddWall(pos_x1y0z1, pos_x1y1z1, pos_x1y1z0, pos_x1y0z0, false);
            mesh.AddWall(pos_x1y1z1, pos_x0y1z1, pos_x0y1z0, pos_x1y1z0, false);
        }

        private void AddDeriv(Vector3 pos, Vector3 normal)
        {
            if ((normal - Vector3.zero).magnitude < 0.1)
            {
                AddSmallDebugCube(pos, 0.1f);
            }
            else
            {
                mesh.AddTriangle(pos, pos + normal, new Vector3(pos.x + 0.1f, pos.y, pos.z));
                mesh.AddTriangle(pos + normal, pos, new Vector3(pos.x + 0.1f, pos.y, pos.z));
                mesh.AddTriangle(pos, pos + normal, new Vector3(pos.x, pos.y + 0.1f, pos.z));
                mesh.AddTriangle(pos + normal, pos, new Vector3(pos.x, pos.y + 0.1f, pos.z));
            }
        }

        public IPooledTerrainMesh GetResultingMesh()
        {
            return pooledMesh;
        }
    }
}