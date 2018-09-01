using System;
using System.Collections.Generic;
using UnityEngine;
using Votyra.Core.Images;
using Votyra.Core.ImageSamplers;
using Votyra.Core.Models;
using Votyra.Core.Pooling;
using Votyra.Core.TerrainMeshes;
using Votyra.Core.Utils;

namespace Votyra.Core.TerrainGenerators.TerrainMeshers
{
    public class BicubicTerrainMesherWithWalls2i : BicubicTerrainMesher2i
    {
        private const int PointsPerInnerCell = QuadToTriangles * 3;

        protected readonly CellComputer _minusXcomputer;
        protected readonly CellComputer _minusYcomputer;

        protected Vector3f[,] _minusXvalues;
        protected Vector3f[,] _minusYvalues;

        public BicubicTerrainMesherWithWalls2i(ITerrainConfig terrainConfig, IImageSampler2i imageSampler, [ConfigInject("ensureFlat")]bool ensureFlat, [ConfigInject("subdivision")] int subdivision, [ConfigInject("noiseScale")]Vector3f noiseScale)
            : base(terrainConfig, imageSampler, ensureFlat, subdivision, noiseScale)
        {
            this._minusXcomputer = CreateCellComputer(_subdivision, _subdivision, 0, _subdivision);
            this._minusYcomputer = CreateCellComputer(0, _subdivision, _subdivision, _subdivision);
        }

        protected override CellComputer CreateMainCellComputer()
        {
            return CreateCellComputer(0, _subdivision, 0, _subdivision);
        }

        protected CellComputer CreateCellComputer(int minX, int maxX, int minY, int maxY)
        {
            return new DualSampleCellComputer(minX, maxX, minY, maxY, _ensureFlat, _subdivision, pos => _imageSampler.Sample(_image, pos));
        }

        protected override int QuadsPerCell => base.QuadsPerCell + _subdivision + _subdivision;

        public override void AddCell(Vector2i cellInGroup)
        {
            base.AddCell(cellInGroup);

            Vector2i cell = cellInGroup + _groupPosition;
            if (cellInGroup.X == 0)
            {
                _minusXvalues = _minusXcomputer.PrepareCell(cell - new Vector2i(1, 0));
            }
            if (cellInGroup.Y == 0)
            {
                _minusYvalues = _minusYcomputer.PrepareCell(cell - new Vector2i(0, 1));
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
                var minusYx00y00 = GetInnerPointValue(cellInGroup - new Vector2i(1, 0), new Vector2i(_subdivision, iy));
                var minusYx00y05 = GetInnerPointValue(cellInGroup - new Vector2i(1, 0), new Vector2i(_subdivision, iy + 1));
                _mesh.AddWall(x00y00, x00y05, minusYx00y05, minusYx00y00);
            }
        }

        private Vector3f GetInnerPointValue(Vector2i cell, Vector2i innerPoint)
        {
            if (cell.X == -1)
            {
                return _minusXvalues[innerPoint.X, innerPoint.Y];
            }
            if (cell.Y == -1)
            {
                return _minusYvalues[innerPoint.X, innerPoint.Y];
            }
            return _mesh[GetInnerPointMeshIndex(cell, innerPoint)];
        }

        private int GetInnerPointMeshIndex(Vector2i cell, Vector2i innerPoint)
        {
            if (innerPoint.X == _subdivision && innerPoint.Y == _subdivision)
            {
                return GetInnerPointMeshIndex(cell, innerPoint - Vector2i.One) + 4;
            }
            if (innerPoint.X == _subdivision)
            {
                return GetInnerPointMeshIndex(cell, innerPoint - new Vector2i(1, 0)) + 1;
            }
            if (innerPoint.Y == _subdivision)
            {
                return GetInnerPointMeshIndex(cell, innerPoint - new Vector2i(0, 1)) + 2;
            }
            return GetMeshIndex(cell) + GetInnerPointIndex(innerPoint) * PointsPerInnerCell;
        }

        private int GetMeshIndex(Vector2i cell)
        {
            return GetCellIndex(cell) * QuadsPerCell * PointsPerInnerCell;
        }

        private int GetCellIndex(Vector2i cell)
        {
            return _cellInGroupCount.Y * cell.X + cell.Y;
        }

        private int GetInnerPointIndex(Vector2i innerPoint)
        {
            return (innerPoint.X * _subdivision + innerPoint.Y);
        }

        protected class DualSampleCellComputer : CellComputer
        {
            public DualSampleCellComputer(int minX, int maxX, int minY, int maxY, bool ensureFlat, int subdivision, Func<Vector2i, SampledData2i> sample)
                : base(minX, maxX, minY, maxY, ensureFlat, subdivision, sample)
            {
            }

            protected override void PrepareInterpolation(Vector2i cell)
            {
                var data_x0y0 = _sample(cell - Vector2i.One + new Vector2i(0, 0));
                var data_x0y1 = _sample(cell - Vector2i.One + new Vector2i(0, 1));
                var data_x0y2 = _sample(cell - Vector2i.One + new Vector2i(0, 2));
                var data_x1y0 = _sample(cell - Vector2i.One + new Vector2i(1, 0));
                var data_x1y1 = _sample(cell - Vector2i.One + new Vector2i(1, 1));
                var data_x1y2 = _sample(cell - Vector2i.One + new Vector2i(1, 2));
                var data_x2y0 = _sample(cell - Vector2i.One + new Vector2i(2, 0));
                var data_x2y1 = _sample(cell - Vector2i.One + new Vector2i(2, 1));
                var data_x2y2 = _sample(cell - Vector2i.One + new Vector2i(2, 2));

                _interpolationMatrix[0, 0] = data_x0y0.x0y0.RawValue;
                _interpolationMatrix[0, 1] = (data_x0y0.x0y1.RawValue + data_x0y1.x0y0.RawValue) / 2;
                _interpolationMatrix[0, 2] = (data_x0y1.x0y1.RawValue + data_x0y2.x0y0.RawValue) / 2;
                _interpolationMatrix[0, 3] = data_x0y2.x0y1.RawValue;

                _interpolationMatrix[1, 0] = (data_x1y0.x0y0.RawValue + data_x0y0.x1y0.RawValue) / 2;
                _interpolationMatrix[1, 1] = data_x1y1.x0y0.RawValue;
                _interpolationMatrix[1, 2] = data_x1y1.x0y1.RawValue;
                _interpolationMatrix[1, 3] = (data_x1y2.x0y1.RawValue + data_x0y2.x1y1.RawValue) / 2;

                _interpolationMatrix[2, 0] = (data_x1y0.x1y0.RawValue + data_x2y0.x0y0.RawValue) / 2;
                _interpolationMatrix[2, 1] = data_x1y1.x1y0.RawValue;
                _interpolationMatrix[2, 2] = data_x1y1.x1y1.RawValue;
                _interpolationMatrix[2, 3] = (data_x1y2.x1y1.RawValue + data_x2y2.x0y1.RawValue) / 2;

                _interpolationMatrix[3, 0] = data_x2y0.x1y0.RawValue;
                _interpolationMatrix[3, 1] = (data_x2y0.x0y1.RawValue + data_x2y1.x0y0.RawValue) / 2;
                _interpolationMatrix[3, 2] = (data_x2y1.x0y1.RawValue + data_x2y2.x0y0.RawValue) / 2;
                _interpolationMatrix[3, 3] = data_x2y2.x1y1.RawValue;
            }
        }
    }
}