using Votyra.Core.Models;

namespace Votyra.Core.Images
{
    public class LayerConfig : ILayerConfig
    {
        public LayerConfig([ConfigInject("layer")] int layer)
        {
            Layer = layer;
        }

        public int Layer { get; }
    }
}