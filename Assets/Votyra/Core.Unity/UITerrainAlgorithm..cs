using System;
using UnityEngine;

namespace Votyra.Core.Unity
{
    [Serializable]
    public class UITerrainAlgorithm
    {
        [SerializeField]
        public string Name;

        [SerializeField]
        public GameObject Prefab;

        //  User-defined conversion from double to Digit
        public static implicit operator TerrainAlgorithm(UITerrainAlgorithm @this) => new TerrainAlgorithm(@this.Name, @this.Prefab);
    }
}