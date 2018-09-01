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
        protected readonly bool _ensureFlat;
        protected readonly CellComputer _mainCellComputer;
        protected readonly CellComputer _maskCellComputer;
        protected readonly Vector3f _noiseScale;
        protected readonly int _subdivision;

        public BicubicTerrainMesher2i(ITerrainConfig terrainConfig, IImageSampler2i imageSampler, [ConfigInject("ensureFlat")]bool ensureFlat, [ConfigInject("subdivision")] int subdivision, [ConfigInject("noiseScale")]Vector3f noiseScale)
        : base(terrainConfig, imageSampler)
        {
            this._ensureFlat = ensureFlat;
            this._subdivision = subdivision;
            this._noiseScale = noiseScale;
            this._mainCellComputer = CreateMainCellComputer();
            this._maskCellComputer = CreateMaskCellComputer();
        }

        protected override int QuadsPerCell => _subdivision * _subdivision;

        public override void AddCell(Vector2i cellInGroup)
        {
            var mask = _imageSampler.Sample(_mask, cellInGroup + _groupPosition);
            if (mask.GetHoleCount() == 4)
            {
                return;
            }
            else
            {
                if (mask.GetHoleCount() == 1)
                {
                }
                var values = _mainCellComputer.PrepareCell(cellInGroup + _groupPosition);

                var maskValues = _maskCellComputer.PrepareCell(cellInGroup + _groupPosition);

                float step = 1.0f / _subdivision;
                var center = _subdivision / 2;

                float dStep = step / 10f;
                for (int ix = 0; ix < _subdivision; ix++)
                {
                    for (int iy = 0; iy < _subdivision; iy++)
                    {
                        if (mask.x0y0.IsHole() && ix < center && iy < center)
                        {
                            continue;
                        }
                        if (mask.x1y0.IsHole() && ix >= center && iy < center)
                        {
                            continue;
                        }
                        if (mask.x0y1.IsHole() && ix < center && iy >= center)
                        {
                            continue;
                        }
                        if (mask.x1y1.IsHole() && ix >= center && iy >= center)
                        {
                            continue;
                        }
                        var x00 = step * (ix);
                        var y00 = step * (iy);
                        var x05 = step * (ix + 1);
                        var y05 = step * (iy + 1);
                        var x00y00 = values[ix + 0, iy + 0];
                        var x00y05 = values[ix + 0, iy + 1];
                        var x05y00 = values[ix + 1, iy + 0];
                        var x05y05 = values[ix + 1, iy + 1];

                        x00y00 = (x00y00.XY + GetDif(x00, y00, dStep, ix, iy)).ToVector3f(x00y00.Z);
                        x00y05 = (x00y05.XY + GetDif(x00, y05, dStep, ix, iy + 1)).ToVector3f(x00y05.Z);
                        x05y00 = (x05y00.XY + GetDif(x05, y00, dStep, ix + 1, iy)).ToVector3f(x05y00.Z);
                        x05y05 = (x05y05.XY + GetDif(x05, y05, dStep, ix + 1, iy + 1)).ToVector3f(x05y05.Z);

                        _mesh.AddQuad(x00y00, x00y05, x05y00, x05y05);
                    }
                }
            }
        }

        protected virtual CellComputer CreateMainCellComputer()
        {
            return new CellComputer(0, _subdivision, 0, _subdivision, _ensureFlat, _subdivision, pos => _imageSampler.Sample(_image, pos));
        }

        protected virtual CellComputer CreateMaskCellComputer()
        {
            return new CellComputer(0, _subdivision, 0, _subdivision, _ensureFlat, _subdivision, pos =>
            {
                var mask = _imageSampler.Sample(_mask, pos);
                return new SampledData2i(mask.x0y0.IsHole() ? 1 : 0, mask.x0y1.IsHole() ? 1 : 0, mask.x1y0.IsHole() ? 1 : 0, mask.x1y1.IsHole() ? 1 : 0);
            });
        }

        private Vector2f GetDif(float x, float y, float dStep, int ix, int iy)
        {
            float stepMinX = ix == 0 ? 0 : dStep;
            float stepPlusX = ix == _subdivision ? 0 : dStep;
            float stepMinY = iy == 0 ? 0 : dStep;
            float stepPlusY = iy == _subdivision ? 0 : dStep;

            var mXMin = _maskCellComputer.Interpolate(x - stepMinX, y);
            var mXPlus = _maskCellComputer.Interpolate(x + stepPlusX, y);
            var mYMin = _maskCellComputer.Interpolate(x, y - stepMinY);
            var mYPlus = _maskCellComputer.Interpolate(x, y + stepPlusY);

            var dX = (mXMin - mXPlus) / (stepMinX + stepPlusX);
            var dY = (mYMin - mYPlus) / (stepMinY + stepPlusY);

            var dNorm = new Vector2f(dX, dY).Normalized;
            var maskOffset = (_maskCellComputer.Interpolate(x, y) * 0.5f);
            var result = dNorm * maskOffset;

            return result;
        }

        protected class CellComputer
        {
            protected bool _ensureFlat;
            protected float[,] _interpolationMatrix;
            protected Vector3f[,] _matrixToFill;
            protected int _maxX;
            protected int _maxY;
            protected int _minX;
            protected int _minY;
            protected Func<Vector2i, SampledData2i> _sample;
            protected int _subdivision;

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

            public float Interpolate(float xfract, float yfract)
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

            // private Vector2f CatmulRomSpline(Vector2f a, Vector2f b, Vector2f c, Vector2f d, float v)
            // {
            //     var pX = (1.0f / 2.0f) * ((b.X * 2.0f) + (-a.X + c.X) * v + ((a.X * 2.0f) - (b.X * 5.0f) + (c.X * 4.0f) - d.X) * (v * v) + (-a.X + (b.X * 3.0f) - (c.X * 3.0f) + d.X) * (v * v * v));
            //     var pY = (1.0f / 2.0f) * ((b.Y * 2.0f) + (-a.Y + c.Y) * v + ((a.Y * 2.0f) - (b.Y * 5.0f) + (c.Y * 4.0f) - d.Y) * (v * v) + (-a.Y + (b.Y * 3.0f) - (c.Y * 3.0f) + d.Y) * (v * v * v));

            //     return new Vector2f(pX, pY);
            // }

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

            private bool IsFlat(int ix, int iy)
            {
                return _interpolationMatrix[ix, iy] == _interpolationMatrix[ix, iy + 1]
                    && _interpolationMatrix[ix + 1, iy] == _interpolationMatrix[ix + 1, iy + 1]
                    && _interpolationMatrix[ix, iy] == _interpolationMatrix[ix + 1, iy + 1];
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
        }
    }
}