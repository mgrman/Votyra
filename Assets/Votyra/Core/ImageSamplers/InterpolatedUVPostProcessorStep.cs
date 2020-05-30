using Votyra.Core.Models;

namespace Votyra.Core.ImageSamplers
{
    public class InterpolatedUvPostProcessorStep : ITerrainUvPostProcessor
    {
        private readonly int subdivision;

        public InterpolatedUvPostProcessorStep(IInterpolationConfig interpolationConfig)
        {
            this.subdivision = interpolationConfig.ImageSubdivision;
        }

        public Vector2f ProcessUv(Vector2f vertex) => vertex / this.subdivision;

        public Vector2f ReverseUv(Vector2f vertex) => vertex;
    }
}
