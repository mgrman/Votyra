using System.Collections.Generic;
using Votyra.Core.Models;

namespace Votyra.Core.ImageSamplers
{
    public class CompositeUVPostProcessor : ITerrainUVPostProcessor
    {
        private readonly List<ITerrainUVPostProcessorStep> _steps;

        public CompositeUVPostProcessor(List<ITerrainUVPostProcessorStep> steps)
        {
            _steps = steps;
        }

        public Vector2f ProcessUV(Vector2f vertex)
        {
            for (var i = 0; i < _steps.Count; i++)
            {
                vertex = _steps[i]
                    .ProcessUV(vertex);
            }

            return vertex;
        }

        public Vector2f ReverseUV(Vector2f vertex)
        {
            for (var i = _steps.Count - 1; i >= 0; i--)
            {
                vertex = _steps[i]
                    .ReverseUV(vertex);
            }

            return vertex;
        }
    }
}