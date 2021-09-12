using UnityEngine;

namespace Votyra.Core.Images
{
    public class MaterialConfig : IMaterialConfig
    {
        public MaterialConfig([ConfigInject("material")] Material material, [ConfigInject("materialWalls")] Material materialWalls)
        {
            Material = material;
            MaterialWalls = materialWalls;
        }

        public Material Material { get; }
        
        public Material MaterialWalls { get; }

        public static bool operator ==(MaterialConfig a, MaterialConfig b) => a?.Equals(b) ?? b?.Equals(a) ?? true;

        public static bool operator !=(MaterialConfig a, MaterialConfig b) => !(a == b);

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;
            var that = obj as MaterialConfig;

            return Material == that.Material && MaterialWalls == that.MaterialWalls;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return Material.GetHashCode() * 23 + MaterialWalls.GetHashCode() - 3;
            }
        }
    }
}