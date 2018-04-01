using System.Collections.Generic;
using Votyra.Core.Pooling;
using Votyra.Core.TerrainMeshes;

namespace Votyra.Core.TerrainGenerators
{
    public interface ITerrainGenerator<TFrameData, TGroupKey>
        where TFrameData : IFrameData
    {
        IReadOnlyPooledDictionary<TGroupKey, ITerrainMesh> Generate(TFrameData data, IEnumerable<TGroupKey> groupsToUpdate);
    }
}