using System;
using Votyra.Core.Models;

namespace Votyra.Core.TerrainGenerators.TerrainMeshers.CellComputers
{
    public interface IInterpolator
    {
        float[,] InterpolationMatrix { get; }

        void PrepareInterpolation(Vector2i cell, Func<Vector2i, SampledData2f> sampleFunc);

        float Sample(Vector2f pos);
    }
}