using Votyra.Core.Models;

namespace Votyra.Core.Images
{
    public class TerrainConfig : ITerrainConfig
    {
        public TerrainConfig([ConfigInject("cellInGroupCount")] Vector3i cellInGroupCount, [ConfigInject("flipTriangles")] bool flipTriangles, [ConfigInject("drawBounds")] bool drawBounds, [ConfigInject("async")] bool async, [ConfigInject("useMeshCollider")] bool useMeshCollider)
        {
            CellInGroupCount = cellInGroupCount;
            FlipTriangles = flipTriangles;
            DrawBounds = drawBounds;
#if UNITY_WEBGL && !UNITY_EDITOR
            Async = false;
#else
            Async = async;
            UseMeshCollider = useMeshCollider;
#endif
        }

        public Vector3i CellInGroupCount { get; }
        public bool FlipTriangles { get; }
        public bool UseMeshCollider { get; }
        public bool DrawBounds { get; }
        public bool Async { get; }

        public static bool operator ==(TerrainConfig a, TerrainConfig b) => a?.Equals(b) ?? b?.Equals(a) ?? true;

        public static bool operator !=(TerrainConfig a, TerrainConfig b) => !(a == b);

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;
            var that = obj as TerrainConfig;

            return CellInGroupCount == that.CellInGroupCount && FlipTriangles == that.FlipTriangles && DrawBounds == that.DrawBounds && Async == that.Async && UseMeshCollider == that.UseMeshCollider;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return CellInGroupCount.GetHashCode() + FlipTriangles.GetHashCode() * 3 + DrawBounds.GetHashCode() * 7 + Async.GetHashCode() * 13 + UseMeshCollider.GetHashCode() * 13;
            }
        }
    }
}