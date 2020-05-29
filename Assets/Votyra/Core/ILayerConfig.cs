namespace Votyra.Core
{
    public interface ILayerConfig : ISharedConfig
    {
        LayerId Layer { get; }
    }

    public enum LayerId
    {
        Rock = 257,
        Earth = 258,
        Sand = 259,
    }
}
