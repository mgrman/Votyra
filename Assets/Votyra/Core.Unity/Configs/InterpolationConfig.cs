using Votyra.Core.Models;

namespace Votyra.Core.Images
{
    public class InterpolationConfig : IInterpolationConfig
    {
        public InterpolationConfig([ConfigInject("activeAlgorithm")]IntepolationAlgorithm activeAlgorithm, [ConfigInject("dynamicMeshes")]bool dynamicMeshes, [ConfigInject("imageSubdivision")]int imageSubdivision, [ConfigInject("meshSubdivision")]Vector2i meshSubdivision)
        {
            this.ActiveAlgorithm = activeAlgorithm;
            this.DynamicMeshes = dynamicMeshes;
            this.ImageSubdivision = imageSubdivision;
            this.MeshSubdivision = meshSubdivision;
        }

        public int ImageSubdivision { get; }

        public Vector2i MeshSubdivision { get; }

        public bool DynamicMeshes { get; }

        public IntepolationAlgorithm ActiveAlgorithm { get; }

        protected bool Equals(InterpolationConfig other) => (this.ImageSubdivision == other.ImageSubdivision) && (this.MeshSubdivision == other.MeshSubdivision) && (this.DynamicMeshes == other.DynamicMeshes) && (this.ActiveAlgorithm == other.ActiveAlgorithm);

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return this.Equals((InterpolationConfig)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = this.ImageSubdivision;
                hashCode = (hashCode * 397) ^ this.MeshSubdivision.GetHashCode();
                hashCode = (hashCode * 397) ^ this.DynamicMeshes.GetHashCode();
                hashCode = (hashCode * 397) ^ (int)this.ActiveAlgorithm;
                return hashCode;
            }
        }
    }
}
