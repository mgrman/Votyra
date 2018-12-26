using System;
using System.Collections.Generic;
using UnityEngine;
using Votyra.Core.Images;
using Votyra.Core.ImageSamplers;
using Votyra.Core.Models;
using Votyra.Core.Pooling;
using Votyra.Core.TerrainMeshes;
using Votyra.Core.Utils;

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
            for (int i = 0; i < _steps.Count; i++)
            {
                vertex = _steps[i].ProcessUV(vertex);
            }

            return vertex;
        }
        
        public Vector2f ReverseUV(Vector2f vertex)
        {
            for (int i = _steps.Count-1; i >=0; i--)
            {
                vertex = _steps[i].ReverseUV(vertex);
            }

            return vertex;
        }
    }
}