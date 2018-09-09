using System;
using System.Collections.Generic;
using UnityEngine;
using Votyra.Core.Images;
using Votyra.Core.ImageSamplers;
using Votyra.Core.Models;
using Votyra.Core.Pooling;
using Votyra.Core.TerrainMeshes;
using Votyra.Core.Utils;
using Votyra.Core.TerrainGenerators.TerrainMeshers.CellComputers;

namespace Votyra.Core.TerrainGenerators.TerrainMeshers
{
    public class BicubicTerrainMesherWithWalls2i : BicubicTerrainMesher2i
    {
        protected readonly ICellComputer _minusXcomputer;
        protected readonly ICellComputer _minusYcomputer;
        protected readonly ICellComputer _minusXMaskComputer;
        protected readonly ICellComputer _minusYMaskComputer;
        protected Vector2i _cellInGroup;
        protected Vector2i _minusXcellInGroup;
        protected Vector2i _minusYcellInGroup;
        protected Vector3f[,] _values;
        protected Vector3f[,] _minusXvalues;
        protected Vector3f[,] _minusYvalues;
        protected Vector3f[,] _mask;
        protected Vector3f[,] _minusXmask;
        protected Vector3f[,] _minusYmask;
        protected bool _holeDetected;
        private const int PointsPerInnerCell = QuadToTriangles * 3;

        public BicubicTerrainMesherWithWalls2i(ITerrainConfig terrainConfig, IImageSampler2i imageSampler, [ConfigInject("ensureFlat")]bool ensureFlat, [ConfigInject("subdivision")] int subdivision, [ConfigInject("noiseScale")]Vector3f noiseScale)
            : base(terrainConfig, imageSampler, ensureFlat, subdivision, noiseScale)
        {
            this._minusXcomputer = CreateCellComputer(Range2i.FromMinAndMax(new Vector2i(_subdivision, 0), new Vector2i(_subdivision + 1, _subdivision + 1)), ImageSampleHandler);
            this._minusYcomputer = CreateCellComputer(Range2i.FromMinAndMax(new Vector2i(0, _subdivision), new Vector2i(_subdivision + 1, _subdivision + 1)), ImageSampleHandler);
            this._minusXMaskComputer = CreateCellComputer(Range2i.FromMinAndMax(new Vector2i(_subdivision, 0), new Vector2i(_subdivision + 1, _subdivision + 1)), MaskSampleHandler);
            this._minusYMaskComputer = CreateCellComputer(Range2i.FromMinAndMax(new Vector2i(0, _subdivision), new Vector2i(_subdivision + 1, _subdivision + 1)), MaskSampleHandler);
        }

        protected override int QuadsPerCell => base.QuadsPerCell + _subdivision + _subdivision;

        public override void Initialize(IImage2i image, IMask2e mask)
        {
            base.Initialize(image, mask);
            _holeDetected = false;
        }

        protected override void AddCellInner(Vector2i cellInGroup, Vector3f[,] values, Vector3f[,] maskValues)
        {
            base.AddCellInner(cellInGroup, values, maskValues);
            _holeDetected = _holeDetected || IsHoleDetected(cellInGroup);

            Vector2i cell = cellInGroup + _groupPosition;
            _values = values;
            _mask = maskValues;

            _cellInGroup = cellInGroup;
            _minusXcellInGroup = new Vector2i(-1, 0);
            _minusYcellInGroup = new Vector2i(0, -1);
            if (cellInGroup.X == 0 || _holeDetected)
            {
                _minusXvalues = _minusXcomputer.PrepareCell(cell - new Vector2i(1, 0));
                _minusXmask = _minusXMaskComputer.PrepareCell(cell - new Vector2i(1, 0));
            }
            if (cellInGroup.Y == 0 || _holeDetected)
            {
                _minusYvalues = _minusYcomputer.PrepareCell(cell - new Vector2i(0, 1));
                _minusYmask = _minusYMaskComputer.PrepareCell(cell - new Vector2i(0, 1));
            }
            Vector2i position = _groupPosition + cellInGroup;

            for (int ix = 0; ix < _subdivision; ix++)
            {
                var x00y00 = GetInnerPointValue(cellInGroup, new Vector2i(ix, 0));
                var x05y00 = GetInnerPointValue(cellInGroup, new Vector2i(ix + 1, 0));
                var minusYx00y00 = GetInnerPointValue(cellInGroup - new Vector2i(0, 1), new Vector2i(ix, _subdivision));
                var minusYx05y00 = GetInnerPointValue(cellInGroup - new Vector2i(0, 1), new Vector2i(ix + 1, _subdivision));
                _mesh.AddWall(x05y00, x00y00, minusYx00y00, minusYx05y00);
            }
            for (int iy = 0; iy < _subdivision; iy++)
            {
                var x00y00 = GetInnerPointValue(cellInGroup, new Vector2i(0, iy));
                var x00y05 = GetInnerPointValue(cellInGroup, new Vector2i(0, iy + 1));
                var minusXx00y00 = GetInnerPointValue(cellInGroup - new Vector2i(1, 0), new Vector2i(_subdivision, iy));
                var minusXx00y05 = GetInnerPointValue(cellInGroup - new Vector2i(1, 0), new Vector2i(_subdivision, iy + 1));
                _mesh.AddWall(x00y00, x00y05, minusXx00y05, minusXx00y00);
            }
        }

