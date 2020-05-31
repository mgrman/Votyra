using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Votyra.Core
{
    [Serializable]
    public class TerrainAlgorithmUi
    {
        [FormerlySerializedAs("Name")]
        [SerializeField]
        private string name;

        [FormerlySerializedAs("Prefab")]
        [SerializeField]
        private GameObject prefab;

        //  User-defined conversion from double to Digit
        public static implicit operator TerrainAlgorithm(TerrainAlgorithmUi @this) => new TerrainAlgorithm(@this.name, @this.prefab);
    }
}
