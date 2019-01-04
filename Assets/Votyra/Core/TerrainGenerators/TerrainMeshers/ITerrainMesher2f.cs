using Votyra.Core.Images;
using Votyra.Core.Models;
using Votyra.Core.Pooling;

namespace Votyra.Core.TerrainGenerators.TerrainMeshers
{
    public interface ITerrainMesher2f
    {
        Range2i AdjustAreaOfInfluenceOfInvalidatedArea(Range2i invalidatedArea);
        
        IPooledTerrainMesh GetResultingMesh(Vector2i group, IImage2f image, IMask2e mask);
    }
}