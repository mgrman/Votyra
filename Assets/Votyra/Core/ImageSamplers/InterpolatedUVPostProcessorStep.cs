using Votyra.Core.Models;

namespace Votyra.Core.ImageSamplers
{
    public class InterpolatedUVPostProcessorStep : ITerrainUVPostProcessor
    {
        private readonly int _subdivision;

        public InterpolatedUVPostProcessorStep(IInterpolationConfig interpolationConfig)
        {
            _subdivision = interpolationConfig.ImageSubdivision;
        }

        public Vector2f ProcessUV(Vector2f vertex) => vertex / _subdivision;

        public Vector2f ReverseUV(Vector2f vertex) => vertex;
    }
}