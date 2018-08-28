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
        protected readonly CellComputer _mainCellComputer;
        protected readonly int _subdivision;
        protected readonly bool _ensureFlat;
        protected readonly Vector3f _noiseScale;

        protected override int QuadsPerCell => _subdivision * _subdivision;

        public BicubicTerrainMesher2i(ITerrainConfig terrainConfig, IImageSampler2i imageSampler, [ConfigInject("ensureFlat")]bool ensureFlat, [ConfigInject("subdivision")] int subdivision, [ConfigInject("noiseScale")]Vector3f noiseScale)
        : base(terrainConfig, imageSampler)
        {
            this._ensureFlat = ensureFlat;
            this._subdivision = subdivision;
            this._noiseScale = noiseScale;
            this._mainCellComputer = CreateMainCellComputer();
        }

        protected virtual CellComputer CreateMainCellComputer()
        {
            return new CellComputer(0, _subdivision, 0, _subdivision, _ensureFlat, _subdivision, pos => _imageSampler.Sample(_image, pos));
        }

        public override void AddCell(Vector2i cellInGroup)
        {
            var values = _mainCellComputer.PrepareCell(cellInGroup + _groupPosition);
            for (int ix = 0; ix < _subdivision; ix++)
            {
                for (int iy = 0; iy < _subdivision; iy++)
                {
                    var x00y00 = values[ix + 0, iy + 0];
                    var x00y05 = values[ix + 0, iy + 1];
                    var x05y00 = values[ix + 1, iy + 0];
                    var x05y05 = values[ix + 1, iy + 1];
                    _mesh.AddQuad(x00y00, x00y05, x05y00, x05y05);
                }
            }
        }

        protected class CellComputer
        {
            protected Vector3f[,] _matrixToFill;
            protected int _minX;
            protected int _maxX;
            protected int _minY;
            protected int _maxY;
            protected int _subdivision;
            protected bool _ensureFlat;
            protected float[,] _interpolationMatrix;
            protected Func<Vector2i, SampledData2i> _sample;

            public CellComputer(int minX, int maxX, int minY, int maxY, bool ensureFlat, int subdivision, Func<Vector2i, SampledData2i> sample)
            {
                this._minX = minX;
                this._maxX = maxX;
                this._minY = minY;
                this._maxY = maxY;
                this._ensureFlat = ensureFlat;
                this._subdivision = subdivision;
                this._matrixToFill = new Vector3f[subdivision + 1, subdivision + 1];
                this._interpolationMatrix = new float[4, 4];
                this._sample = sample;
            }

            public Vector3f[,] PrepareCell(Vector2i cell)
            {
                var flatValue = CellFlatMode(cell);
                if (!flatValue.HasValue)
                {
                    PrepareInterpolation(cell);
                }

                float step = 1.0f / _subdivision;
                for (int ix = _minX; ix <= _maxX; ix++)
                {
                    for (int iy = _minY; iy <= _maxY; iy++)
                    {
                        var x0 = step * ix;
                        var y0 = step * iy;
                        var x1 = step * (ix + 1);
                        var y1 = step * (iy + 1);
                        var x00y00 = new Vector3f(cell.X + x0, cell.Y + y0, flatValue ?? Interpolate(x0, y0));
                        _matrixToFill[ix, iy] = x00y00;
                    }
                }
                return _matrixToFill;
            }

            private float? CellFlatMode(Vector2i cell)
            {
                if (_ensureFlat)
                {
                    var data = _sample(cell);
                    return ((data.x0y0 == data.x0y1) && (data.x0y1 == data.x1y0) && (data.x1y0 == data.x1y1)) ? data.x0y0.RawValue : (float?)null;
                }
                else
                {
                    return null;
                }
            }


            private bool IsFlat(int ix, int iy)
            {
                return _interpolationMatrix[ix, iy] == _interpolationMatrix[ix, iy + 1]
                    && _interpolationMatrix[ix + 1, iy] == _interpolationMatrix[ix + 1, iy + 1]
                    && _interpolationMatrix[ix, iy] == _interpolationMatrix[ix + 1, iy + 1];
            }

            protected virtual void PrepareInterpolation(Vector2i cell)
            {
                var data_x0y0 = _sample(cell - Vector2i.One + new Vector2i(0, 0));
                // var data_x0y1 = _imageSampler.Sample(_image, cell - Vector2i.One + new Vector2i(0, 1));
                var data_x0y2 = _sample(cell - Vector2i.One + new Vector2i(0, 2));

                // var data_x1y0 = _imageSampler.Sample(_image, cell - Vector2i.One + new Vector2i(1, 0));
                // var data_x1y1 = _imageSampler.Sample(_image, cell - Vector2i.One + new Vector2i(1, 1));
                // var data_x1y2 = _imageSampler.Sample(_image, cell - Vector2i.One + new Vector2i(1, 2));
                var data_x2y0 = _sample(cell - Vector2i.One + new Vector2i(2, 0));
                // var data_x2y1 = _imageSampler.Sample(_image, cell - Vector2i.One + new Vector2i(2, 1));
                var data_x2y2 = _sample(cell - Vector2i.One + new Vector2i(2, 2));

                _interpolationMatrix[0, 0] = data_x0y0.x0y0.RawValue;
                _interpolationMatrix[0, 1] = data_x0y0.x0y1.RawValue;
                _interpolationMatrix[0, 2] = data_x0y2.x0y0.RawValue;
                _interpolationMatrix[0, 3] = data_x0y2.x0y1.RawValue;

                _interpolationMatrix[1, 0] = data_x0y0.x1y0.RawValue;
                _interpolationMatrix[1, 1] = data_x0y0.x1y1.RawValue;
                _interpolationMatrix[1, 2] = data_x0y2.x1y0.RawValue;
                _interpolationMatrix[1, 3] = data_x0y2.x1y1.RawValue;

                _interpolationMatrix[2, 0] = data_x2y0.x0y0.RawValue;
                _interpolationMatrix[2, 1] = data_x2y0.x0y1.RawValue;
                _interpolationMatrix[2, 2] = data_x2y2.x0y0.RawValue;
                _interpolationMatrix[2, 3] = data_x2y2.x0y1.RawValue;

                _interpolationMatrix[3, 0] = data_x2y0.x1y0.RawValue;
                _interpolationMatrix[3, 1] = data_x2y0.x1y1.RawValue;
                _interpolationMatrix[3, 2] = data_x2y2.x1y0.RawValue;
                _interpolationMatrix[3, 3] = data_x2y2.x1y1.RawValue;
            }

            private float Interpolate(float xfract, float yfract)
            {
                if (_ensureFlat)
                {
                    if (xfract == 0f && IsFlat(0, 1))
                        return _interpolationMatrix[0, 1];
                    else if (xfract == 1f && IsFlat(2, 1))
                        return _interpolationMatrix[2, 1];
                    else if (yfract == 0f && IsFlat(1, 0))
                        return _interpolationMatrix[1, 0];
                    else if (yfract == 1f && IsFlat(1, 2))
                        return _interpolationMatrix[1, 2];
                    else if (xfract == 0f && yfract == 0f && IsFlat(0, 0))
                        return _interpolationMatrix[0, 0];
                    else if (xfract == 0f && yfract == 1f && IsFlat(0, 2))
                        return _interpolationMatrix[0, 2];
                    else if (xfract == 1f && yfract == 0f && IsFlat(2, 0))
                        return _interpolationMatrix[2, 0];
                    else if (xfract == 1f && yfract == 1f && IsFlat(2, 2))
                        return _interpolationMatrix[2, 2];
                    else
                        return SampleBicubic(xfract, yfract);
                }
                else
                    return SampleBicubic(xfract, yfract);
            }

            private float SampleBicubic(float xfract, float yfract)
            {
                float col0 = CubicHermite(_interpolationMatrix[0, 0], _interpolationMatrix[1, 0], _interpolationMatrix[2, 0], _interpolationMatrix[3, 0], xfract);
                float col1 = CubicHermite(_interpolationMatrix[0, 1], _interpolationMatrix[1, 1], _interpolationMatrix[2, 1], _interpolationMatrix[3, 1], xfract);
                float col2 = CubicHermite(_interpolationMatrix[0, 2], _interpolationMatrix[1, 2], _interpolationMatrix[2, 2], _interpolationMatrix[3, 2], xfract);
                float col3 = CubicHermite(_interpolationMatrix[0, 3], _interpolationMatrix[1, 3], _interpolationMatrix[2, 3], _interpolationMatrix[3, 3], xfract);
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
}