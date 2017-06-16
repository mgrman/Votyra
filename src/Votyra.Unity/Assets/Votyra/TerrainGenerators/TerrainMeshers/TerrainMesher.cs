using Votyra.Models;
using Votyra.Utils;
using Votyra.TerrainGenerators.TerrainMeshers.TerrainMeshes;
using UnityEngine;
using Votyra.Unity.Assets.Votyra.Pooling;

namespace Votyra.TerrainGenerators.TerrainMeshers
{
    public class TerrainMesher2i : ITerrainMesher2i
    {
        protected ITerrainGeneratorContext2i options;
        public Vector2i CellInGroupCount { get; private set; }
        protected Vector2i groupPosition;
        protected Vector3 bounds_center;
        protected Vector3 bounds_size;
        protected IPooledTerrainMesh pooledMesh;
        protected ITerrainMesh2 mesh;

        public void Initialize(ITerrainGeneratorContext2i terrainOptions)
        {
            options = terrainOptions;
            this.CellInGroupCount = terrainOptions.CellInGroupCount;
            this.bounds_center = terrainOptions.GroupBounds.center;
            this.bounds_size = terrainOptions.GroupBounds.size;
        }

        public void InitializeGroup(Vector2i group)
        {
            var bounds = new Bounds(new Vector3
             (
                 bounds_center.x + (group.x * CellInGroupCount.x),
                 bounds_center.y + (group.y * CellInGroupCount.y),
                 bounds_center.z
             ), bounds_size);

            this.groupPosition = CellInGroupCount * group;

            this.pooledMesh = PooledTerrainMeshContainer<FixedTerrainMesh2>.CreateDirty(CellInGroupCount);
            this.mesh = this.pooledMesh.Mesh;
            mesh.Clear(bounds);
        }

        public void AddCell(Vector2i cellInGroup)
        {
            Vector2i cell = cellInGroup + groupPosition;

            if (cellInGroup.x >= 0 && cellInGroup.y >= 0)
            {
                Vector2i position = groupPosition + cellInGroup;

                SampledData2i data = options.ImageSampler.Sample(options.Image, cell);

                int minusXres_x1y0 = options.ImageSampler.SampleX1Y0(options.Image, new Vector2i(cellInGroup.x - 1, cellInGroup.y - 0));
                int minusXres_x1y1 = options.ImageSampler.SampleX1Y1(options.Image, new Vector2i(cellInGroup.x - 1, cellInGroup.y - 0));
                int minusYres_x0y1 = options.ImageSampler.SampleX0Y1(options.Image, new Vector2i(cellInGroup.x - 0, cellInGroup.y - 1));
                int minusYres_x1y1 = options.ImageSampler.SampleX1Y1(options.Image, new Vector2i(cellInGroup.x - 0, cellInGroup.y - 1));

                Vector3 pos_x0y0 = new Vector3(position.x, position.y, data.x0y0);
                Vector3 pos_x0y1 = new Vector3(position.x, position.y + 1, data.x0y1);
                Vector3 pos_x1y0 = new Vector3(position.x + 1, position.y, data.x1y0);
                Vector3 pos_x1y1 = new Vector3(position.x + 1, position.y + 1, data.x1y1);

                Vector3 pos_x0y0_lowerY = new Vector3(position.x, position.y, minusXres_x1y0);
                Vector3 pos_x0y1_lowerY = new Vector3(position.x, position.y + 1, minusXres_x1y1);

                Vector3 pos_x0y0_lowerX = new Vector3(position.x, position.y, minusYres_x0y1);
                Vector3 pos_x1y0_lowerX = new Vector3(position.x + 1, position.y, minusYres_x1y1);

                mesh.AddQuad(cellInGroup, pos_x0y0, pos_x0y1, pos_x1y0, pos_x1y1, IsFlipped(data));

                mesh.AddWallY(cellInGroup, pos_x0y0, pos_x0y1, pos_x0y1_lowerY, pos_x0y0_lowerY);

                mesh.AddWallX(cellInGroup, pos_x1y0, pos_x0y0, pos_x0y0_lowerX, pos_x1y0_lowerX);
            }
        }

        private bool IsFlipped(SampledData2i sampleData)
        {
            var difMain = Mathf.Abs(sampleData.x0y0 - sampleData.x1y1);
            var difMinor = Mathf.Abs(sampleData.x1y0 - sampleData.x0y1);
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

        public IPooledTerrainMesh GetResultingMesh()
        {
            return pooledMesh;
        }
    }
}