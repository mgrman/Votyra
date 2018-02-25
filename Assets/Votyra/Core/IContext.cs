using Votyra.Core.Logging;
using Votyra.Core.Profiling;

namespace Votyra.Core
{
    public interface IContext
    {
        ProfilerFactoryDelegate ProfilerFactory { get; }
        LoggerFactoryDelegate LoggerFactory { get; }
    }
}