using System;
using System.Collections.Generic;
using UnityEngine;
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

        protected float? flatValue;

        protected readonly int _subdivision;

        protected readonly bool _ensureFlat;
        protected readonly Vector3f _noiseScale;

        public BicubicTerrainMesher2i(ITerrainConfig terrainConfig, IImageSampler2i imageSampler, [ConfigInject("ensureFlat")]bool ensureFlat, [ConfigInject("subdivision")] int subdivision, [ConfigInject("noiseScale")]Vector3f noiseScale)
        : base(terrainConfig, imageSampler)
        {
            this._ensureFlat = ensureFlat;
            this._subdivision = subdivision;
            this._noiseScale = noiseScale;
        }

        protected override int QuadsPerCell => _subdivision * _subdivision;

        public override void AddCell(Vector2i cellInGroup)
        {
            Vector2i cell = cellInGroup + _groupPosition;
            Vector2i position = _groupPosition + cellInGroup;

            SetCellFlatMode(cell);
            if (!flatValue.HasValue)
            {
                PrepareInterpolation(cell);
            }

            float step = 1.0f / _subdivision;
            for (int ix = 0; ix < _subdivision; ix++)
            {
                for (int iy = 0; iy < _subdivision; iy++)
                {
                    var x0 = step * ix;
                    var y0 = step * iy;
                    var x1 = step * (ix + 1);
                    var y1 = step * (iy + 1);
                    var x00y00 = new Vector3f(position.X + x0, position.Y + y0, Interpolate(x0, y0, cell));
                    var x00y05 = new Vector3f(position.X + x0, position.Y + y1, Interpolate(x0, y1, cell));
                    var x05y00 = new Vector3f(position.X + x1, position.Y + y0, Interpolate(x1, y0, cell));
                    var x05y05 = new Vector3f(position.X + x1, position.Y + y1, Interpolate(x1, y1, cell));
                    _mesh.AddQuad(x00y00, x00y05, x05y00, x05y05);
                }
            }
        }

        private void SetCellFlatMode(Vector2i cell)
        {
            if (_ensureFlat)
            {
                var data = _imageSampler.Sample(_image, cell);
                flatValue = ((data.x0y0 == data.x0y1) && (data.x0y1 == data.x1y0) && (data.x1y0 == data.x1y1)) ? data.x0y0.RawValue : (float?)null;
            }
            else
            {
                flatValue = null;
            }
        }


        private bool IsFlat(int ix, int iy)
        {
            return values[ix, iy] == values[ix, iy + 1]
            && values[ix + 1, iy] == values[ix + 1, iy + 1]
            && values[ix, iy] == values[ix + 1, iy + 1];
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

        private float Interpolate(float xfract, float yfract, Vector2i cell)
        {
            if (_ensureFlat)
            {
                if (flatValue.HasValue)
                    return flatValue.Value;
                else if (xfract == 0f && IsFlat(0, 1))
                    return values[0, 1];
                else if (xfract == 1f && IsFlat(2, 1))
                    return values[2, 1];
                else if (yfract == 0f && IsFlat(1, 0))
                    return values[1, 0];
                else if (yfract == 1f && IsFlat(1, 2))
                    return values[1, 2];
                else if (xfract == 0f && yfract == 0f && IsFlat(0, 0))
                    return values[0, 0];
                else if (xfract == 0f && yfract == 1f && IsFlat(0, 2))
                    return values[0, 2];
                else if (xfract == 1f && yfract == 0f && IsFlat(2, 0))
                    return values[2, 0];
                else if (xfract == 1f && yfract == 1f && IsFlat(2, 2))
                    return values[2, 2];
                else
                    return SampleBicubic(xfract, yfract, cell);
            }
            else
                return SampleBicubic(xfract, yfract, cell);
        }

        private float SampleBicubic(float xfract, float yfract, Vector2i cell)
        {
            float col0 = CubicHermite(values[0, 0], values[1, 0], values[2, 0], values[3, 0], xfract);
            float col1 = CubicHermite(values[0, 1], values[1, 1], values[2, 1], values[3, 1], xfract);
            float col2 = CubicHermite(values[0, 2], values[1, 2], values[2, 2], values[3, 2], xfract);
            float col3 = CubicHermite(values[0, 3], values[1, 3], values[2, 3], values[3, 3], xfract);
            float value = CubicHermite(col0, col1, col2, col3, yfract);

            if (_noiseScale.Z != 0f)
            {
                value += Mathf.PerlinNoise((cell.X + xfract) * _noiseScale.X, (cell.Y + yfract) * _noiseScale.Y) * _noiseScale.Z;
            }
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