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
    public class CellComputer : ICellComputer
    {
        protected readonly IInterpolator _interpolator;
        protected readonly Vector3f[,] _matrixToFill;
        protected readonly Range2i _range;
        protected readonly Func<Vector2i, SampledData2f> _sample;
        protected readonly int _subdivision;

        public CellComputer(Range2i range, int subdivision, Func<Vector2i, SampledData2f> sample, IInterpolator interpolator)
        {
            this._range = range;
            this._subdivision = subdivision;
            this._matrixToFill = new Vector3f[subdivision + 1, subdivision + 1];
            this._interpolator = interpolator;
            this._sample = sample;
        }

        public Vector3f[,] PrepareCell(Vector2i cell)
        {
            _interpolator.PrepareInterpolation(cell, _sample);

            float step = 1.0f / _subdivision;
            for (int ix = _range.Min.X; ix < _range.Max.X; ix++)
            {
                for (int iy = _range.Min.Y; iy < _range.Max.Y; iy++)
                {
                    var x0 = step * ix;
                    var y0 = step * iy;
                    var pos = new Vector2f(x0, y0);
                    _matrixToFill[ix, iy] = (pos + cell).ToVector3f(_interpolator.Sample(pos)); ;
                }
            }
            return _matrixToFill;
        }
    }
}