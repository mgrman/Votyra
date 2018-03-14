using System;
using Votyra.Core.Models;
using Votyra.Core.Pooling;
using Votyra.Core.TerrainMeshes;
using Votyra.Core.ImageSamplers;

namespace Votyra.Core.TerrainGenerators.TerrainMeshers
{
    public class TerrainMesherWithWalls2i : TerrainMesher2i
    {
        public override void AddCell(Vector2i cellInGroup)
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

            mesh.AddWall(pos_x0y0, pos_x0y1, pos_x0y1_lowerY, pos_x0y0_lowerY, false);

            mesh.AddWall(pos_x1y0, pos_x0y0, pos_x0y0_lowerX, pos_x1y0_lowerX, false);
        }
    }
}