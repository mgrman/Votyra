using Votyra.Core.ImageSamplers;
using Votyra.Core.Models;
using Votyra.Core.TerrainMeshes;

namespace Votyra.Core.TerrainGenerators.TerrainMeshers
{
    public class TerrainMesherWithWalls2i : TerrainMesher2i
    {
        public TerrainMesherWithWalls2i(ITerrainConfig terrainConfig, IImageSampler2i imageSampler)
            : base(terrainConfig, imageSampler)
        {
        }

        protected override int TrianglesPerCell => 6;

        public override void AddCell(Vector2i cellInGroup)
        {
            Vector2i cell = cellInGroup + _groupPosition;

            Vector2i position = _groupPosition + cellInGroup;

            SampledData2i data = _imageSampler.Sample(_image, cell);

            var minusXres_x1y0 = _imageSampler.SampleX1Y0(_image, new Vector2i(cell.X - 1, cell.Y - 0));
            var minusXres_x1y1 = _imageSampler.SampleX1Y1(_image, new Vector2i(cell.X - 1, cell.Y - 0));
            var minusYres_x0y1 = _imageSampler.SampleX0Y1(_image, new Vector2i(cell.X - 0, cell.Y - 1));
            var minusYres_x1y1 = _imageSampler.SampleX1Y1(_image, new Vector2i(cell.X - 0, cell.Y - 1));
            // Debug.Log($"{minusXres_x1y0} {minusXres_x1y1}");

            _mesh.AddQuad(position, data);

            _mesh.AddWallAlongX(position, data, minusYres_x0y1, minusYres_x1y1);
            _mesh.AddWallAlongY(position, data, minusXres_x1y0, minusXres_x1y1);
        }
    }
}