using Votyra.Common.Models;
using Votyra.Common.Utils;
using Votyra.TerrainGenerators;
using Votyra.TerrainMeshers.TriangleMesh;
using UnityEngine;

namespace Votyra.TerrainMeshers
{
    public class TerrainMesher : ITerrainMesher
    {
        public Vector2i CellInGroupCount { get; private set; }
        protected Vector2i groupPosition;
        protected Vector3 bounds_center;
        protected Vector3 bounds_size;
        protected ITerrainMesh mesh;
        protected IMatrix<ResultHeightData> results;

        public virtual void Initialize(ITerrainContext terrainOptions)
        {
            this.CellInGroupCount = terrainOptions.CellInGroupCount;
            this.bounds_center = terrainOptions.GroupBounds.center;
            this.bounds_size = terrainOptions.GroupBounds.size;
        }

        public virtual void InitializeGroup(Vector2i group, ITerrainMesh mesh, IMatrix<ResultHeightData> data)
        {
            this.results = data;
            this.mesh = mesh;
            var bounds = new Bounds(new Vector3
             (
                 bounds_center.x + (group.x * CellInGroupCount.x),
                 bounds_center.y + (group.y * CellInGroupCount.y),
                 bounds_center.z
             ), bounds_size);
            mesh.Clear(bounds);

            this.groupPosition = CellInGroupCount * group;
        }

        public void AddCell(Vector2i cellInGroup)
        {
            if (cellInGroup.x >= 0 && cellInGroup.y >= 0)
            {
                Vector2i position = groupPosition + cellInGroup;

                ResultHeightData heightData = results[cellInGroup];

                ResultHeightData minusXres = results[cellInGroup.x - 1, cellInGroup.y];
                ResultHeightData minusYres = results[cellInGroup.x, cellInGroup.y - 1];

                Vector3 pos_x0y0 = new Vector3(position.x, position.y, heightData.data.x0y0);
                Vector3 pos_x0y1 = new Vector3(position.x, position.y + 1, heightData.data.x0y1);
                Vector3 pos_x1y0 = new Vector3(position.x + 1, position.y, heightData.data.x1y0);
                Vector3 pos_x1y1 = new Vector3(position.x + 1, position.y + 1, heightData.data.x1y1);

                Vector3 pos_x0y0_lowerY = new Vector3(position.x, position.y, minusXres.data.x1y0);
                Vector3 pos_x0y1_lowerY = new Vector3(position.x, position.y + 1, minusXres.data.x1y1);

                Vector3 pos_x0y0_lowerX = new Vector3(position.x, position.y, minusYres.data.x0y1);
                Vector3 pos_x1y0_lowerX = new Vector3(position.x + 1, position.y, minusYres.data.x1y1);

                mesh.AddQuad(cellInGroup, pos_x0y0, pos_x0y1, pos_x1y0, pos_x1y1, heightData.flip);

                mesh.AddWallY(cellInGroup, pos_x0y0, pos_x0y1, pos_x0y1_lowerY, pos_x0y0_lowerY);

                mesh.AddWallX(cellInGroup, pos_x1y0, pos_x0y0, pos_x0y0_lowerX, pos_x1y0_lowerX);
            }
        }
    }
}