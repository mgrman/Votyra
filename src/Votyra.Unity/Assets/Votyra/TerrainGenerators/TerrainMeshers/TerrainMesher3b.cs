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
        protected IImage3b Image { get; private set; }
        public Vector3i CellInGroupCount { get; private set; }
        protected Vector3i groupPosition;
        protected Vector3i groupSize;
        protected IPooledTerrainMesh pooledMesh;
        protected ITerrainMesh mesh;

        public void Initialize(ITerrainGeneratorContext3b terrainOptions)
        {
            this.Initialize(terrainOptions.ImageSampler, terrainOptions.Image, terrainOptions.CellInGroupCount);
        }

        public void Initialize(IImageSampler3b imageSampler, IImage3b image, Vector3i cellInGroupCount)
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

            foreach (var tri in DataToTriangles[data])
            {
                mesh.AddTriangle(cell + tri.a, cell + tri.b, cell + tri.c);
            }
        }
        private static readonly List<SampledData3b> DataWithoutTriangles = new List<SampledData3b>();

        private static readonly IReadOnlyDictionary<SampledData3b, IReadOnlyCollection<Triangle3>> DataToTriangles = SampledData3b
            .GenerateAllValues()
            .ToDictionary(o => o, o => ChooseTrianglesForCell(o).ToArray() as IReadOnlyCollection<Triangle3>);


        private static IEnumerable<Triangle3> ChooseTrianglesForCell(SampledData3b data)
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
                return Enumerable.Empty<Triangle3>();
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
                return TerrainMeshExtensions.GetQuadTriangles(
                     matrix.MultiplyPoint(pos_x0y0z0),
                     matrix.MultiplyPoint(pos_x0y1z0),
                     matrix.MultiplyPoint(pos_x1y0z0),
                     matrix.MultiplyPoint(pos_x1y1z0), false)
                     .ChangeOrderIfTrue(isInverted);
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
                return TerrainMeshExtensions.GetQuadTriangles(
                   matrix.MultiplyPoint(pos_x0y0z1),
                   matrix.MultiplyPoint(pos_x0y1z1),
                   matrix.MultiplyPoint(pos_x1y0z0),
                   matrix.MultiplyPoint(pos_x1y1z0), false)
                   .ChangeOrderIfTrue(isInverted);
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
                return new Triangle3(
                        matrix.MultiplyPoint(pos_x1y0z0),
                        matrix.MultiplyPoint(pos_x0y1z0),
                        matrix.MultiplyPoint(pos_x0y0z1))
                   .AsEnumerable()
                   .ChangeOrderIfTrue(isInverted);
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
                return new Triangle3[]{
                    new Triangle3(
                        matrix.MultiplyPoint(pos_x1y0z0),
                        matrix.MultiplyPoint(pos_x0y1z0),
                        matrix.MultiplyPoint(pos_x0y0z1)),
                    new Triangle3(
                        matrix.MultiplyPoint(pos_x1y0z0),
                        matrix.MultiplyPoint(pos_x1y1z0),
                        matrix.MultiplyPoint(pos_x0y1z0))
                   }.ChangeOrderIfTrue(isInverted);
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
                return new Triangle3[]{
                    new Triangle3(
                        matrix.MultiplyPoint(pos_x0y1z1),
                        matrix.MultiplyPoint(pos_x1y0z1),
                        matrix.MultiplyPoint(pos_x1y1z0))
                   }.ChangeOrderIfTrue(isInverted);
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
                return new Triangle3[]{
                    new Triangle3(
                        matrix.MultiplyPoint(pos_x0y0z1),
                        matrix.MultiplyPoint(pos_x1y0z0),
                        matrix.MultiplyPoint(pos_x1y1z1)),
                    new Triangle3(
                        matrix.MultiplyPoint(pos_x0y0z1),
                        matrix.MultiplyPoint(pos_x1y1z1),
                        matrix.MultiplyPoint(pos_x0y1z0)),
                   }.ChangeOrderIfTrue(isInverted);
            }
            // else if (SampledData3b.ParseCube(@"
            //   1-----0
            //  /|    /|
            // 1-+---1 |
            // | 1---+-0
            // |/    |/
            // 1-----1
            // ").EqualsRotationInvariant(data, out matrix))
            // {
            //     matrix = matrix.inverse;
            //     return new Triangle3[]{
            //         new Triangle3(
            //             matrix.MultiplyPoint(pos_x0y1z0),
            //             matrix.MultiplyPoint(pos_x1y0z1),
            //             matrix.MultiplyPoint(pos_x1y0z0)),
            //         new Triangle3(
            //             matrix.MultiplyPoint(pos_x0y1z0),
            //             matrix.MultiplyPoint(pos_x0y1z1),
            //             matrix.MultiplyPoint(pos_x1y0z1)),
            //        };
            // }
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
                return new Triangle3[]{
                    new Triangle3(
                        matrix.MultiplyPoint(pos_x0y0z0),
                        matrix.MultiplyPoint(pos_x1y1z0),
                        matrix.MultiplyPoint(pos_x0y0z1)),
                    new Triangle3(
                        matrix.MultiplyPoint(pos_x1y1z0),
                        matrix.MultiplyPoint(pos_x0y1z0),
                        matrix.MultiplyPoint(pos_x0y0z1)),
                   }.ChangeOrderIfTrue(isInverted);
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
                return new Triangle3[]{
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
                   }.ChangeOrderIfTrue(isInverted);
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
                return triangles;
            }
            return Enumerable.Empty<Triangle3>();
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


        // private readonly static SampledData3b[] Templates = new SampledData3b[]
        // {
        //     SampledData3b.Ceiling,
        //     SampledData3b.Floor,
        //     SampledData3b.SideX
        // };




        // private readonly static SampledData2i[] ExpandedTemplates = Templates
        //     .SelectMany(template =>
        //     {
        //         return new[]
        //         {
        //             template,
        //             template.GetRotatedXY(1),
        //             template.GetRotatedXY(2),
        //             template.GetRotatedXY(3),
        //         };
        //     })
        //     .Distinct()
        //     .ToArray();

        // private readonly static Dictionary<SampledData2i, SampledData2i> TileMap = SampledData2i.GenerateAllValues(new Range2i(-1, 1))
        //     .ToDictionary(inputValue => inputValue, inputValue =>
        //     {
        //         SampledData2i choosenTemplateTile = default(SampledData2i);
        //         float choosenTemplateTileDiff = float.MaxValue;
        //         for (int it = 0; it < ExpandedTemplates.Length; it++)
        //         {
        //             SampledData2i tile = ExpandedTemplates[it];
        //             var value = SampledData2i.Dif(tile, inputValue);
        //             if (value < choosenTemplateTileDiff)
        //             {
        //                 choosenTemplateTile = tile;
        //                 choosenTemplateTileDiff = value;
        //             }
        //         }
        //         return choosenTemplateTile;
        //     });












        // private bool IsFlipped(SampledData2i sampleData)
        // {
        //     var difMain = Mathf.Abs(sampleData.x0y0 - sampleData.x1y1);
        //     var difMinor = Mathf.Abs(sampleData.x1y0 - sampleData.x0y1);
        //     bool flip;
        //     if (difMain == difMinor)
        //     {
        //         var sumMain = sampleData.x0y0 + sampleData.x1y1;
        //         var sumMinor = sampleData.x1y0 + sampleData.x0y1;
        //         flip = sumMain < sumMinor;
        //     }
        //     else
        //     {
        //         flip = difMain < difMinor;
        //     }
        //     return flip;
        // }

        public IPooledTerrainMesh GetResultingMesh()
        {
            return pooledMesh;
        }
    }
}