using System;
using UnityEngine;

namespace Votyra.Core
{
    [Serializable]
    public class TerrainAlgorithm_UI
    {
        [SerializeField]
        public string Name;

        [SerializeField]
        public GameObject Prefab;

        //  User-defined conversion from double to Digit
        public static implicit operator TerrainAlgorithm(TerrainAlgorithm_UI @this) => new TerrainAlgorithm(@this.Name, @this.Prefab);
    }
}