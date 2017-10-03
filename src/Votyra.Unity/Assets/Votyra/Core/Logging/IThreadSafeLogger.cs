using System;

namespace Votyra.Core.Logging
{
    public interface IThreadSafeLogger
    {
        void LogMessage(object message);

        void LogError(object message);

        void LogException(Exception exception);

        void LogWarning(object message);
    }
}
