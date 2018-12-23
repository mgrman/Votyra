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
    public class WallsVertexPostProcessor : ITerrainVertexPostProcessor
    {
        public readonly float _wallSquishFactor;

        public WallsVertexPostProcessor([ConfigInject("wallSquishFactor")] float wallSquishFactor)
        {
            _wallSquishFactor = wallSquishFactor;
        }

        public Vector3f PostProcessVertex(Vector3f position)
        {
            var expandedCellIndex = position.XY - ((position.XY / 2f).Floor() * 2f);


            var posX = position.X + ((expandedCellIndex.X < 1 ? expandedCellIndex.X : (2 - expandedCellIndex.X)) - 0.5f) * _wallSquishFactor;
            var posY = position.Y + ((expandedCellIndex.Y < 1 ? expandedCellIndex.Y : (2 - expandedCellIndex.Y)) - 0.5f) * _wallSquishFactor;
            return new Vector3f(posX, posY, position.Z);
        }
    }
}