        protected override IInterpolator CreateInterpolator()
        {
            IInterpolator interpolator = new BicubicDualSampleInterpolator();
            if (_ensureFlat)
            {
                interpolator = new EnsureFlatInterpolatorDecorator(interpolator);
            }

            return interpolator;
        }

        private Vector3f? GetInnerPointValue(Vector2i cellInGroup, Vector2i innerPoint)
        {
            //TODO problem is here that when hole is in group the approach of getting previous values using mesh no longer works.
            // possible solution is to use real values if hole is detected, therefore if no hole in group the faster approach is used.
            // another solution is to keep a sliding buffer of potential values to use and remember the values used in previous cells (possible problem with the cellComputer returning cache matrix (it is always reused and the contents in not readonly))

            if (_holeDetected)
            {
                if (cellInGroup.Y == _cellInGroup.Y - 1)
                {
                    return _minusYmask[innerPoint.X, innerPoint.Y].Z < 0.5f ? _minusYvalues[innerPoint.X, innerPoint.Y] : (Vector3f?)null;
                }
                if (cellInGroup.X == _cellInGroup.X - 1)
                {
                    return _minusXmask[innerPoint.X, innerPoint.Y].Z < 0.5f ? _minusXvalues[innerPoint.X, innerPoint.Y] : (Vector3f?)null;
                }
                return _mask[innerPoint.X, innerPoint.Y].Z < 0.5f ? _values[innerPoint.X, innerPoint.Y] : (Vector3f?)null;
            }
            else
            {
                if (cellInGroup.Y == -1)
                {
                    return _minusYmask[innerPoint.X, innerPoint.Y].Z < 0.5f ? _minusYvalues[innerPoint.X, innerPoint.Y] : (Vector3f?)null;
                }
                if (cellInGroup.X == -1)
                {
                    return _minusXmask[innerPoint.X, innerPoint.Y].Z < 0.5f ? _minusXvalues[innerPoint.X, innerPoint.Y] : (Vector3f?)null;
                }
                return _mesh[GetInnerPointMeshIndex(cellInGroup, innerPoint)];
            }
        }

        private int GetInnerPointMeshIndex(Vector2i cellInGroup, Vector2i innerPoint)
        {
            if (innerPoint.X == _subdivision && innerPoint.Y == _subdivision)
            {
                return GetInnerPointMeshIndex(cellInGroup, innerPoint - Vector2i.One) + 4;
            }
            if (innerPoint.X == _subdivision)
            {
                return GetInnerPointMeshIndex(cellInGroup, innerPoint - new Vector2i(1, 0)) + 1;
            }
            if (innerPoint.Y == _subdivision)
            {
                return GetInnerPointMeshIndex(cellInGroup, innerPoint - new Vector2i(0, 1)) + 2;
            }
            return GetMeshIndex(cellInGroup) + GetInnerPointIndex(innerPoint) * PointsPerInnerCell;
        }

        private int GetMeshIndex(Vector2i cellInGroup)
        {
            return GetCellIndex(cellInGroup) * QuadsPerCell * PointsPerInnerCell;
        }

        private int GetCellIndex(Vector2i cellInGroup)
        {
            return _cellInGroupCount.Y * cellInGroup.X + cellInGroup.Y;
        }

        private int GetInnerPointIndex(Vector2i innerPoint)
        {
            return (innerPoint.X * _subdivision + innerPoint.Y);
        }

        private bool IsHoleDetected(Vector2i cellInGroup)
        {
            if (cellInGroup.Y == _cellInGroupCount.Y - 1)
            {
                cellInGroup = new Vector2i(cellInGroup.X + 1, 0);
            }
            else
            {
                cellInGroup = new Vector2i(cellInGroup.X, cellInGroup.Y + 1);
            }
            var expectedTriangleCount = GetMeshIndex(cellInGroup) - (_subdivision + _subdivision) * PointsPerInnerCell;

            return _mesh.TriangleCount != expectedTriangleCount / 3;
        }
    }
}