using Votyra.Core.Models;

namespace Votyra.Core.Images
{
    public class TerrainConfig : ITerrainConfig
    {
        public TerrainConfig([ConfigInject("cellInGroupCount")] Vector2i cellInGroupCount, [ConfigInject("async")] bool async)
        {
            CellInGroupCount = cellInGroupCount;
#if UNITY_WEBGL && !UNITY_EDITOR
            Async = false;
#else
            Async = async;
#endif
        }

        public Vector2i CellInGroupCount { get; }
        
        public bool Async { get; }

        public static bool operator ==(TerrainConfig a, TerrainConfig b) => a?.Equals(b) ?? b?.Equals(a) ?? true;

        public static bool operator !=(TerrainConfig a, TerrainConfig b) => !(a == b);

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;
            var that = obj as TerrainConfig;

            return CellInGroupCount == that.CellInGroupCount  && Async == that.Async;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return CellInGroupCount.GetHashCode() * 3 + Async.GetHashCode() * 13;
            }
        }
    }
}