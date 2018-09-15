namespace Votyra.Core.TerrainGenerators.TerrainMeshers.CellComputers
{
    public interface IInterpolatorPreparator
    {
        float[,] InterpolationMatrix { get; }
    }
}