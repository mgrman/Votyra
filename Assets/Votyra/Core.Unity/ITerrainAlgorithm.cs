using UnityEngine;

namespace Votyra.Core.Unity
{
    public interface ITerrainAlgorithm
    {
        string Name { get; }
        
        GameObject Prefab { get; }
    }
}