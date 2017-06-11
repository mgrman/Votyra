using System;

namespace Votyra.Logging
{
    public interface ILogger
    {
        void LogMessage(object message);

        void LogError(object message);

        void LogException(Exception exception);

        void LogWarning(object message);
    }
}