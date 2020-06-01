using Votyra.Core.Models;

namespace Votyra.Core
{
    public interface ILogConfig : ISharedConfig
    {
        LogLevel LogLevel { get; }
    }

    public enum LogLevel
    {
        None = 0,
        Error = 1,
        Warnings = 2,
        Info = 3,
        Debug = 4,
    }
}
