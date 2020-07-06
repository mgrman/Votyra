using Votyra.Core.Models;

namespace Votyra.Core.Images
{
    public class TerrainConfig : ITerrainConfig
    {
        public TerrainConfig([ConfigInject("cellInGroupCount")]Vector3i cellInGroupCount, [ConfigInject("drawBounds")]bool drawBounds, [ConfigInject("asyncTerrainGeneration")]bool asyncTerrainGeneration, [ConfigInject("asyncInput")]bool asyncInput, [ConfigInject("colliderType")]ColliderType colliderType)
        {
            this.CellInGroupCount = cellInGroupCount;
            this.DrawBounds = drawBounds;
#if UNITY_WEBGL && !UNITY_EDITOR
            this.AsyncTerrainGeneration = false;
            this.AsyncInput = false;
#else
            this.AsyncTerrainGeneration = asyncTerrainGeneration;
            this.AsyncInput = asyncInput;
#endif
            this.ColliderType = colliderType;
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
            if ((obj == null) || (this.GetType() != obj.GetType()))
            {
                return false;
            }

            var that = obj as TerrainConfig;

            return (this.CellInGroupCount == that.CellInGroupCount) && (this.DrawBounds == that.DrawBounds) && (this.AsyncTerrainGeneration == that.AsyncTerrainGeneration) && (this.ColliderType == that.ColliderType);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return this.CellInGroupCount.GetHashCode() + (this.DrawBounds.GetHashCode() * 7) + (this.AsyncTerrainGeneration.GetHashCode() * 13) + (this.ColliderType.GetHashCode() * 13);
            }
        }
    }
}
