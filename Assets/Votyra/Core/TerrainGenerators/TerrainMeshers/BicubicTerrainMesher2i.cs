using System;
using System.Collections.Generic;
using Votyra.Core.Images;
using Votyra.Core.ImageSamplers;
using Votyra.Core.Models;
using Votyra.Core.Pooling;
using Votyra.Core.TerrainMeshes;

namespace Votyra.Core.TerrainGenerators.TerrainMeshers
{
    public class BicubicTerrainMesher2i : TerrainMesher2i
    {
        protected readonly float[,] values = new float[4, 4];

        protected const int subdivision = 2;

        public BicubicTerrainMesher2i(ITerrainConfig terrainConfig, IImageSampler2i imageSampler)
        : base(terrainConfig, imageSampler)
        {
        }

        protected override int QuadsPerCell => subdivision * subdivision;

        public override void AddCell(Vector2i cellInGroup)
        {
            Vector2i cell = cellInGroup + _groupPosition;

            Vector2i position = _groupPosition + cellInGroup;

            PrepareInterpolation(cell);


            // var data = _imageSampler.Sample(_image, cell);
            // var maskData = _imageSampler.Sample(_mask, cell);

            float step = 1.0f / subdivision;

            for (int ix = 0; ix < subdivision; ix++)
            {
                for (int iy = 0; iy < subdivision; iy++)
                {
                    var x0 = step * ix;
                    var y0 = step * iy;
                    var x1 = step * (ix + 1);
                    var y1 = step * (iy + 1);
                    var x00y00 = new Vector3f(position.X + x0, position.Y + y0, Interpolate(x0, y0));
                    var x00y05 = new Vector3f(position.X + x0, position.Y + y1, Interpolate(x0, y1));
                    var x05y00 = new Vector3f(position.X + x1, position.Y + y0, Interpolate(x1, y0));
                    var x05y05 = new Vector3f(position.X + x1, position.Y + y1, Interpolate(x1, y1));
                    _mesh.AddQuad(x00y00, x00y05, x05y00, x05y05);
                }
            }

            // var x00y00 = new Vector3f(position.X + 0.0f, position.Y + 0.0f, Interpolate(0.0f, 0.0f));
            // var x00y10 = new Vector3f(position.X + 0.0f, position.Y + 1.0f, Interpolate(0.0f, 1.0f));
            // var x10y00 = new Vector3f(position.X + 1.0f, position.Y + 0.0f, Interpolate(1.0f, 0.0f));
            // var x10y10 = new Vector3f(position.X + 1.0f, position.Y + 1.0f, Interpolate(1.0f, 1.0f));
            // var x05y00 = new Vector3f(position.X + 0.5f, position.Y + 0.0f, Interpolate(0.5f, 0.0f));
            // var x05y10 = new Vector3f(position.X + 0.5f, position.Y + 1.0f, Interpolate(0.5f, 1.0f));
            // var x00y05 = new Vector3f(position.X + 0.0f, position.Y + 0.5f, Interpolate(0.0f, 0.5f));
            // var x10y05 = new Vector3f(position.X + 1.0f, position.Y + 0.5f, Interpolate(1.0f, 0.5f));
            // var x05y05 = new Vector3f(position.X + 0.5f, position.Y + 0.5f, Interpolate(0.5f, 0.5f));

            // _mesh.AddQuad(x00y00, x00y05, x05y00, x05y05);
            // _mesh.AddQuad(x05y00, x05y05, x10y00, x10y05);
            // _mesh.AddQuad(x00y05, x00y10, x05y05, x05y10);
            // _mesh.AddQuad(x05y05, x05y10, x10y05, x10y10);
        }

        private void PrepareInterpolation(Vector2i cell)
        {
            var data_x0y0 = _imageSampler.Sample(_image, cell - Vector2i.One + new Vector2i(0, 0));
            // var data_x0y1 = _imageSampler.Sample(_image, cell - Vector2i.One + new Vector2i(0, 1));
            var data_x0y2 = _imageSampler.Sample(_image, cell - Vector2i.One + new Vector2i(0, 2));

            // var data_x1y0 = _imageSampler.Sample(_image, cell - Vector2i.One + new Vector2i(1, 0));
            // var data_x1y1 = _imageSampler.Sample(_image, cell - Vector2i.One + new Vector2i(1, 1));
            // var data_x1y2 = _imageSampler.Sample(_image, cell - Vector2i.One + new Vector2i(1, 2));
            var data_x2y0 = _imageSampler.Sample(_image, cell - Vector2i.One + new Vector2i(2, 0));
            // var data_x2y1 = _imageSampler.Sample(_image, cell - Vector2i.One + new Vector2i(2, 1));
            var data_x2y2 = _imageSampler.Sample(_image, cell - Vector2i.One + new Vector2i(2, 2));

            values[0, 0] = data_x0y0.x0y0.RawValue;
            values[0, 1] = data_x0y0.x0y1.RawValue;
            values[0, 2] = data_x0y2.x0y0.RawValue;
            values[0, 3] = data_x0y2.x0y1.RawValue;

            values[1, 0] = data_x0y0.x1y0.RawValue;
            values[1, 1] = data_x0y0.x1y1.RawValue;
            values[1, 2] = data_x0y2.x1y0.RawValue;
            values[1, 3] = data_x0y2.x1y1.RawValue;

            values[2, 0] = data_x2y0.x0y0.RawValue;
            values[2, 1] = data_x2y0.x0y1.RawValue;
            values[2, 2] = data_x2y2.x0y0.RawValue;
            values[2, 3] = data_x2y2.x0y1.RawValue;

            values[3, 0] = data_x2y0.x1y0.RawValue;
            values[3, 1] = data_x2y0.x1y1.RawValue;
            values[3, 2] = data_x2y2.x1y0.RawValue;
            values[3, 3] = data_x2y2.x1y1.RawValue;
        }

        private float Interpolate(float xfract, float yfract)
        {
            if (xfract == 0f && yfract == 0f)
                return values[1, 1];
            else if (xfract == 0f && yfract == 1f)
                return values[1, 2];
            else if (xfract == 1f && yfract == 0f)
                return values[2, 1];
            else if (xfract == 1f && yfract == 1f)
                return values[2, 2];
            else
                return SampleBicubic(xfract, yfract);
        }

        private float SampleBicubic(float xfract, float yfract)
        {
            float col0 = CubicHermite(values[0, 0], values[1, 0], values[2, 0], values[3, 0], xfract);
            float col1 = CubicHermite(values[0, 1], values[1, 1], values[2, 1], values[3, 1], xfract);
            float col2 = CubicHermite(values[0, 2], values[1, 2], values[2, 2], values[3, 2], xfract);
            float col3 = CubicHermite(values[0, 3], values[1, 3], values[2, 3], values[3, 3], xfract);
            float value = CubicHermite(col0, col1, col2, col3, yfract);
            return value;
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
    }
}