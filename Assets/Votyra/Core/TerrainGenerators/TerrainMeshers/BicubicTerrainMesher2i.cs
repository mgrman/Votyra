using System;
using System.Collections.Generic;
using UnityEngine;
using Votyra.Core.Images;
using Votyra.Core.ImageSamplers;
using Votyra.Core.Models;
using Votyra.Core.Pooling;
using Votyra.Core.TerrainGenerators.TerrainMeshers.CellComputers;
using Votyra.Core.TerrainMeshes;

namespace Votyra.Core.TerrainGenerators.TerrainMeshers
{
    public class BicubicTerrainMesher2i : TerrainMesher2i
    {
        protected readonly bool _ensureFlat;
        protected readonly ICellComputer _mainCellComputer;
        protected readonly ICellComputer _maskCellComputer;
        protected readonly Vector3f _noiseScale;
        protected readonly int _subdivision;
        protected readonly Range2i _subdivisionValueRange;

        public BicubicTerrainMesher2i(ITerrainConfig terrainConfig, IImageSampler2i imageSampler, [ConfigInject("ensureFlat")]bool ensureFlat, [ConfigInject("subdivision")] int subdivision, [ConfigInject("noiseScale")]Vector3f noiseScale)
        : base(terrainConfig, imageSampler)
        {
            this._ensureFlat = ensureFlat;
            this._subdivision = subdivision;
            this._subdivisionValueRange = Range2i.FromMinAndMax(Vector2i.Zero, new Vector2i(_subdivision + 1, _subdivision + 1));
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
                var values = _mainCellComputer.PrepareCell(cellInGroup + _groupPosition);
                var maskValues = _maskCellComputer.PrepareCell(cellInGroup + _groupPosition);
                float step = 1.0f / _subdivision;
                for (int ix = 0; ix < _subdivision; ix++)
                {
                    for (int iy = 0; iy < _subdivision; iy++)
                    {
                        var x00 = step * (ix);
                        var y00 = step * (iy);
                        var x05 = step * (ix + 1);
                        var y05 = step * (iy + 1);
                        Vector3f? x00y00 = values[ix + 0, iy + 0];
                        Vector3f? x00y05 = values[ix + 0, iy + 1];
                        Vector3f? x05y00 = values[ix + 1, iy + 0];
                        Vector3f? x05y05 = values[ix + 1, iy + 1];
                        var x00y00Mask = maskValues[ix + 0, iy + 0];
                        var x00y05Mask = maskValues[ix + 0, iy + 1];
                        var x05y00Mask = maskValues[ix + 1, iy + 0];
                        var x05y05Mask = maskValues[ix + 1, iy + 1];

                        x00y00 = x00y00Mask.Z <= 0.5f ? x00y00 : (Vector3f?)null;
                        x00y05 = x00y05Mask.Z <= 0.5f ? x00y05 : (Vector3f?)null;
                        x05y00 = x05y00Mask.Z <= 0.5f ? x05y00 : (Vector3f?)null;
                        x05y05 = x05y05Mask.Z <= 0.5f ? x05y05 : (Vector3f?)null;

                        _mesh.AddQuad(x00y00, x00y05, x05y00, x05y05);
                    }
                }
            }
        }

        protected virtual IInterpolator CreateInterpolator()
        {
            IInterpolator interpolator = new BicubicInterpolator();
            if (_ensureFlat)
            {
                interpolator = new EnsureFlatInterpolatorDecorator(interpolator);
            }

            return interpolator;
        }

        protected virtual ICellComputer CreateMainCellComputer()
        {
            return CreateCellComputer(_subdivisionValueRange, ImageSampleHandler);
        }

        protected virtual ICellComputer CreateMaskCellComputer()
        {
            return CreateCellComputer(_subdivisionValueRange, MaskSampleHandler);
        }

        protected virtual ICellComputer CreateCellComputer(Range2i range, Func<Vector2i, SampledData2f> sample)
        {
            return new CellComputer(range, _subdivision, sample, CreateInterpolator());
        }

        protected SampledData2f ImageSampleHandler(Vector2i pos) => _imageSampler.Sample(_image, pos).ToSampSampledData2F();

        protected SampledData2f MaskSampleHandler(Vector2i pos) => _imageSampler.Sample(_mask, pos).ToSampSampledData2F();
    }
}