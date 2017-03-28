using System;
using System.Collections.Generic;
using System.Linq;
using TycoonTerrain.Common.Models;
using TycoonTerrain.Images;
using TycoonTerrain.ImageSamplers;
using TycoonTerrain.Pooling;
using TycoonTerrain.TerrainAlgorithms;
using TycoonTerrain.TerrainMeshers;

namespace TycoonTerrain.TerrainGenerators
{
    public class TerrainOptions : IDisposable
    {
        private const int MAX_CELL_COUNT = 60 * 60;

        public readonly Vector2i CellInGroupCount;
        public readonly Bounds GroupBounds;
        public readonly Range2i RangeZ;

        public readonly IImage2i Image;
        public readonly IImageSampler ImageSampler;
        public readonly ITerrainAlgorithm TerrainAlgorithm;
        public readonly ITerrainMesher TerrainMesher;
        public readonly float Time;
        public readonly bool ComputeAsync;
        public readonly bool FlipTriangles;

        public IList<Vector2i> GroupsToUpdate;

        public TerrainOptions(Vector2i cellInGroupCount, bool flipTriangles, IImage2i image,
            IImageSampler sampler, ITerrainAlgorithm terrainAlgorithm, ITerrainMesher terrainMesher, float time, IList<Vector2i> groupsToUpdate)
        {
            this.CellInGroupCount = cellInGroupCount;
            this.FlipTriangles = flipTriangles;

            if (this.CellInGroupCount.AreaSum > MAX_CELL_COUNT)
            {
                throw new InvalidOperationException("Too many cells in group! Max is 60x60");
            }

            this.Image = image;

            this.ImageSampler = sampler;
            this.TerrainAlgorithm = terrainAlgorithm;
            this.TerrainMesher = terrainMesher;
            this.Time = time;

            this.RangeZ = Image.RangeZ;
            this.GroupBounds = new Bounds(new Vector3(CellInGroupCount.x / 2.0f, CellInGroupCount.y / 2.0f, RangeZ.Center), new Vector3(CellInGroupCount.x, CellInGroupCount.y, RangeZ.Size));

            this.GroupsToUpdate = groupsToUpdate;
        }

        public TerrainOptions(TerrainOptions template)
        {
            this.CellInGroupCount = template.CellInGroupCount;
            this.FlipTriangles = template.FlipTriangles;

            this.Image = template.Image;
            this.ImageSampler = template.ImageSampler;
            this.TerrainAlgorithm = template.TerrainAlgorithm;
            this.TerrainMesher = template.TerrainMesher;
            this.Time = template.Time;

            this.RangeZ = template.RangeZ;
            this.GroupBounds = template.GroupBounds;

            if (template.GroupsToUpdate != null)
            {
                var groupsToUpdate = Pool.Vector2ListPool.GetObject();
                groupsToUpdate.Clear();

                foreach (var group in template.GroupsToUpdate)
                {
                    groupsToUpdate.Add(group);
                }
                this.GroupsToUpdate = groupsToUpdate;
            }
        }

        public bool IsChanged(TerrainOptions old)
        {
            return old == null ||
                this.CellInGroupCount != old.CellInGroupCount ||
                this.FlipTriangles != old.FlipTriangles ||
                this.Image != old.Image ||
                this.Image.IsAnimated ||
                this.TerrainAlgorithm != old.TerrainAlgorithm ||
                this.ImageSampler != old.ImageSampler ||
                this.RangeZ != old.RangeZ ||
                ((this.GroupsToUpdate == null) != (old.GroupsToUpdate == null)) ||
                !this.GroupsToUpdate.SequenceEqual(old.GroupsToUpdate);
        }

        public bool IsBoundsChanged(TerrainOptions old)
        {
            return old == null ||
                this.CellInGroupCount != old.CellInGroupCount;
        }

        public bool IsValid
        {
            get
            {
                return this.TerrainAlgorithm != null
                    && this.CellInGroupCount.Positive
                    && this.Image != null
                    && this.ImageSampler != null
                    && this.TerrainMesher != null;
            }
        }

        public TerrainOptions Clone()
        {
            return new TerrainOptions(this);
        }

        public void Dispose()
        {
            if (GroupsToUpdate is List<Vector2i>)
            {
                Pool.Vector2ListPool.ReturnObject(GroupsToUpdate as List<Vector2i>);
                GroupsToUpdate = null;
            }
        }
    }
}