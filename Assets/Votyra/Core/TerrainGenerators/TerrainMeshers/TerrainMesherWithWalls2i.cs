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

            SampledData2h data = _imageSampler.Sample(_image, cell);
            SampledMask2e maskData = _imageSampler.Sample(_mask, cell);

            SampledData2h minusXres = _imageSampler.Sample(_image, cell + new Vector2i(-1, 0));
            SampledMask2e minusXresMaskData = _imageSampler.Sample(_mask, cell + new Vector2i(-1, 0));
            SampledData2h minusYres = _imageSampler.Sample(_image, cell + new Vector2i(0, -1));
            SampledMask2e minusYresMaskData = _imageSampler.Sample(_mask, cell + new Vector2i(0, -1));

            _mesh.AddQuad(position, data, maskData);

            _mesh.AddWallAlongX(position, data, maskData, minusYres, minusYresMaskData);
            _mesh.AddWallAlongY(position, data, maskData, minusXres, minusXresMaskData);
        }
    }
}