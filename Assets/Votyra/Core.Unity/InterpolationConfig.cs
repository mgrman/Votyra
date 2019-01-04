using Votyra.Core.Models;

namespace Votyra.Core.Images
{
    public class InterpolationConfig : IInterpolationConfig
    {
        public InterpolationConfig([ConfigInject("imageSubdivision")]int subdivision,
            [ConfigInject("activeAlgorithm")] IntepolationAlgorithm activeAlgorithm)
        {
            Subdivision = subdivision;
            ActiveAlgorithm = activeAlgorithm;
        }

        public IntepolationAlgorithm ActiveAlgorithm { get; }
        
        public int Subdivision { get; }

        public static bool operator ==(InterpolationConfig a, InterpolationConfig b)
        {
            return a?.Equals(b) ?? b?.Equals(a) ?? true;
        }

        public static bool operator !=(InterpolationConfig a, InterpolationConfig b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            var that = obj as InterpolationConfig;

            return this.Subdivision == that.Subdivision
                   && this.ActiveAlgorithm == that.ActiveAlgorithm;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return this.Subdivision.GetHashCode()
                    + this.ActiveAlgorithm.GetHashCode()*7;
            }
        }
    }
}