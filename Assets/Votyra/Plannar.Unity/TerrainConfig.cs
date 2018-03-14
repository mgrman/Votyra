using UnityEngine;
using Votyra.Core.Models;

namespace Votyra.Plannar.Images
{
    public class TerrainConfig : ITerrainConfig
    {
        public TerrainConfig(Vector3i imageSize, object initialData, Vector3f initialDataScale, Vector2i cellInGroupCount, bool flipTriangles, bool drawBounds, bool async, Material material, Material materialWalls)
        {
            ImageSize = imageSize;
            InitialData = initialData;
            InitialDataScale = initialDataScale;
            CellInGroupCount = cellInGroupCount;
            FlipTriangles = flipTriangles;
            DrawBounds = drawBounds;
            Async = async;
            Material = material;
            MaterialWalls = materialWalls;
        }

        public Vector3i ImageSize { get; }
        public object InitialData { get; }
        public Vector3f InitialDataScale { get; }
        public Vector2i CellInGroupCount { get; }
        public bool FlipTriangles { get; }
        public bool DrawBounds { get; }
        public bool Async { get; }
        public Material Material { get; }
        public Material MaterialWalls { get; }
    }
}