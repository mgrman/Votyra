using Votyra.Core.Models;

namespace Votyra.Core.TerrainGenerators.TerrainMeshers
{
    public class WallsVertexPostProcessor : ITerrainVertexPostProcessor
    {
        public readonly float WallSquishFactor;

        public WallsVertexPostProcessor(IDualSampleConfig dualSampleConfig)
        {
            WallSquishFactor = dualSampleConfig.WallSquishFactor;
        }

        public Vector3f PostProcessVertex(Vector3f position)
        {
            var expandedCellIndex = position.XY - (position.XY / 2f).Floor() * 2f;


            var posX = position.X + ((expandedCellIndex.X < 1 ? expandedCellIndex.X : 2 - expandedCellIndex.X) - 0.5f) * WallSquishFactor;
            var posY = position.Y + ((expandedCellIndex.Y < 1 ? expandedCellIndex.Y : 2 - expandedCellIndex.Y) - 0.5f) * WallSquishFactor;
            return new Vector3f(posX, posY, position.Z);
        }
    }
}