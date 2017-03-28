using TycoonTerrain.Common.Models;
using TycoonTerrain.TerrainMeshers.TriangleMesh;

namespace TycoonTerrain.TerrainMeshers
{
    public class DirectTerrainMesher : TerrainMesher
    {
        public override void AddCell(Vector2i cellInGroup)
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

                mesh.AddQuad(quadIndex, pos_x0y0, pos_x0y1, pos_x1y0, pos_x1y1, heightData.flip);
                quadIndex++;

                mesh.AddWall(quadIndex, pos_x0y0, pos_x0y1, pos_x0y1_lowerY, pos_x0y0_lowerY);
                quadIndex++;

                mesh.AddWall(quadIndex, pos_x1y0, pos_x0y0, pos_x0y0_lowerX, pos_x1y0_lowerX);
                quadIndex++;
            }
        }
    }
}