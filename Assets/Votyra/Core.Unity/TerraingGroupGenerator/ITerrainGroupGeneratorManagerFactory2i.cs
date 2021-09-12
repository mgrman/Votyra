using Votyra.Core.Models;

namespace Votyra.Core
{
    public interface ITerrainGroupGeneratorManagerFactory2i
    {
        ITerrainGroupGeneratorManager2i CreateGroupManager(Vector2i newGroup);
    }
}