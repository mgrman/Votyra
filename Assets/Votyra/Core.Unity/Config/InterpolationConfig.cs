namespace Votyra.Core.Unity.Config
{
    public class InterpolationConfig : IInterpolationConfig
    {
        public InterpolationConfig([ConfigInject("isBicubic")] bool isBicubic, [ConfigInject("dynamicMeshes")] bool dynamicMeshes)
        {
            IsBicubic = isBicubic;
            DynamicMeshes = dynamicMeshes;
        }

        public int MeshSubdivision => IsBicubic ? 4 : 1;
        public bool DynamicMeshes { get; }
        public bool IsBicubic { get; }

        protected bool Equals(InterpolationConfig other) =>
            MeshSubdivision == other.MeshSubdivision && DynamicMeshes == other.DynamicMeshes;

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != GetType())
                return false;
            return Equals((InterpolationConfig) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = MeshSubdivision;
                hashCode = (hashCode * 397) ^ DynamicMeshes.GetHashCode();
                hashCode = (hashCode * 397) ^ (IsBicubic?1:0);
                return hashCode;
            }
        }
    }
}