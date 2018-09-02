using System;
using System.Collections.Generic;
using UnityEngine;
using Votyra.Core.Images;
using Votyra.Core.ImageSamplers;
using Votyra.Core.Models;
using Votyra.Core.Pooling;
using Votyra.Core.TerrainMeshes;

namespace Votyra.Core.TerrainGenerators.TerrainMeshers.CellComputers
{
    public class EnsureFlatInterpolatorDecorator : IInterpolator
    {
        public IInterpolator ChildInterpolator { get; }

        public float[,] InterpolationMatrix => ChildInterpolator.InterpolationMatrix;

        private float? _flatValue;

        public EnsureFlatInterpolatorDecorator(IInterpolator interpolator)
        {
            this.ChildInterpolator = interpolator;
        }
        public void PrepareInterpolation(Vector2i cell, Func<Vector2i, SampledData2f> sampleFunc)
        {
            _flatValue = CellFlatMode(cell, sampleFunc);
            if (!_flatValue.HasValue)
            {
                ChildInterpolator.PrepareInterpolation(cell, sampleFunc);
            }
        }

        private float? CellFlatMode(Vector2i cell, Func<Vector2i, SampledData2f> sampleFunc)
        {
            var data = sampleFunc(cell);
            return ((data.x0y0 == data.x0y1) && (data.x0y1 == data.x1y0) && (data.x1y0 == data.x1y1)) ? data.x0y0 : (float?)null;
        }

        public float Sample(Vector2f pos)
        {
            if (_flatValue.HasValue)
                return _flatValue.Value;
            else if (pos.X == 0f && IsFlat(0, 1))
                return InterpolationMatrix[0, 1];
            else if (pos.X == 1f && IsFlat(2, 1))
                return InterpolationMatrix[2, 1];
            else if (pos.Y == 0f && IsFlat(1, 0))
                return InterpolationMatrix[1, 0];
            else if (pos.Y == 1f && IsFlat(1, 2))
                return InterpolationMatrix[1, 2];
            else if (pos.X == 0f && pos.Y == 0f && IsFlat(0, 0))
                return InterpolationMatrix[0, 0];
            else if (pos.X == 0f && pos.Y == 1f && IsFlat(0, 2))
                return InterpolationMatrix[0, 2];
            else if (pos.X == 1f && pos.Y == 0f && IsFlat(2, 0))
                return InterpolationMatrix[2, 0];
            else if (pos.X == 1f && pos.Y == 1f && IsFlat(2, 2))
                return InterpolationMatrix[2, 2];
            else
                return ChildInterpolator.Sample(pos);
        }


        private bool IsFlat(int ix, int iy)
        {
            return InterpolationMatrix[ix, iy] == InterpolationMatrix[ix, iy + 1]
                && InterpolationMatrix[ix + 1, iy] == InterpolationMatrix[ix + 1, iy + 1]
                && InterpolationMatrix[ix, iy] == InterpolationMatrix[ix + 1, iy + 1];
        }

    }
}