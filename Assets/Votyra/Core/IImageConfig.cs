using Votyra.Core.Models;

namespace Votyra.Core
{
    public interface IImageConfig : ISharedConfig
    {
        Vector3i ImageSize { get; }
    }
}