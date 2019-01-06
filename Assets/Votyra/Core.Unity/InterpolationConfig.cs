namespace Votyra.Core.Images
{
    public class InterpolationConfig : IInterpolationConfig
    {
        public InterpolationConfig([ConfigInject("activeAlgorithm")] IntepolationAlgorithm activeAlgorithm, [ConfigInject("dynamicMeshes")] bool dynamicMeshes, [ConfigInject("imageSubdivision")] int imageSubdivision, [ConfigInject("meshSubdivision")] int meshSubdivision)
        {
            ActiveAlgorithm = activeAlgorithm;
            DynamicMeshes = dynamicMeshes;
            ImageSubdivision = imageSubdivision;
            MeshSubdivision = meshSubdivision;
        }

        public int ImageSubdivision { get; }
        public int MeshSubdivision { get; }
        public bool DynamicMeshes { get; }
        public IntepolationAlgorithm ActiveAlgorithm { get; }

        protected bool Equals(InterpolationConfig other) => ImageSubdivision == other.ImageSubdivision && MeshSubdivision == other.MeshSubdivision && DynamicMeshes == other.DynamicMeshes && ActiveAlgorithm == other.ActiveAlgorithm;

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
                var hashCode = ImageSubdivision;
                hashCode = (hashCode * 397) ^ MeshSubdivision;
                hashCode = (hashCode * 397) ^ DynamicMeshes.GetHashCode();
                hashCode = (hashCode * 397) ^ (int) ActiveAlgorithm;
                return hashCode;
            }
        }
    }
}