using UnityEngine;

namespace Votyra.Core
{
    public interface ITerrainAlgorithm
    {
        string Name { get; }
        
        GameObject Prefab { get; }
    }
}