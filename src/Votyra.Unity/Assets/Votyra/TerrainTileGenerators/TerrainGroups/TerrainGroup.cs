using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Votyra.Models;
using UnityEngine;

namespace Votyra.TerrainTileGenerators.TerrainGroups
{
    public class TerrainGroup : ITerrainGroup
    {
        public Vector2i Group { get; private set; }
        public MatrixWithOffset<ResultHeightData> Data { get; private set; }

        public TerrainGroup(Vector2i cellInGroupCount)
        {
            Data = new MatrixWithOffset<ResultHeightData>(cellInGroupCount, Vector2i.One);
        }

        public void Clear(Vector2i group)
        {
            Group = group;
        }
    }
}
