using Votyra.Core.Models;

namespace Votyra.Core.Images
{
    public class InterpolationConfig : IInterpolationConfig
    {
        public InterpolationConfig([ConfigInject("imageSubdivision")]int subdivision)
        {
            Subdivision = subdivision;
        }

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

            return this.Subdivision == that.Subdivision;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return this.Subdivision.GetHashCode();
            }
        }
    }
}