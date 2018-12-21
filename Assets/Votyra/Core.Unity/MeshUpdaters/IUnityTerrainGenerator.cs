using System.Collections.Generic;
using Votyra.Core.Pooling;
using Votyra.Core.TerrainMeshes;

namespace Votyra.Core.TerrainGenerators
{
    public interface IUnityTerrainGenerator<TFrameData, TGroupKey>
        where TFrameData : IFrameData
    {
        IReadOnlyPooledDictionary<TGroupKey, UnityMesh> Generate(TFrameData data, IEnumerable<TGroupKey> groupsToUpdate);
    }
}