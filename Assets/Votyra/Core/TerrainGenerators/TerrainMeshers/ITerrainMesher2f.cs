using Votyra.Core.Images;
using Votyra.Core.Models;
using Votyra.Core.Pooling;

namespace Votyra.Core.TerrainGenerators.TerrainMeshers
{
    public interface ITerrainMesher2f
    {
        Range2i AdjustAreaOfInfluenceOfInvalidatedArea(Range2i invalidatedArea);
        
        void Initialize(IImage2f image, IMask2e mask);

        void InitializeGroup(Vector2i group);

        IPooledTerrainMesh GetResultingMesh();
    }
}