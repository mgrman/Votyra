using System;
using System.Collections.Generic;
using UnityEngine;
using Votyra.Core.Models;

namespace Votyra.Core.TerrainGenerators.TerrainMeshers.CellComputers
{
    public class BicubicInterpolator : IInterpolator
    {
        public BicubicInterpolator()
        {
            this.InterpolationMatrix = new float[4, 4];
        }

        public float[,] InterpolationMatrix { get; private set; }

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

        public float Sample(Vector2f pos)
        {
            float xfract = pos.X;
            float yfract = pos.Y;
            float col0 = Intepolate(InterpolationMatrix[0, 0], InterpolationMatrix[1, 0], InterpolationMatrix[2, 0], InterpolationMatrix[3, 0], xfract);
            float col1 = Intepolate(InterpolationMatrix[0, 1], InterpolationMatrix[1, 1], InterpolationMatrix[2, 1], InterpolationMatrix[3, 1], xfract);
            float col2 = Intepolate(InterpolationMatrix[0, 2], InterpolationMatrix[1, 2], InterpolationMatrix[2, 2], InterpolationMatrix[3, 2], xfract);
            float col3 = Intepolate(InterpolationMatrix[0, 3], InterpolationMatrix[1, 3], InterpolationMatrix[2, 3], InterpolationMatrix[3, 3], xfract);
            float value = Intepolate(col0, col1, col2, col3, yfract);
            return value;
        }

        // Monotone cubic interpolation
        // https://en.wikipedia.org/wiki/Monotone_cubic_interpolation
        private float Intepolate(float y0, float y1, float y2, float y3, float x12Rel)
        {
            // Get consecutive differences and slopes
            float dys0 = y1 - y0;
            float dys1 = y2 - y1;
            float dys2 = y3 - y2;

            // Get degree-1 coefficients
            float c1s1;
            if (dys0 * dys1 <= 0)
            {
                c1s1 = 0;
            }
            else
            {
                c1s1 = 6f / (3f / dys0 + 3f / dys1);
            }
            float c1s2;
            if (dys1 * dys2 <= 0)
            {
                c1s2 = 0;
            }
            else
            {
                c1s2 = 6f / (3f / dys1 + 3f / dys2);
            }

            // Get degree-2 and degree-3 coefficients
            float c3s1 = c1s1 + c1s2 - dys1 - dys1;
            float c2s1 = (dys1 - c1s1 - c3s1);

            // Interpolate
            var diff = x12Rel;
            var diffSq = diff * diff;
            return y1 + c1s1 * diff + c2s1 * diffSq + c3s1 * diff * diffSq;
        }
    }
}