using System;

namespace Votyra.Logging
{
    public static class LoggerFactoryExtensions
    {

        public static IThreadSafeLogger CreateLogger(this object instance)
        {
            return LoggerFactory.Create(instance);
        }
    }
}