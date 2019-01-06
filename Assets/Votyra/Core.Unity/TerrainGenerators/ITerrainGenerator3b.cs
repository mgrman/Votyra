using Votyra.Core.Images;
using Votyra.Core.Models;
using Votyra.Core.Pooling;

namespace Votyra.Core.TerrainGenerators
{
    public interface ITerrainGenerator3b
    {
        IPooledTerrainMesh Generate(Vector3i group, IImage3b image);
    }
}