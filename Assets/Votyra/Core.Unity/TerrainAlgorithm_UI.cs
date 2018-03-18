using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Votyra.Core.Models;
using System;
using UniRx;

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
        public static implicit operator TerrainAlgorithm(TerrainAlgorithm_UI @this)
        {
            return new TerrainAlgorithm(@this.Name, @this.Prefab);
        }
    }
}