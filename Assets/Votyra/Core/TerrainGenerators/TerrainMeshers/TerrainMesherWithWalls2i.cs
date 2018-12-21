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

            var data = _imageSampler.Sample(_image, cell);
            var maskData = _imageSampler.Sample(_mask, cell);

            var x0y0 = maskData.x0y0.IsNotHole() ? new Vector2f(cell.X, cell.Y).ToVector3f(data.x0y0) : (Vector3f?)null;
            var x0y1 = maskData.x0y1.IsNotHole() ? new Vector2f(cell.X, cell.Y + 1).ToVector3f(data.x0y1) : (Vector3f?)null;
            var x1y0 = maskData.x1y0.IsNotHole() ? new Vector2f(cell.X + 1, cell.Y).ToVector3f(data.x1y0) : (Vector3f?)null;
            var x1y1 = maskData.x1y1.IsNotHole() ? new Vector2f(cell.X + 1, cell.Y + 1).ToVector3f(data.x1y1) : (Vector3f?)null;
            _mesh.AddQuad(MoveToCreateWalls(x0y0), MoveToCreateWalls(x0y1), MoveToCreateWalls(x1y0), MoveToCreateWalls(x1y1));
        }

        private Vector3f? MoveToCreateWalls(Vector3f? data)
        {
            if (data == null)
            {
                return null;
            }
            var position = data.Value;

            var x = position.X + (position.X % 2 < 0.5f ? 0.5f : -0.5f);
            var y = position.Y + (position.Y % 2 < 0.5f ? 0.5f : -0.5f);
            return new Vector3f(x, y, position.Z);
        }
    }
}