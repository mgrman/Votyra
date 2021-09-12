using Votyra.Core.Models;

namespace Votyra.Core
{
    public interface IInitialImageConfig : IConfig
    {
        object InitialData { get; }
        Vector3f InitialDataScale { get; }
    }
}