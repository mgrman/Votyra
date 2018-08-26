using Votyra.Core.Models;

namespace Votyra.Core
{
    public interface IImageConfig : IConfig
    {
        Vector3i ImageSize { get; }
    }
}