
using TycoonTerrain.Common.Models;
using UnityEngine;

namespace TycoonTerrain.TerrainGenerators
{
    public interface ITerrainGroup
    {
        Vector2i Group { get; }
         MatrixWithOffset<ResultHeightData> Data { get; }
        
        void Clear( Vector2i group);
    }
}
