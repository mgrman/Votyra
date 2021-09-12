using Votyra.Core.Models;

namespace Votyra.Core
{
    public interface ITerrainConfig : IConfig
    {
        Vector2i CellInGroupCount { get; }
        bool Async { get; }
    }
}