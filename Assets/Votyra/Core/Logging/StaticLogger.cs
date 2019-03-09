using System;

namespace Votyra.Core.Logging
{
    public static class StaticLogger
    {
        public static IThreadSafeLogger LoggerImplementation { get; set; }

        public static void LogMessage(object message) => LoggerImplementation?.LogMessage(message);

        public static void LogError(object message) => LoggerImplementation?.LogError(message);

        public static void LogException(Exception exception) => LoggerImplementation?.LogException(exception);

        public static void LogWarning(object message) => LoggerImplementation?.LogWarning(message);
    }
}