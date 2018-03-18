using UnityEngine;
using Votyra.Core.Models;

namespace Votyra.Core.Images
{
    public class TerrainConfig : ITerrainConfig
    {
        public TerrainConfig(Vector3i cellInGroupCount, bool flipTriangles, bool drawBounds, bool async, Material material, Material materialWalls)
        {
            CellInGroupCount = cellInGroupCount;
            FlipTriangles = flipTriangles;
            DrawBounds = drawBounds;
            Async = async;
            Material = material;
            MaterialWalls = materialWalls;
        }

        public Vector3i CellInGroupCount { get; }
        public bool FlipTriangles { get; }
        public bool DrawBounds { get; }
        public bool Async { get; }
        public Material Material { get; }
        public Material MaterialWalls { get; }


        public static bool operator ==(TerrainConfig a, TerrainConfig b)
        {
            return a?.Equals(b) ?? b?.Equals(a) ?? true;
        }

        public static bool operator !=(TerrainConfig a, TerrainConfig b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            var that = obj as TerrainConfig;

            return this.CellInGroupCount == that.CellInGroupCount
                && this.FlipTriangles == that.FlipTriangles
                && this.DrawBounds == that.DrawBounds
                && this.Async == that.Async
                && this.Material == that.Material
                && this.MaterialWalls == that.MaterialWalls;
        }

        public override int GetHashCode()
        {
            return this.CellInGroupCount.GetHashCode()
                + this.FlipTriangles.GetHashCode() * 3
                + this.DrawBounds.GetHashCode() * 7
                + this.Async.GetHashCode() * 13
                + this.Material.GetHashCode() * 23
                + this.MaterialWalls.GetHashCode() - 3;
        }
    }
}