using Votyra.Core.Models;

namespace Votyra.Core.ImageSamplers
{
    public class DualSampleTerrainUvPostProcessor : ITerrainUvPostProcessor
    {
        public static readonly Vector2f Offset = new Vector2f(0.25f, 0.25f);
        private readonly float subdivision;

        public DualSampleTerrainUvPostProcessor(IInterpolationConfig interpolationConfig)
        {
            this.subdivision = interpolationConfig.ImageSubdivision;
        }

        public Vector2f ProcessUv(Vector2f vertex) => ((vertex / 2.0f) + Offset) / this.subdivision;

        public Vector2f ReverseUv(Vector2f vertex) => (vertex - Offset) * 2.0f;
    }
}
