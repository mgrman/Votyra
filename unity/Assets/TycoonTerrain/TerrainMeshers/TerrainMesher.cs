using TycoonTerrain.Common.Models;
using TycoonTerrain.Common.Utils;
using TycoonTerrain.TerrainGenerators;
using TycoonTerrain.TerrainMeshers.TriangleMesh;
using UnityEngine;

namespace TycoonTerrain.TerrainMeshers
{
    public class TerrainMesher : ITerrainMesher
    {
        protected const int CELL_TO_TRIANGLE = 3 * 2;

        protected Vector2i cellInGroupCount;
        protected Vector2i groupPosition;
        public int TriangleCount { get; private set; }
        protected Vector3 bounds_center;
        protected Vector3 bounds_size;
        protected int quadIndex;
        protected bool flipTriangles;
        protected ITriangleMesh mesh;
        protected IMatrix<ResultHeightData> results;

        public virtual void Initialize(TerrainOptions terrainOptions)
        {
            this.cellInGroupCount = terrainOptions.CellInGroupCount;
            this.TriangleCount = terrainOptions.CellInGroupCount.AreaSum * CELL_TO_TRIANGLE;
            this.bounds_center = terrainOptions.GroupBounds.center;
            this.bounds_size = terrainOptions.GroupBounds.size;
            this.flipTriangles = terrainOptions.FlipTriangles;
        }

        public virtual void InitializeGroup(Vector2i group, ITriangleMesh mesh, IMatrix<ResultHeightData> data)
        {
            this.results = data;
            this.mesh = mesh;
            var bounds = new Bounds(new Vector3
             (
                 bounds_center.x + (group.x * cellInGroupCount.x),
                 bounds_center.y + (group.y * cellInGroupCount.y),
                 bounds_center.z
             ), bounds_size);
            mesh.Clear(bounds);

            this.groupPosition = cellInGroupCount * group;
            this.quadIndex = 0;
        }

        public virtual void AddCell(Vector2i cellInGroup)
        {
            if (cellInGroup.x >= 0 && cellInGroup.y >= 0)
            {
                Vector2i position = groupPosition + cellInGroup;

                var cell_area = RectUtils.FromMinAndSize(position.x, position.y, 1, 1);

                ResultHeightData heightData = results[cellInGroup];

                mesh.AddQuad(quadIndex, cell_area, heightData, heightData.flip);
                quadIndex++;

                var minusXres = results[cellInGroup.x - 1, cellInGroup.y];
                float minusXres_x1y1 = minusXres.data.x1y1;
                float minusXres_x1y0 = minusXres.data.x1y0;
                mesh.AddWall(quadIndex,
                    new Vector3(position.x, position.y, heightData.data.x0y0),
                    new Vector3(position.x, position.y + 1, heightData.data.x0y1),
                    new Vector3(position.x, position.y + 1, minusXres_x1y1),
                    new Vector3(position.x, position.y, minusXres_x1y0));
                quadIndex++;

                var minusYres = results[cellInGroup.x, cellInGroup.y - 1];
                float minusYres_x0y1 = minusYres.data.x0y1;
                float minusYres_x1y1 = minusYres.data.x1y1;
                mesh.AddWall(quadIndex,
                    new Vector3(position.x + 1, position.y, heightData.data.x1y0),
                    new Vector3(position.x, position.y, heightData.data.x0y0),
                    new Vector3(position.x, position.y, minusYres_x0y1),
                    new Vector3(position.x + 1, position.y, minusYres_x1y1));
                quadIndex++;
            }
        }
    }
}