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
    public class BicubicInterpolator : IInterpolator
    {
        public float[,] InterpolationMatrix { get; private set; }

        public BicubicInterpolator()
        {
            this.InterpolationMatrix = new float[4, 4];
        }

        public virtual void PrepareInterpolation(Vector2i cell, Func<Vector2i, SampledData2f> sampleFunc)
        {
            var data_x0y0 = sampleFunc(cell - Vector2i.One + new Vector2i(0, 0));
            // var data_x0y1 = _imageSampler.Sample(_image, cell - Vector2i.One + new Vector2i(0, 1));
            var data_x0y2 = sampleFunc(cell - Vector2i.One + new Vector2i(0, 2));

            // var data_x1y0 = _imageSampler.Sample(_image, cell - Vector2i.One + new Vector2i(1, 0));
            // var data_x1y1 = _imageSampler.Sample(_image, cell - Vector2i.One + new Vector2i(1, 1));
            // var data_x1y2 = _imageSampler.Sample(_image, cell - Vector2i.One + new Vector2i(1, 2));
            var data_x2y0 = sampleFunc(cell - Vector2i.One + new Vector2i(2, 0));
            // var data_x2y1 = _imageSampler.Sample(_image, cell - Vector2i.One + new Vector2i(2, 1));
            var data_x2y2 = sampleFunc(cell - Vector2i.One + new Vector2i(2, 2));

            InterpolationMatrix[0, 0] = data_x0y0.x0y0;
            InterpolationMatrix[0, 1] = data_x0y0.x0y1;
            InterpolationMatrix[0, 2] = data_x0y2.x0y0;
            InterpolationMatrix[0, 3] = data_x0y2.x0y1;

            InterpolationMatrix[1, 0] = data_x0y0.x1y0;
            InterpolationMatrix[1, 1] = data_x0y0.x1y1;
            InterpolationMatrix[1, 2] = data_x0y2.x1y0;
            InterpolationMatrix[1, 3] = data_x0y2.x1y1;

            InterpolationMatrix[2, 0] = data_x2y0.x0y0;
            InterpolationMatrix[2, 1] = data_x2y0.x0y1;
            InterpolationMatrix[2, 2] = data_x2y2.x0y0;
            InterpolationMatrix[2, 3] = data_x2y2.x0y1;

            InterpolationMatrix[3, 0] = data_x2y0.x1y0;
            InterpolationMatrix[3, 1] = data_x2y0.x1y1;
            InterpolationMatrix[3, 2] = data_x2y2.x1y0;
            InterpolationMatrix[3, 3] = data_x2y2.x1y1;
        }

        // t is a value that goes from 0 to 1 to interpolate in a C1 continuous way across uniformly sampled data points.
        // when t is 0, this will return B.  When t is 1, this will return C.  Inbetween values will return an interpolation
        // between B and C.  A and B are used to calculate slopes at the edges.
        private float CubicHermite(float A, float B, float C, float D, float t)
        {
            float a = -A / 2.0f + (3.0f * B) / 2.0f - (3.0f * C) / 2.0f + D / 2.0f;
            float b = A - (5.0f * B) / 2.0f + 2.0f * C - D / 2.0f;
            float c = -A / 2.0f + C / 2.0f;
            float d = B;

            return a * t * t * t + b * t * t + c * t + d;
        }

        public float Sample(Vector2f pos)
        {
            float xfract = pos.X;
            float yfract = pos.Y;
            float col0 = CubicHermite(InterpolationMatrix[0, 0], InterpolationMatrix[1, 0], InterpolationMatrix[2, 0], InterpolationMatrix[3, 0], xfract);
            float col1 = CubicHermite(InterpolationMatrix[0, 1], InterpolationMatrix[1, 1], InterpolationMatrix[2, 1], InterpolationMatrix[3, 1], xfract);
            float col2 = CubicHermite(InterpolationMatrix[0, 2], InterpolationMatrix[1, 2], InterpolationMatrix[2, 2], InterpolationMatrix[3, 2], xfract);
            float col3 = CubicHermite(InterpolationMatrix[0, 3], InterpolationMatrix[1, 3], InterpolationMatrix[2, 3], InterpolationMatrix[3, 3], xfract);
            float value = CubicHermite(col0, col1, col2, col3, yfract);
            return value;
        }
    }
}