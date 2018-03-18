using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Votyra.Core.Models;
using System;
using UniRx;

namespace Votyra.Core
{
    public class TerrainAlgorithm : ITerrainAlgorithm
    {
        public TerrainAlgorithm(string name, GameObject prefab)
        {
            Name = name;
            Prefab = prefab;
        }

        public string Name { get; }
        public GameObject Prefab { get; }

        public static bool operator ==(TerrainAlgorithm a, TerrainAlgorithm b)
        {
            return a?.Equals(b) ?? b?.Equals(a) ?? true;
        }

        public static bool operator !=(TerrainAlgorithm a, TerrainAlgorithm b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            var that = obj as TerrainAlgorithm;

            return this.Name == that.Name && this.Prefab == that.Prefab;
        }

        public override int GetHashCode()
        {
            return this.Name.GetHashCode() + this.Prefab.GetHashCode() * 7;
        }
    }
}