using UnityEngine;

namespace Votyra.Core.Images
{
    public class MaterialConfig : IMaterialConfig
    {
        public MaterialConfig([ConfigInject("material"),]
            Material material, [ConfigInject("materialWalls"),]
            Material materialWalls)
        {
            this.Material = material;
            this.MaterialWalls = materialWalls;
        }

        public Material Material { get; }

        public Material MaterialWalls { get; }

        public static bool operator ==(MaterialConfig a, MaterialConfig b) => a?.Equals(b) ?? b?.Equals(a) ?? true;

        public static bool operator !=(MaterialConfig a, MaterialConfig b) => !(a == b);

        public override bool Equals(object obj)
        {
            if ((obj == null) || (this.GetType() != obj.GetType()))
            {
                return false;
            }

            var that = obj as MaterialConfig;

            return (this.Material == that.Material) && (this.MaterialWalls == that.MaterialWalls);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((this.Material.GetHashCode() * 23) + this.MaterialWalls.GetHashCode()) - 3;
            }
        }
    }
}
