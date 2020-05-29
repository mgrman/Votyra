using Votyra.Core.Models;

namespace Votyra.Core.Images
{
    public class TerrainConfig : ITerrainConfig
    {
        public TerrainConfig([ConfigInject("cellInGroupCount")] Vector3i cellInGroupCount, [ConfigInject("drawBounds")] bool drawBounds, [ConfigInject("asyncTerrainGeneration")] bool asyncTerrainGeneration, [ConfigInject("asyncInput")] bool asyncInput, [ConfigInject("colliderType")] ColliderType colliderType)
        {
            CellInGroupCount = cellInGroupCount;
            DrawBounds = drawBounds;
#if UNITY_WEBGL && !UNITY_EDITOR
            AsyncTerrainGeneration = false;
            AsyncTerrainGeneration = false;
#else
            AsyncTerrainGeneration = asyncTerrainGeneration;
            AsyncInput = asyncInput;
#endif
            ColliderType = colliderType;
        }

        public Vector3i CellInGroupCount { get; }

        public ColliderType ColliderType { get; }

        public bool DrawBounds { get; }

        public bool AsyncTerrainGeneration { get; }

        public bool AsyncInput { get; }

        public static bool operator ==(TerrainConfig a, TerrainConfig b) => a?.Equals(b) ?? b?.Equals(a) ?? true;

        public static bool operator !=(TerrainConfig a, TerrainConfig b) => !(a == b);

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var that = obj as TerrainConfig;

            return CellInGroupCount == that.CellInGroupCount  && DrawBounds == that.DrawBounds && AsyncTerrainGeneration == that.AsyncTerrainGeneration && ColliderType == that.ColliderType;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return CellInGroupCount.GetHashCode() + DrawBounds.GetHashCode() * 7 + AsyncTerrainGeneration.GetHashCode() * 13 + ColliderType.GetHashCode() * 13;
            }
        }
    }
}
