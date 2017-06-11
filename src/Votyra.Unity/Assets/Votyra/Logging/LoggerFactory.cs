using System;

namespace Votyra.Logging
{
    public static class LoggerFactory
    {
        public static Func<string, ILogger> Factory { get; set; }

        public static ILogger Create<T>()
        {
            return Factory(typeof(T).FullName);
        }

        public static ILogger Create(Type type)
        {
            return Factory(type.FullName);
        }

        public static ILogger Create(object instance)
        {
            return Factory(instance.GetType().FullName);
        }
    }
}