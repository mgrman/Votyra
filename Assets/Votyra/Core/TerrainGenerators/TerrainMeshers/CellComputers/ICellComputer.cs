using Votyra.Core.Models;

namespace Votyra.Core.TerrainGenerators.TerrainMeshers.CellComputers
{
    public interface ICellComputer
    {
        Vector3f[,] PrepareCell(Vector2i cell);
    }
}