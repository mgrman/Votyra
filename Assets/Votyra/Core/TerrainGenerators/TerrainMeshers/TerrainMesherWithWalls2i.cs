using System;
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

        protected override Func<Vector3f?, Vector3f?> PostProcessVertices { get; } = (Vector3f? data) =>
        {
            if (data == null)
            {
                return null;
            }
            var position = data.Value;

            var expandedCellIndex = position.RoundToVector3i() % 2;
            var x = position.X + (expandedCellIndex.X == 0 ? 0.5f : -0.5f);
            var y = position.Y + (expandedCellIndex.Y == 0 ? 0.5f : -0.5f);
            return new Vector3f(x, y, position.Z);
        };
    }
}