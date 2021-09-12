using Votyra.Core.Models;

namespace Votyra.Core
{
    public interface IImageConfig : IConfig
    {
        Vector2i ImageSize { get; }
    }
}