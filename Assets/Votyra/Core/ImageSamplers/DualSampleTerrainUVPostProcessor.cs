using Votyra.Core.Models;

namespace Votyra.Core.ImageSamplers
{
    public class DualSampleTerrainUVPostProcessor : ITerrainUVPostProcessor
    {
        public static readonly Vector2f Offset = new Vector2f(0.25f, 0.25f);
        private readonly float _subdivision;

        public DualSampleTerrainUVPostProcessor(IInterpolationConfig interpolationConfig)
        {
            _subdivision = interpolationConfig.ImageSubdivision;
        }

        public Vector2f ProcessUV(Vector2f vertex) => (vertex / 2.0f + Offset) / _subdivision;

        public Vector2f ReverseUV(Vector2f vertex) => (vertex - Offset) * 2.0f;
    }
}