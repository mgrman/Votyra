using Votyra.Core.Models;

namespace Votyra.Core
{
    public interface IInitialImageConfig
    {
        object InitialData { get; }
        Vector3f InitialDataScale { get; }
    }
}