using System;

namespace Votyra.Logging
{
    public static class LoggerFactory
    {
        public static Func<string, object, IThreadSafeLogger> Factory { get; set; }

        public static IThreadSafeLogger Create<T>()
        {
            return Factory(typeof(T).FullName, null);
        }

        public static IThreadSafeLogger Create(Type type)
        {
            return Factory(type.FullName, null);
        }

        public static IThreadSafeLogger Create(object instance)
        {
            return Factory(instance.GetType().FullName, instance);
        }
    }
}