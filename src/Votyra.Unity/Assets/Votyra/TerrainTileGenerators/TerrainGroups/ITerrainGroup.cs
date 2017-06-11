
using Votyra.Models;
using UnityEngine;

namespace Votyra.TerrainTileGenerators.TerrainGroups
{
    public interface ITerrainGroup
    {
        Vector2i Group { get; }

        MatrixWithOffset<ResultHeightData> Data { get; }

        void Clear(Vector2i group);

    }
}
