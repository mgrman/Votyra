using System;

namespace Votyra.Core.Logging
{
    public static class LoggerFactoryExtensions
    {
        public static IThreadSafeLogger Create<T>(this LoggerFactoryDelegate factory)
        {
            return factory(typeof(T).FullName, null);
        }

        public static IThreadSafeLogger Create(this LoggerFactoryDelegate factory, Type type)
        {
            return factory(type.FullName, null);
        }

        public static IThreadSafeLogger Create(this LoggerFactoryDelegate factory, string name)
        {
            return factory(name, null);
        }
        public static IThreadSafeLogger Create(this LoggerFactoryDelegate factory, object instance)
        {
            return factory(instance.GetType().FullName, instance);
        }
    }
}
