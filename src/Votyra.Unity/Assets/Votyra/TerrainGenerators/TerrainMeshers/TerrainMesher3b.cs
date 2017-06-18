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

        public void AddCell(Vector3i cellInGroup)
        {
            Vector3i cell = cellInGroup + groupPosition;

            SampledData3b data = ImageSampler.Sample(Image, cell);


            var pos_x0y0z0 = new Vector3(0, 0, 0);
            var pos_x0y0z1 = new Vector3(0, 0, 1);
            var pos_x0y1z0 = new Vector3(0, 1, 0);
            var pos_x0y1z1 = new Vector3(0, 1, 1);
            var pos_x1y0z0 = new Vector3(1, 0, 0);
            var pos_x1y0z1 = new Vector3(1, 0, 1);
            var pos_x1y1z0 = new Vector3(1, 1, 0);
            var pos_x1y1z1 = new Vector3(1, 1, 1);

            Matrix4x4 matrix;
            if (SampledData3b.Floor.EqualsRotationInvariant(data, out matrix))
            {
                matrix = matrix.inverse;
                mesh.AddQuad(
                     matrix.MultiplyPoint(pos_x0y0z0),
                     matrix.MultiplyPoint(pos_x0y1z0),
                     matrix.MultiplyPoint(pos_x1y0z0),
                     matrix.MultiplyPoint(pos_x1y1z0), false);
            }
            else if (SampledData3b.SlopeX.EqualsRotationInvariant(data, out matrix))
            {
                matrix = matrix.inverse;
                mesh.AddQuad(
                   matrix.MultiplyPoint(pos_x0y0z1),
                   matrix.MultiplyPoint(pos_x0y1z1),
                   matrix.MultiplyPoint(pos_x1y0z0),
                   matrix.MultiplyPoint(pos_x1y1z0), false);
            }
            // else if (data == SampledData3b.Ceiling)
            // {
            //     mesh.AddQuad(pos_x0y0z1, pos_x0y1z1, pos_x1y0z1, pos_x1y1z1, false);
            // }
            // else if (data.EqualsRotationXYInvariant(SampledData3b.SideX))
            // {
            //     if (data.Data_x0y0z0 && data.Data_x0y1z0)
            //     {
            //         mesh.AddWall(pos_x0y0z1, pos_x0y1z1, pos_x0y1z0, pos_x0y0z0, false);

            //     }
            //     else if (data.Data_x1y0z0 && data.Data_x0y0z0)
            //     {
            //         mesh.AddWall(pos_x1y0z1, pos_x0y0z1, pos_x0y0z0, pos_x1y0z0, false);

            //     }
            //     else if (data.Data_x1y0z0 && data.Data_x1y1z0)
            //     {
            //         mesh.AddWall(pos_x1y0z1, pos_x1y1z1, pos_x1y1z0, pos_x1y0z0, false);

            //     }
            //     else if (data.Data_x1y1z0 && data.Data_x0y1z0)
            //     {
            //         mesh.AddWall(pos_x1y1z1, pos_x0y1z1, pos_x0y1z0, pos_x1y1z0, false);
            //     }
            // }
            // else if (data.EqualsRotationXYInvariant(SampledData3b.SlopeX))
            // {
            //     if (data.Data_x0y0z0 && data.Data_x0y1z0) //riseX+
            //     {
            //         mesh.AddQuad(pos_x0y0z0, pos_x0y1z0, pos_x1y0z1, pos_x1y1z1, false);
            //     }
            //     else if (data.Data_x0y0z0 && data.Data_x1y0z0) //riseY+
            //     {
            //         mesh.AddQuad(pos_x0y0z0, pos_x0y1z1, pos_x1y0z0, pos_x1y1z1, false);

            //     }
            //     else if (data.Data_x0y1z0 && data.Data_x1y1z0)//riseY-
            //     {
            //         mesh.AddQuad(pos_x0y0z1, pos_x0y1z0, pos_x1y0z1, pos_x1y1z0, false);

            //     }
            //     else if (data.Data_x1y0z0 && data.Data_x1y1z0) //riseX-
            //     {
            //         mesh.AddQuad(pos_x0y0z1, pos_x0y1z1, pos_x1y0z0, pos_x1y1z0, false);
            //     }
            // }


            // if (data.Data > 0 && data.Data != byte.MaxValue)
            // {
            //     if (data.Data_x0y0z0)
            //         AddSmallDebugCube(pos_center + pos_x0y0z0, 0.1f);
            //     if (data.Data_x0y0z1)
            //         AddSmallDebugCube(pos_center + pos_x0y0z1, 0.1f);
            //     if (data.Data_x0y1z0)
            //         AddSmallDebugCube(pos_center + pos_x0y1z0, 0.1f);
            //     if (data.Data_x0y1z1)
            //         AddSmallDebugCube(pos_center + pos_x0y1z1, 0.1f);
            //     if (data.Data_x1y0z0)
            //         AddSmallDebugCube(pos_center + pos_x1y0z0, 0.1f);
            //     if (data.Data_x1y0z1)
            //         AddSmallDebugCube(pos_center + pos_x1y0z1, 0.1f);
            //     if (data.Data_x1y1z0)
            //         AddSmallDebugCube(pos_center + pos_x1y1z0, 0.1f);
            //     if (data.Data_x1y1z1)
            //         AddSmallDebugCube(pos_center + pos_x1y1z1, 0.1f);

            //     // AddSmallDebugCube(pos_center, 0.2f);
            // }

            // mesh.AddQuad(pos_x0y0z1, pos_x0y1z1, pos_x1y0z1, pos_x1y1z1, false);

            // mesh.AddWall(pos_x0y0z1, pos_x0y1z1, pos_x0y1z0, pos_x0y0z0, false);

            // mesh.AddWall(pos_x1y0z1, pos_x0y0z1, pos_x0y0z0, pos_x1y0z0, false);
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