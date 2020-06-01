using System;

namespace Votyra.Core.Logging
{
    public interface IThreadSafeLogger
    {
        void LogDebug(object message);

        void LogInfo(object message);

        void LogError(object message);

        void LogWarning(object message);
    }
}
