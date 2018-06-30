using System;

namespace Votyra.Core.Logging
{
    public interface IThreadSafeLogger
    {
        void LogError(object message);

        void LogException(Exception exception);

        void LogMessage(object message);

        void LogWarning(object message);
    }
}