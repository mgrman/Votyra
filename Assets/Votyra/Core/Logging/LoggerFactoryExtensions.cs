using System;

namespace Votyra.Core.Logging
{
    public static class LoggerFactoryExtensions
    {
        public static LoggerFactoryDelegate factory;


        public static IThreadSafeLogger Create<T>(this LoggerFactoryDelegate factory)
        {
            if (factory == null)
                return null;
            return factory(typeof(T).FullName, null);
        }

        public static IThreadSafeLogger Create(this LoggerFactoryDelegate factory, Type type)
        {
            if (factory == null)
                return null;
            return factory(type.FullName, null);
        }

        public static IThreadSafeLogger Create(this LoggerFactoryDelegate factory, string name)
        {
            if (factory == null)
                return null;
            return factory(name, null);
        }

        public static IThreadSafeLogger Create(this LoggerFactoryDelegate factory, object instance)
        {
            if (factory == null)
                return null;
            return factory(instance.GetType().FullName, instance);
        }
    }
}