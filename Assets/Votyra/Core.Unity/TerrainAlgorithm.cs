using UnityEngine;

namespace Votyra.Core.Unity
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

        public static bool operator ==(TerrainAlgorithm a, TerrainAlgorithm b) => a?.Equals(b) ?? b?.Equals(a) ?? true;

        public static bool operator !=(TerrainAlgorithm a, TerrainAlgorithm b) => !(a == b);

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;
            var that = obj as TerrainAlgorithm;

            return Name == that.Name && Prefab == that.Prefab;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return Name.GetHashCode() + Prefab.GetHashCode() * 7;
            }
        }
    }
}