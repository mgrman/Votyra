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
    public class DualSampleTerrainUVPostProcessor : ITerrainUVPostProcessorStep
    {
        public static readonly Vector2f Offset = new Vector2f(0.25f, 0.25f);
        public Vector2f ProcessUV(Vector2f vertex) => vertex / 2.0f + Offset;
        public Vector2f ReverseUV(Vector2f vertex) => (vertex - Offset) * 2.0f;
    }
}