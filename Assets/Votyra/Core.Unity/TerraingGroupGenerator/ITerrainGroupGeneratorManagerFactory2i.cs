using Votyra.Core.Models;

namespace Votyra.Core.Unity.TerraingGroupGenerator
{
    public interface ITerrainGroupGeneratorManagerFactory2i
    {
        ITerrainGroupGeneratorManager2i CreateGroupManager(Vector2i newGroup);
    }
}