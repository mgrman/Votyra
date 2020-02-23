using Votyra.Core.Models;

namespace Votyra.Core
{
    public interface IInitialImageConfig : ISharedConfig
    {
        object InitialData { get; }
        Vector3f InitialDataScale { get; }
        bool ZeroFromInitialStateIsNull { get; }
    }
}