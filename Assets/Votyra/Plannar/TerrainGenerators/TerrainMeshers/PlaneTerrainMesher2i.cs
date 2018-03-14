using System;
using Votyra.Core.Models;
using Votyra.Core.Pooling;
using Votyra.Core.TerrainMeshes;
using Votyra.Plannar.ImageSamplers;

namespace Votyra.Plannar.TerrainGenerators.TerrainMeshers
{
    public class PlaneTerrainMesher2i : ITerrainMesher2i
    {
        protected ITerrainGeneratorContext2i options;
        public Vector2i CellInGroupCount { get; private set; }
        protected Vector2i groupPosition;
        protected Vector3f bounds_center;
        protected Vector3f bounds_size;
        protected IPooledTerrainMesh2i pooledMesh;
        protected ITerrainMesh2i mesh;

        public void Initialize(ITerrainGeneratorContext2i terrainOptions)
        {
            options = terrainOptions;
            this.CellInGroupCount = terrainOptions.CellInGroupCount;
            this.bounds_center = terrainOptions.GroupBounds.center;
            this.bounds_size = terrainOptions.GroupBounds.size;
        }

        public void InitializeGroup(Vector2i group)
        {
            var bounds = new Rect3f(new Vector3f
             (
                 bounds_center.x + (group.x * CellInGroupCount.x),
                 bounds_center.y + (group.y * CellInGroupCount.y),
                 bounds_center.z
             ), bounds_size);

            this.groupPosition = CellInGroupCount * group;

            this.pooledMesh = PooledTerrainMesh2iContainer<FixedTerrainMesh2i>.CreateDirty(CellInGroupCount);
            this.mesh = this.pooledMesh.Mesh;
            mesh.Clear(bounds);
        }

        public virtual void AddCell(Vector2i cellInGroup)
        {
            Vector2i cell = cellInGroup + groupPosition;

            Vector2i position = groupPosition + cellInGroup;

            SampledData2i data = options.ImageSampler.Sample(options.Image, cell);

            int minusXres_x1y0 = options.ImageSampler.SampleX1Y0(options.Image, new Vector2i(cell.x - 1, cell.y - 0));
            int minusXres_x1y1 = options.ImageSampler.SampleX1Y1(options.Image, new Vector2i(cell.x - 1, cell.y - 0));
            int minusYres_x0y1 = options.ImageSampler.SampleX0Y1(options.Image, new Vector2i(cell.x - 0, cell.y - 1));
            int minusYres_x1y1 = options.ImageSampler.SampleX1Y1(options.Image, new Vector2i(cell.x - 0, cell.y - 1));
            // Debug.Log($"{minusXres_x1y0} {minusXres_x1y1}");
            var pos_x0y0 = new Vector3f(position.x, position.y, data.x0y0);
            var pos_x0y1 = new Vector3f(position.x, position.y + 1, data.x0y1);
            var pos_x1y0 = new Vector3f(position.x + 1, position.y, data.x1y0);
            var pos_x1y1 = new Vector3f(position.x + 1, position.y + 1, data.x1y1);

            var pos_x0y0_lowerY = new Vector3f(position.x, position.y, minusXres_x1y0);
            var pos_x0y1_lowerY = new Vector3f(position.x, position.y + 1, minusXres_x1y1);

            var pos_x0y0_lowerX = new Vector3f(position.x, position.y, minusYres_x0y1);
            var pos_x1y0_lowerX = new Vector3f(position.x + 1, position.y, minusYres_x1y1);

            mesh.AddQuad(pos_x0y0, pos_x0y1, pos_x1y0, pos_x1y1, IsFlipped(data));
        }

        protected bool IsFlipped(SampledData2i sampleData)
        {
            var difMain = Math.Abs(sampleData.x0y0 - sampleData.x1y1);
            var difMinor = Math.Abs(sampleData.x1y0 - sampleData.x0y1);
            bool flip;
            if (difMain == difMinor)
            {
                var sumMain = sampleData.x0y0 + sampleData.x1y1;
                var sumMinor = sampleData.x1y0 + sampleData.x0y1;
                flip = sumMain < sumMinor;
            }
            else
            {
                flip = difMain < difMinor;
            }
            return flip;
        }

        public IPooledTerrainMesh2i GetResultingMesh()
        {
            return pooledMesh;
        }
    }
}