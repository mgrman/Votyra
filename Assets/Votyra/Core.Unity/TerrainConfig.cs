using Votyra.Core.Models;

namespace Votyra.Core.Images
{
    public class TerrainConfig : ITerrainConfig
    {
        public TerrainConfig([ConfigInject("cellInGroupCount")] Vector3i cellInGroupCount, [ConfigInject("drawBounds")] bool drawBounds, [ConfigInject("async")] bool async)
        {
            CellInGroupCount = cellInGroupCount;
            DrawBounds = drawBounds;
#if UNITY_WEBGL && !UNITY_EDITOR
            Async = false;
#else
            Async = async;
#endif
        }

        public Vector3i CellInGroupCount { get; }
        
        public bool DrawBounds { get; }
        
        public bool Async { get; }

        public static bool operator ==(TerrainConfig a, TerrainConfig b) => a?.Equals(b) ?? b?.Equals(a) ?? true;

        public static bool operator !=(TerrainConfig a, TerrainConfig b) => !(a == b);

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;
            var that = obj as TerrainConfig;

            return CellInGroupCount == that.CellInGroupCount && DrawBounds == that.DrawBounds && Async == that.Async;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return CellInGroupCount.GetHashCode() * 3 + DrawBounds.GetHashCode() * 7 + Async.GetHashCode() * 13;
            }
        }
    }
}