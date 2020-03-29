namespace Votyra.Core.Images
{
    public class LayerConfig : ILayerConfig
    {
        public LayerConfig([ConfigInject("layer")] LayerId layer)
        {
            Layer = layer;
        }

        public LayerId Layer { get; }
    }
}
